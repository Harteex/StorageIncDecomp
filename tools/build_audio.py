#!/usr/bin/env python3
"""Convert the Xbox 360 XACT audio for the FNA port (used by tools/build_content.py).

FNA's XACT implementation (FACT, in FAudio) accepts big-endian Xbox engine (.xgs) and sound
bank (.xsb) files, but reads their floats without byte-swapping (FAudio bug), so they are
converted to little-endian by tools/xact_endian.py. Wave banks (.xwb) are rebuilt:
the Xbox banks hold XMA2, which FAudio can only decode through GStreamer/FFmpeg (absent from
the Windows fnalibs). Each entry is decoded once with ffmpeg and written into a PC wave
bank (little-endian 'WBND', same XACT version) as 16-bit PCM. Entry order, sample rates
and loop regions are preserved, so the sound banks' wave indices stay valid.
"""
import os
import struct
import sys
import tempfile
import wave

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from xact_endian import convert_xgs, convert_xsb  # noqa: E402
from xwb_extract import TAG_PCM, TAG_XMA, decode_xma, find_ffmpeg, parse_xwb  # noqa: E402

BANKDATA_SIZE = 0x60
METADATA_SIZE = 24
NAME_SIZE = 64
ALIGNMENT = 2048


def decode_entry(entry, ffmpeg):
    """Return little-endian 16-bit PCM bytes for one wave bank entry."""
    if entry["format_tag"] == TAG_XMA:
        with tempfile.TemporaryDirectory() as tmp:
            out = os.path.join(tmp, "out.wav")
            decode_xma(ffmpeg, entry, out)
            with wave.open(out) as w:
                if w.getsampwidth() != 2 or w.getnchannels() != entry["channels"]:
                    raise ValueError("unexpected ffmpeg output format")
                return w.readframes(w.getnframes())
    if entry["format_tag"] == TAG_PCM and entry["bits_per_sample"] == 16:
        data = entry["_data"]
        return bytes(b for i in range(0, len(data), 2) for b in (data[i + 1], data[i]))
    raise NotImplementedError(f"format tag {entry['format_tag']}")


def mini_wave_format(channels, rate):
    """XACT 3 MINIWAVEFORMAT: tag:2 channels:3 rate:18 blockAlign:8 bits:1 (PCM, 16-bit)."""
    block_align = channels * 2
    return TAG_PCM | (channels << 2) | (rate << 5) | (block_align << 23) | (1 << 31)


def build_pc_wavebank(bank, pcm_entries):
    count = len(bank["entries"])
    seg_bank = 0x34
    seg_meta = seg_bank + BANKDATA_SIZE
    seg_seek = seg_meta + METADATA_SIZE * count
    seg_names = seg_seek
    seg_data = (seg_names + ALIGNMENT - 1) // ALIGNMENT * ALIGNMENT

    metadata = bytearray()
    wavedata = bytearray()
    for e, pcm in zip(bank["entries"], pcm_entries):
        samples = len(pcm) // (2 * e["channels"])
        offset = len(wavedata)
        wavedata += pcm
        wavedata += b"\0" * (-len(wavedata) % ALIGNMENT)
        metadata += struct.pack("<6I", (samples << 4) | e["flags"], mini_wave_format(e["channels"], e["sample_rate"]),
                                offset, len(pcm), e["loop_start_samples"], e["loop_length_samples"])

    name = bank["name"].encode("ascii")[:63].ljust(64, b"\0")
    bankdata = struct.pack("<II", 0, count) + name + struct.pack("<IIII", METADATA_SIZE, NAME_SIZE, ALIGNMENT, 0) + b"\0" * 8
    segments = [(seg_bank, BANKDATA_SIZE), (seg_meta, len(metadata)), (seg_seek, 0), (seg_names, 0),
                (seg_data, len(wavedata))]
    header = b"WBND" + struct.pack("<II", bank["version"], bank["header_version"])
    header += b"".join(struct.pack("<II", o, l) for o, l in segments)
    out = bytearray(header + bankdata + metadata)
    out += b"\0" * (seg_data - len(out))
    return bytes(out + wavedata)


def convert_wavebank(src, dst, ffmpeg):
    bank = parse_xwb(open(src, "rb").read())
    pcm = [decode_entry(e, ffmpeg) for e in bank["entries"]]
    with open(dst, "wb") as f:
        f.write(build_pc_wavebank(bank, pcm))
    return len(pcm)


def convert_audio_file(src, dst, ffmpeg=None):
    """Convert or copy one XACT file; returns a short description."""
    ext = os.path.splitext(src)[1].lower()
    if ext in (".xgs", ".xsb"):
        data = open(src, "rb").read()
        if data[:4] in (b"FSGX", b"KBDS"):  # big-endian Xbox file
            data = convert_xgs(data) if ext == ".xgs" else convert_xsb(data)
            result = "converted to little-endian"
        else:
            result = "copied"
        with open(dst, "wb") as f:
            f.write(data)
        return result
    if ext == ".xwb":
        ffmpeg = ffmpeg or find_ffmpeg(None)
        if not ffmpeg:
            raise RuntimeError("ffmpeg not found (needed to decode XMA)")
        return f"{convert_wavebank(src, dst, ffmpeg)} waves -> PCM"
    raise ValueError(f"not an XACT file: {src}")
