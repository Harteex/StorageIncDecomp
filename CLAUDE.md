# Storage Inc — Decompilation Project

Decompilation of **Storage Inc**, an Xbox 360 Indie Game (XBLIG, built with XNA Game Studio).
Store description: "Cooperative arcade puzzle game for 1-4 Local players. 100 Maps, all with full coop support."

Phase 1 (done): unpack the package, decompile the managed code to readable C#, and extract the content assets.
Phase 2 (in progress): port to PC on **FNA** (`src/`). It compiles and content loads (converted to XNA 4 XNBs); shaders and audio are next. See `docs/porting.md`.

## Source files (repo root — treat as read-only originals, never modify)

| File | What it is |
|---|---|
| `7F40FC5F96CDEA326B02027C03A249463814C12D58 (my drive)` | **Primary input.** Copied from the user's own Xbox 360 drive. |
| `7F40FC5F96CDEA326B02027C03A249463814C12D58 (dl)` | Same package from another source. Differs from "my drive" only in bytes `0x004–0x257` (the LIVE signature block); content is identical. |

## Known facts about the package

- Container: **STFS**, magic `LIVE` (Microsoft-signed), size 7,520,256 bytes.
- Title ID `0x584E07D2` = the shared XNA Indie Games host title ("Indie Games"). Content type folder `00000002`.
- Display name at header offset `0x411` (UTF-16BE): "Storage Inc". Description at `0xD11`.
- Extract with `python tools/stfs_extract.py "7F40…(my drive)" extracted` (format notes in `docs/stfs.md`).

## Package contents (`extracted/`)

- Root: `GameInfo.xml` (UTF-16; released 24 Aug 2009), `GameInfo.bin`, `DashboardIcon.png`, `TitleImage.png`.
- `584E07D1/PhysCoop.exe`: the game (internal name "PhysCoop"). It targets **XNA 3.0** (runtime profile `Xbox 360.v3.0`) on .NET CF 2.0 (`v2.0.50727`) and uses GamerServices, Storage, Audio and System.Xml.Serialization.
- `584E07D1/FarseerProject.dll` (AssemblyVersion 2.0.0.0): the **Farseer Physics** engine (open source, so this can likely be replaced with the matching upstream release).
- `Content/`: `.xnb` files with the header `XNBx` v3 and flag `0x80`, meaning Xbox platform, LZX-compressed. Includes models, textures, deferred-lighting/shadow effects, and fonts. The game uses the XNA "GameStateManagement" sample (`ScreenManagerResources`).
- `Content/audio`, `Content/ScreenManagerResources/menuAudio`: XACT banks (`.xgs`/`.xsb`/`.xwb`).
- `Content/stages/001–100.xml`: plain XmlSerializer `StageData` files (wall polygons as `Vector2` arrays, ...). `stages/mapimages/` holds a preview `.xnb` for each stage.

## Tooling

- Shells: PowerShell (default) and Git Bash. Python 3.13 and .NET SDK 10 are installed.
- Convert XNB content with `python tools/xnb_extract.py` (defaults: `extracted/584E07D1/Content` → `assets/`). Format notes are in `docs/xnb.md`. Xbox GPU data (textures, vertex/index buffers) is big-endian.
- Extract audio with `python tools/xwb_extract.py` (same defaults, XMA2 → WAV, needs ffmpeg; named by XSB cue). Notes are in `docs/audio.md`.
- ILSpy is a repo-local dotnet tool (`dotnet-tools.json`, ilspycmd 11.1). Run `dotnet tool restore`, then regenerate `decomp/` with:
  `dotnet ilspycmd -p -o decomp/PhysCoop -r extracted/584E07D1 extracted/584E07D1/PhysCoop.exe` (same for `FarseerProject.dll`).
- Port build: `git submodule update --init --recursive`, `python tools/fetch_fnalibs.py`, `python tools/build_content.py`, `dotnet build StorageInc.slnx`. Verify content with `dotnet run --project tools/ContentCheck`.
- Prefer writing small, reproducible Python scripts in `tools/` over one-off GUI tools, so every extraction step can be re-run.

## Layout

```
tools/        Python helper scripts (STFS extractor, XNB/XWB unpackers, decompiler fixups, fnalibs fetch)
extracted/    Raw files pulled out of the STFS package (generated, gitignored)
decomp/       Raw ILSpy output: the untouched reference, never edit
src/          The FNA port: StorageInc (game) + FarseerPhysics, StorageInc.slnx at the root
external/FNA  FNA git submodule (upstream; never modify)
fnalibs/      FNA native libraries (fetched, gitignored)
build/        Converted XNA 4 PC content for the port (tools/build_content.py, gitignored)
assets/       Converted content: PNG textures/fonts, OBJ models, .fxo effects, WAV audio (generated, gitignored)
docs/         Notes on formats, porting, findings
```

## Conventions

- The original packages, `extracted/` and `assets/` are gitignored (copyrighted, and regenerable).
- Never edit the original package files; all outputs must be regenerable from them via `tools/`.
- Record format discoveries (STFS quirks, XNB reader types, custom map formats) in `docs/`.
- `decomp/` stays raw ILSpy output. All porting and cleanup happens in `src/`, in separate commits so diffs stay reviewable.
- **XNA 3.0 enums are numbered differently from XNA 4.0/FNA.** Translate raw casts like `(SurfaceFormat)22` using the table in `docs/porting.md`. Never decompile with FNA as the XNA reference.
- Watch for decompiled base calls written as `((BaseType)this).SameMethod(...)`, which recurse infinitely. Verify against the IL (`dotnet ilspycmd -il`).
- **Never modify `external/FNA`.** FNA's policy forbids AI-generated contributions; we only consume it.

## Decompiled code (`decomp/`)

- `decomp/PhysCoop/`: the game, about 9k lines. Namespaces are `PhysCoop` (gameplay plus GameStateManagement screens), `PhysCoop.Oak` (`Draw2D`, from the developer's own "Oak" engine), and `BallGame_Editor` (`StageData`, the stage XML schema, apparently left over from an earlier "BallGame" project).
- `decomp/FarseerProject/`: Farseer Physics 2.0, about 11k lines.
- Known decompiler artifacts: 8 `goto IL_00bc` in `PhysCoop/PhysCoop/InputState.cs`. `ref` is used where XNA has `out`, and `(int)` casts on `PlayerIndex` are missing (`tools/fix_decomp.py` fixes both in `src/`). The csprojs in `decomp/` don't build; use `src/`.
