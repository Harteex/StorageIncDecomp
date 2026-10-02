# Audio notes (XACT, Xbox 360)

Implemented in `tools/xwb_extract.py`. ffmpeg is required for XMA. It is found on PATH or at `C:\Program2\ffmpeg\bin\ffmpeg.exe`, or you can pass `--ffmpeg`.

    python tools/xwb_extract.py extracted/584E07D1/Content assets

Output: `assets/<bank dir>/Wave Bank/NNN_<CueName>.wav`, plus `Wave Bank.json` holding the entry table, loop points and the cue to wave mapping.

## Banks

| Bank | Engine (`.xgs`) | Waves | Notes |
|---|---|---|---|
| `Content/audio/` | `GameSounds.xgs` | 22 | Gameplay SFX |
| `Content/ScreenManagerResources/menuAudio/` | `MenuSounds.xgs` | 8 | Menu SFX, `Menu_Music` (38 s loop), MapComplete/MapFail jingles |

Both wave banks are internally named "Wave Bank" and both sound banks "Sound Bank". The game loads them as two separate AudioEngine/WaveBank/SoundBank sets (see `ScreenManager.cs`, `GameplayScreen.cs`).

## Wave bank (`.xwb`)

- Magic `DNBW`, meaning a **big-endian** Xbox bank (PC banks are `WBND`, little-endian). Tool version 43, header version 42.
- Header: 5 segments (offset, length): bank data, entry metadata, seek tables, entry names (empty here), wave data.
- Bank data: flags (`0x80000` = has seek tables), entry count, 64-byte name, metadata element size (24), name element size, alignment (2048).
- Entry (24 bytes): flags:4 | duration:28 (samples), MINIWAVEFORMAT, play offset, play length, loop start, loop length (in samples).
- MINIWAVEFORMAT (v39+): tag:2 (0 PCM, 1 XMA, 2 ADPCM, 3 WMA), channels:3, rate:18, block align:8, bits:1.
- Every entry here is **XMA2**, mono. The stored sample rates are slightly off 44100 (for example 44162 or 45692). vgmstream uses them as-is, and so do we.
- Decoding wraps each entry in a RIFF with an `XMA2WAVEFORMATEX` header (tag `0x166`, 34-byte extension) and runs ffmpeg. The decoded sample counts match the bank durations exactly.

## Sound bank (`.xsb`)

- Magic `KBDS` (big-endian), version 43. The layout follows vgmstream's `xwb_xsb.h`, which `XsbParser` in the tool mirrors.
- Cues point to sounds (simple = one wave, complex = clips of play-wave events) or to variation tables (random choice).
- Random-variation cues: `GoodsDrop` (3 waves), `Bump` (4), `TruckCollideHard` (5). Their files get `_1`, `_2`, … suffixes.
- All 21 cue names used by the code exist in the banks, and every wave is reached by some cue.
