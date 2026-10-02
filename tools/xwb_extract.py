#!/usr/bin/env python3
"""Extract XACT wave banks (.xwb) from the Xbox 360 build to WAV.

Usage:
    python tools/xwb_extract.py [content_dir] [out_dir] [--ffmpeg PATH]

Xbox 360 wave banks are big-endian and hold XMA2 audio (XACT version 43). Each entry is
wrapped in a RIFF/XMA2 container (WAVE_FORMAT_XMA2 = 0x166) and decoded to 16-bit PCM
with ffmpeg. PCM entries (format tag 0) are written directly. Loop points and the raw
entry table go to a .json next to each bank's output folder. See docs/audio.md.
"""
import argparse
import json
import os
import shutil
import struct
import subprocess
import sys
import tempfile

TAG_PCM, TAG_XMA, TAG_ADPCM, TAG_WMA = 0, 1, 2, 3
SEG_BANKDATA, SEG_ENTRYMETADATA, SEG_SEEKTABLES, SEG_ENTRYNAMES, SEG_ENTRYWAVEDATA = range(5)
DEFAULT_FFMPEG_PATHS = [r"C:\Program2\ffmpeg\bin\ffmpeg.exe"]


def parse_xwb(data):
    sig = data[:4]
    if sig == b"DNBW":
        e = ">"
    elif sig == b"WBND":
        e = "<"
    else:
        raise ValueError(f"not a wave bank: {sig!r}")
    version, header_version = struct.unpack_from(e + "II", data, 4)
    if version < 39:
        raise NotImplementedError(f"XACT version {version} (pre-XMA2 layout) not supported")
    segs = [struct.unpack_from(e + "II", data, 12 + 8 * i) for i in range(5)]
    bo = segs[SEG_BANKDATA][0]
    flags, count = struct.unpack_from(e + "II", data, bo)
    name = data[bo + 8:bo + 72].split(b"\0")[0].decode("ascii", "replace")
    meta_size, name_size, alignment, compact = struct.unpack_from(e + "IIII", data, bo + 72)
    if flags & 0x00020000:
        raise NotImplementedError("compact wave bank entries not supported")

    names = []
    if segs[SEG_ENTRYNAMES][1]:
        no = segs[SEG_ENTRYNAMES][0]
        names = [data[no + i * name_size:no + (i + 1) * name_size].split(b"\0")[0].decode("ascii", "replace")
                 for i in range(count)]

    wave_base = segs[SEG_ENTRYWAVEDATA][0]
    entries = []
    for i in range(count):
        flags_dur, fmt, play_off, play_len, loop_start, loop_len = struct.unpack_from(
            e + "6I", data, segs[SEG_ENTRYMETADATA][0] + i * meta_size)
        entries.append({
            "index": i,
            "name": names[i] if names else None,
            "flags": flags_dur & 0xF,
            "duration_samples": flags_dur >> 4,
            "format_tag": fmt & 0x3,
            "channels": (fmt >> 2) & 0x7,
            "sample_rate": (fmt >> 5) & 0x3FFFF,
            "block_align": (fmt >> 23) & 0xFF,
            "bits_per_sample": 16 if fmt >> 31 else 8,
            "play_offset": play_off,
            "play_length": play_len,
            "loop_start_samples": loop_start,
            "loop_length_samples": loop_len,
            "_data": data[wave_base + play_off:wave_base + play_off + play_len],
        })
    return {"name": name, "version": version, "header_version": header_version,
            "flags": flags, "alignment": alignment, "entries": entries}


class XsbParser:
    """Map XACT sound bank (.xsb, v43) cue names to wave bank entries.

    Follows vgmstream's xwb_xsb.h (v41+ layout). Returns {cue_name: [(wavebank, wave), ...]}.
    """

    def __init__(self, data):
        self.d = data
        if data[:4] == b"KBDS":
            self.e = ">"
        elif data[:4] == b"SDBK":
            self.e = "<"
        else:
            raise ValueError(f"not a sound bank: {data[:4]!r}")
        self.version = self.s16(4)
        if self.version <= 41:
            raise NotImplementedError(f"XSB version {self.version} not supported")
        self.simple_count = self.s16(0x13)
        self.complex_count = self.s16(0x15)
        self.wavebank_count = self.d[0x1B]
        self.simple_off = self.s32(0x22)
        self.complex_off = self.s32(0x26)
        self.wavebanks_off = self.s32(0x3A)
        self.nameoffsets_off = self.s32(0x42)
        self.name = self.cstr(0x4A)

    def s8(self, o):
        return struct.unpack_from("b", self.d, o)[0]

    def s16(self, o):
        return struct.unpack_from(self.e + "h", self.d, o)[0]

    def s32(self, o):
        return struct.unpack_from(self.e + "i", self.d, o)[0]

    def u32(self, o):
        return struct.unpack_from(self.e + "I", self.d, o)[0]

    def cstr(self, o):
        return self.d[o:self.d.index(b"\0", o)].decode("ascii", "replace")

    def wavebank_names(self):
        return [self.cstr(self.wavebanks_off + i * 0x40) for i in range(self.wavebank_count)]

    def cues(self):
        result = {}
        names_off = self.nameoffsets_off
        off = self.simple_off
        for i in range(self.simple_count):
            sound = self.s32(off + 1)
            off += 5
            name = self._cue_name(names_off, f"simple{i}")
            names_off += 6
            result[name] = self._sound(sound)
        off = self.complex_off
        for i in range(self.complex_count):
            flags = self.d[off]
            sound = self.s32(off + 1)
            off += 0x0F
            name = self._cue_name(names_off, f"complex{i}")
            names_off += 6
            result[name] = self._sound(sound) if flags & 4 else self._variation(sound)
        return result

    def _cue_name(self, names_off, fallback):
        if self.nameoffsets_off <= 0:
            return fallback
        o = self.s32(names_off)
        return self.cstr(o) if 0 < o < len(self.d) else fallback

    def _sound(self, off):
        waves = []
        flags = self.d[off]
        off += 9
        clip_count = 0
        if flags & 0x01:
            clip_count = self.d[off]
            off += 1
        else:
            waves.append((self.s8(off + 2), self.s16(off)))
            off += 3
        if flags & 0x0E:
            off += self.s16(off)  # RPC block
        if flags & 0x10:
            off += self.s16(off)  # DSP block
        for _ in range(clip_count):
            waves += self._clip(self.s32(off + 1))
            off += 9
        return waves

    def _clip(self, off):
        waves = []
        count = self.d[off]
        off += 1
        for _ in range(count):
            event = self.u32(off) & 0x1F
            off += 6
            if event in (1, 4):
                waves.append((self.s8(off + 4), self.s16(off + 2)))
                off += 0x0A if event == 1 else 0x22
            elif event in (3, 6):
                tracks = self.u32(off + (0x07 if event == 3 else 0x1F)) & 0xFFFF
                off += 0x0F if event == 3 else 0x27
                for _ in range(tracks):
                    waves.append((self.s8(off + 2), self.s16(off)))
                    off += 5
            elif event == 8:
                off += 0x10
            else:
                raise NotImplementedError(f"XSB clip event {event}")
        return waves

    def _variation(self, off):
        waves = []
        header = self.u32(off)
        count, kind = header & 0xFFFF, (header >> 19) & 7
        off += 8
        for _ in range(count):
            if kind in (0, 4):
                waves.append((self.s8(off + 2), self.s16(off)))
                off += 5 if kind == 0 else 3
            elif kind in (1, 3):
                waves += self._sound(self.s32(off))
                off += 6 if kind == 1 else 0x10
            else:
                raise NotImplementedError(f"XSB variation type {kind}")
        return waves


def wave_names_from_xsb(xsb_path, bank_name):
    """Return ({wave_index: name}, {cue: [wave indices]}) for the given wave bank."""
    xsb = XsbParser(open(xsb_path, "rb").read())
    banks = xsb.wavebank_names()
    cues = {}
    for cue, waves in xsb.cues().items():
        cues[cue] = [w for b, w in waves if 0 <= b < len(banks) and banks[b] == bank_name]
    names = {}
    for cue, waves in cues.items():
        unique = list(dict.fromkeys(waves))
        for k, w in enumerate(unique):
            names.setdefault(w, cue if len(unique) == 1 else f"{cue}_{k + 1}")
    return names, cues


def riff(fmt_chunk, data, extra_chunks=b""):
    body = b"WAVE" + b"fmt " + struct.pack("<I", len(fmt_chunk)) + fmt_chunk + extra_chunks
    body += b"data" + struct.pack("<I", len(data)) + data + (b"\0" if len(data) & 1 else b"")
    return b"RIFF" + struct.pack("<I", len(body)) + body


def xma2_riff(entry):
    ch, rate, n = entry["channels"], entry["sample_rate"], entry["duration_samples"]
    channel_mask = {1: 0x4, 2: 0x3}.get(ch, 0)
    # WAVEFORMATEX + XMA2WAVEFORMATEX extension (34 bytes)
    fmt = struct.pack("<HHIIHHH", 0x166, ch, rate, rate * ch * 2, 4, 16, 34)
    fmt += struct.pack("<HIIIIIIIBBH",
                       1,                    # NumStreams
                       channel_mask,         # ChannelMask
                       n,                    # SamplesEncoded
                       0x10000,              # BytesPerBlock
                       0, n,                 # PlayBegin, PlayLength
                       entry["loop_start_samples"], entry["loop_length_samples"],
                       0,                    # LoopCount
                       4,                    # EncoderVersion
                       max(1, (len(entry["_data"]) + 0xFFFF) // 0x10000))  # BlockCount
    return riff(fmt, entry["_data"])


def pcm_wav(entry):
    ch, rate, bits = entry["channels"], entry["sample_rate"], entry["bits_per_sample"]
    data = entry["_data"]
    if bits == 16:  # Xbox PCM is big-endian
        data = bytes(b for i in range(0, len(data), 2) for b in (data[i + 1], data[i]))
    fmt = struct.pack("<HHIIHH", 1, ch, rate, rate * ch * bits // 8, ch * bits // 8, bits)
    return riff(fmt, data)


def find_ffmpeg(explicit):
    for cand in [explicit, shutil.which("ffmpeg"), *DEFAULT_FFMPEG_PATHS]:
        if cand and os.path.exists(cand):
            return cand
    return None


def decode_xma(ffmpeg, entry, out_path):
    with tempfile.TemporaryDirectory() as tmp:
        src = os.path.join(tmp, "in.xma")
        with open(src, "wb") as f:
            f.write(xma2_riff(entry))
        r = subprocess.run([ffmpeg, "-v", "error", "-y", "-i", src, "-c:a", "pcm_s16le", out_path],
                           capture_output=True, text=True)
        if r.returncode != 0:
            raise RuntimeError(r.stderr.strip() or f"ffmpeg exited {r.returncode}")


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("content_dir", nargs="?", default="extracted/584E07D1/Content")
    ap.add_argument("out_dir", nargs="?", default="assets")
    ap.add_argument("--ffmpeg", help="path to ffmpeg (needed for XMA)")
    args = ap.parse_args()
    ffmpeg = find_ffmpeg(args.ffmpeg)

    failures = total = 0
    for root, _, files in os.walk(args.content_dir):
        for fn in sorted(files):
            if not fn.lower().endswith(".xwb"):
                continue
            src = os.path.join(root, fn)
            rel = os.path.relpath(src, args.content_dir)
            out_dir = os.path.join(args.out_dir, os.path.splitext(rel)[0])
            os.makedirs(out_dir, exist_ok=True)
            bank = parse_xwb(open(src, "rb").read())
            print(f"{rel}: '{bank['name']}' v{bank['version']}, {len(bank['entries'])} entries")
            cue_names, cues = {}, {}
            for xsb_fn in (f for f in files if f.lower().endswith(".xsb")):
                cue_names, cues = wave_names_from_xsb(os.path.join(root, xsb_fn), bank["name"])
                bank["sound_bank"] = xsb_fn
            bank["cues"] = cues
            for e in bank["entries"]:
                total += 1
                e["cue_name"] = cue_names.get(e["index"])
                label = e["name"] or e["cue_name"]
                out = os.path.join(out_dir, f"{e['index']:03d}" + (f"_{label}" if label else "") + ".wav")
                try:
                    if e["format_tag"] == TAG_PCM:
                        with open(out, "wb") as f:
                            f.write(pcm_wav(e))
                    elif e["format_tag"] == TAG_XMA:
                        if not ffmpeg:
                            raise RuntimeError("ffmpeg not found (use --ffmpeg)")
                        decode_xma(ffmpeg, e, out)
                    else:
                        raise NotImplementedError(f"format tag {e['format_tag']}")
                except Exception as ex:
                    failures += 1
                    print(f"  FAILED entry {e['index']}: {ex}")
            meta = {k: v for k, v in bank.items() if k != "entries"}
            meta["entries"] = [{k: v for k, v in e.items() if k != "_data"} for e in bank["entries"]]
            with open(out_dir + ".json", "w") as f:
                json.dump(meta, f, indent=1)
    print(f"{total - failures}/{total} entries extracted")
    return 1 if failures else 0


if __name__ == "__main__":
    sys.exit(main())
