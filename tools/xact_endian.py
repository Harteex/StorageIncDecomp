#!/usr/bin/env python3
"""Convert big-endian Xbox 360 XACT engine (.xgs) and sound bank (.xsb) files to little-endian.

Why: FACT (FAudio's XACT runtime, used by FNA) detects big-endian files and swaps integers,
but its read_f32() ignores the swap flag, so every float in an Xbox .xgs/.xsb is read
byte-reversed. For Storage Inc that broke the "Volume" cue variable (range -96..6 became
~7e-41), so the truck motor RPC never attenuated and the motor played at full volume.
Converting the files to the PC byte order sidesteps the bug, and stays correct if FAudio
fixes it later.

The walk mirrors FACT_INTERNAL_ParseAudioEngine / FACT_INTERNAL_ParseSoundBank: every
multi-byte field FACT reads is byte-reversed in place; bytes and strings are untouched, so
sizes and offsets stay the same. Tables FACT doesn't read (e.g. the cue hash table) are left
as they are. Only XACT 3.0 content (version 43) is handled, which is what this game uses.
"""
import struct
import sys


class Swapper:
    def __init__(self, data):
        self.d = bytearray(data)
        self.fields = {}  # offset -> size

    def be(self, fmt, off):
        return struct.unpack_from(">" + fmt, self.d, off)[0]

    def mark(self, off, size):
        prev = self.fields.get(off)
        if prev is not None and prev != size:
            raise ValueError(f"field at {off:#x} read as {prev} and {size} bytes")
        self.fields[off] = size

    def apply(self):
        for off, size in self.fields.items():
            self.d[off:off + size] = self.d[off:off + size][::-1]
        return bytes(self.d)


class Cursor:
    """Sequential reader that records every multi-byte field it reads."""

    def __init__(self, sw, pos):
        self.sw = sw
        self.p = pos

    def skip(self, n):
        self.p += n

    def u8(self):
        v = self.sw.d[self.p]
        self.p += 1
        return v

    def _read(self, fmt, size):
        v = self.sw.be(fmt, self.p)
        self.sw.mark(self.p, size)
        self.p += size
        return v

    def u16(self):
        return self._read("H", 2)

    def s16(self):
        return self._read("h", 2)

    def u32(self):
        return self._read("I", 4)

    def s32(self):
        return self._read("i", 4)

    def f32(self):
        return self._read("f", 4)

    def u64(self):
        return self._read("Q", 8)


def convert_xgs(data):
    if data[:4] != b"FSGX":
        raise ValueError("not a big-endian XACT engine file")
    sw = Swapper(data)
    c = Cursor(sw, 0)
    c.u32()  # magic: reversing 'FSGX' gives 'XGSF'
    if c.u16() != 43:
        raise NotImplementedError("only XACT 3.0 content is supported")
    c.u16()  # tool version
    c.u16()  # unknown
    c.u64()  # last modified
    c.u8()  # platform/version
    categories, variables, _blob1, _blob2, rpcs, dsp_presets, _dsp_params = [c.u16() for _ in range(7)]
    (cat_off, var_off, _b1, cat_name_idx, _b2, var_name_idx, _cat_names, _var_names,
     rpc_off, dsp_off, dsp_param_off) = [c.u32() for _ in range(11)]

    c.p = cat_off
    for _ in range(categories):
        c.u8(); c.u16(); c.u16(); c.u8(); c.u16(); c.u8(); c.u8()
    c.p = var_off
    for _ in range(variables):
        c.u8(); c.f32(); c.f32(); c.f32()
    c.p = rpc_off
    for _ in range(rpcs):
        c.u16()
        points = c.u8()
        c.u16()
        for _ in range(points):
            c.f32(); c.f32(); c.u8()
    param_counts = []
    c.p = dsp_off
    for _ in range(dsp_presets):
        c.u8()
        param_counts.append(c.u16())
        c.u16()
    c.p = dsp_param_off
    for n in param_counts:
        for _ in range(n):
            c.u8(); c.f32(); c.f32(); c.f32(); c.u16()
    for idx_off, count in ((cat_name_idx, categories), (var_name_idx, variables)):
        c.p = idx_off
        for _ in range(count):
            c.u32(); c.u16()
    return sw.apply()


def _rpc_codes(c):
    for _ in range(c.u8()):
        c.u32()


def _track_events(c):
    for _ in range(c.u8()):
        info = c.u32()
        c.u16()  # random offset
        c.u8()  # separator
        kind = info & 0x1F
        if kind == 0:  # stop
            c.u8()
        elif kind == 1:  # play wave
            c.u8(); c.u16(); c.u8(); c.u8(); c.u16(); c.u16()
        elif kind in (3, 6):  # play wave track variation / track effect variation
            c.u8(); c.u8(); c.u16(); c.u16()
            if kind == 6:
                c.s16(); c.s16(); c.u8(); c.u8(); c.u8()  # pitch, volume, 3.0 variation flags
            waves = c.u32() & 0xFFFF
            c.skip(4)
            for _ in range(waves):
                c.u16(); c.u8(); c.u8(); c.u8()
        elif kind == 4:  # play wave effect variation (3.0 layout)
            c.u8(); c.u16(); c.u8(); c.u8(); c.u16(); c.u16()
            c.s16(); c.s16(); c.u8(); c.u8(); c.u8()
        elif kind in (7, 8, 16, 17):  # pitch, volume, pitch repeating, volume repeating
            settings = c.u8()
            if settings & 1:  # ramp
                c.f32(); c.f32(); c.f32(); c.u16()
            else:
                c.u8(); c.f32(); c.f32(); c.skip(5)
                if kind in (16, 17):
                    c.u16(); c.u16()
        elif kind == 9:  # marker
            c.u32()
        elif kind == 18:  # marker repeating
            c.u32(); c.u16(); c.u16()
        else:
            raise NotImplementedError(f"XACT event type {kind}")


def convert_xsb(data):
    if data[:4] != b"KBDS":
        raise ValueError("not a big-endian XACT sound bank")
    sw = Swapper(data)
    c = Cursor(sw, 0)
    c.u32()  # magic: reversing 'KBDS' gives 'SDBK'
    if c.u16() != 43:
        raise NotImplementedError("only XACT 3.0 content is supported")
    c.u16()  # tool
    c.u16()  # crc
    c.u64()  # last modified
    c.u8()  # platform
    simple, complex_ = c.u16(), c.u16()
    c.u16(); c.u16()
    c.u8()  # wave bank count
    sounds = c.u16()
    c.u16(); c.u16()
    simple_off, complex_off, _names_off = c.s32(), c.s32(), c.s32()
    c.u32()
    variation_off, transition_off, _wb_names, _hash_off, name_idx_off, sound_off = [c.s32() for _ in range(6)]

    c.p = sound_off
    for _ in range(sounds):
        flags = c.u8()
        c.u16(); c.u8(); c.s16(); c.u8(); c.u16()
        tracks = 1
        if flags & 0x01:
            tracks = c.u8()
        else:
            c.u16(); c.u8()
        if flags & 0x0E:
            start = c.p
            length = c.u16()
            if flags & 0x02:
                _rpc_codes(c)
            if flags & 0x04:
                for _ in range(tracks):
                    _rpc_codes(c)
            c.p = start + length
        if flags & 0x10:
            c.u16()
            for _ in range(c.u8()):
                c.u32()
        if flags & 0x01:
            codes = []
            for _ in range(tracks):
                c.u8()
                codes.append(c.u32())
            for code in codes:
                c.p = code
                _track_events(c)

    variations = transitions = 0
    c.p = simple_off
    for _ in range(simple if simple_off != -1 else 0):
        c.u8(); c.u32()
    c.p = complex_off
    for _ in range(complex_ if complex_off != -1 else 0):
        flags = c.u8()
        c.u32()
        transition = c.u32()
        c.u8(); c.u16(); c.u16(); c.u8()
        variations += not (flags & 0x04)
        transitions += transition not in (0, 0xFFFFFFFF)

    c.p = variation_off
    for _ in range(variations):
        kind = (c.u32() >> 19) & 0x07
        entries = sw.be("I", c.p - 4) & 0xFFFF
        c.u16(); c.s16()
        for _ in range(entries):
            if kind == 0:
                c.u16(); c.u8(); c.u8(); c.u8()
            elif kind == 1:
                c.u32(); c.u8(); c.u8()
            elif kind == 3:
                c.u32(); c.f32(); c.f32(); c.u32()
            elif kind == 4:
                c.u16(); c.u8()
            else:
                raise NotImplementedError(f"variation table type {kind}")
    c.p = transition_off
    for _ in range(transitions):
        for _ in range(c.u32()):
            c.s32(); c.u32(); c.u32(); c.u32(); c.u32(); c.u16(); c.u16(); c.u16()

    c.p = name_idx_off
    for _ in range(simple + complex_ if name_idx_off != -1 else 0):
        c.u32(); c.u16()
    return sw.apply()


def convert(path_in, path_out):
    data = open(path_in, "rb").read()
    out = convert_xgs(data) if data[:4] == b"FSGX" else convert_xsb(data)
    with open(path_out, "wb") as f:
        f.write(out)
    return len(out)


if __name__ == "__main__":
    for p in sys.argv[1:]:
        print(p, convert(p, p + ".le"))
