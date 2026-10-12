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

## In the FNA port

FNA's XACT implementation is FACT, part of FAudio. FACT accepts **big-endian Xbox** engine (`.xgs`, tool version 42, XACT platform 7) and sound bank (`.xsb`) files and swaps their integers, but **its `read_f32()` ignores the swap flag**. Every float in those files is therefore read byte-reversed. Here that turned the `Volume` cue variable range −96..6 into about 7e-41, so the RPC that attenuates `TruckMotor` never applied and the motor played at full volume all the time.

`tools/xact_endian.py` converts both file types to **little-endian PC byte order**. It walks the same structure as FACT's parsers and byte-reverses each multi-byte field. Sizes and offsets don't change. This stays correct if FAudio fixes the bug later.

Measured through FNA with WASAPI loopback after the fix: `Volume` −96 is silent, and −25 → −1 raises the level 15.7× (expected 24 dB = 15.85×). `ContentCheck` checks that a new `TruckMotor` cue reads `Volume` = −96.

At −25 dB the motor still hums quietly while a player is in the game. That's the original design: the game sets `Volume = −25 + |left stick Y| × 24` every frame.

**FAudio 26.10 or newer is required.** Every gameplay sound is a complex sound whose single event is a "play wave effect variation" (event type 4: random pitch ±100 cents and random volume). In XACT 3.0 (content version 43), that event ends with a 1-byte variation flag. Earlier FACT versions parsed it in the 3.1 layout: 4 floats (filter frequency and Q) plus a u16 of flags. That read the next sound's bytes as filter settings and silenced every gameplay sound, motor included, while the cues still reported `IsPlaying`. The menu bank only has simple sounds and was unaffected. Upstream fixed it in FAudio commit 63669ee ("Fix bytecode parsing for version 3.0 wave event variations"), first released in 26.10. The fnalibs daily fetched on 2026-10-07 still contained 26.09. `ContentCheck` fails if the loaded FAudio is older than 26.10. `xact_endian.py` already uses the correct 3.0 layout.

XMA can only be decoded by FAudio through GStreamer/FFmpeg, which the Windows fnalibs don't include. `tools/build_audio.py` therefore rebuilds each wave bank as a **PC wave bank**:
- Format: `WBND`, little-endian, same version 43 / header 42, 2048-byte alignment, no seek tables.
- Each entry is decoded once with ffmpeg and stored as 16-bit PCM.
- Entry order, sample rates and loop regions (in samples) are unchanged, so the sound banks' wave indices still match.

`dotnet run --project tools/ContentCheck` loads both engines and banks through FNA and starts every cue the game uses. All 21 start playing.

## Sound bank (`.xsb`)

- Magic `KBDS` (big-endian), version 43. The layout follows vgmstream's `xwb_xsb.h`, which `XsbParser` in the tool mirrors.
- Cues point to sounds (simple = one wave, complex = clips of play-wave events) or to variation tables (random choice).
- Random-variation cues: `GoodsDrop` (3 waves), `Bump` (4), `TruckCollideHard` (5). Their files get `_1`, `_2`, … suffixes.
- All 21 cue names used by the code exist in the banks, and every wave is reached by some cue.
