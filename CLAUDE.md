# Storage Inc — Decompilation Project

Decompilation of **Storage Inc**, an Xbox 360 Indie Game (XBLIG, built with XNA Game Studio).
Store description: "Cooperative arcade puzzle game for 1-4 Local players. 100 Maps, all with full coop support."

Phase 1 goal: unpack the package, decompile the managed code to readable C#, and extract the content assets.
Later phases (not yet decided): rebuild / port to PC (likely MonoGame or FNA).

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
- `584E07D1/PhysCoop.exe`: the game (internal name "PhysCoop"). It targets **XNA 3.x** on .NET CF 2.0 (`v2.0.50727`) and uses GamerServices, Storage, Audio and System.Xml.Serialization.
- `584E07D1/FarseerProject.dll` (AssemblyVersion 2.0.0.0): the **Farseer Physics** engine (open source, so this can likely be replaced with the matching upstream release).
- `Content/`: `.xnb` files with the header `XNBx` v3 and flag `0x80`, meaning Xbox platform, LZX-compressed. Includes models, textures, deferred-lighting/shadow effects, and fonts. The game uses the XNA "GameStateManagement" sample (`ScreenManagerResources`).
- `Content/audio`, `Content/ScreenManagerResources/menuAudio`: XACT banks (`.xgs`/`.xsb`/`.xwb`).
- `Content/stages/001–100.xml`: plain XmlSerializer `StageData` files (wall polygons as `Vector2` arrays, ...). `stages/mapimages/` holds a preview `.xnb` for each stage.

## Tooling

- Shells: PowerShell (default) and Git Bash. Python 3.13 and .NET SDK 10 are installed.
- Convert XNB content with `python tools/xnb_extract.py` (defaults: `extracted/584E07D1/Content` → `assets/`). Format notes are in `docs/xnb.md`. Xbox GPU data (textures, vertex/index buffers) is big-endian.
- ILSpy is a repo-local dotnet tool (`dotnet-tools.json`, ilspycmd 11.1). Run `dotnet tool restore`, then regenerate `decomp/` with:
  `dotnet ilspycmd -p -o decomp/PhysCoop -r extracted/584E07D1 extracted/584E07D1/PhysCoop.exe` (same for `FarseerProject.dll`).
- Prefer writing small, reproducible Python scripts in `tools/` over one-off GUI tools, so every extraction step can be re-run.

## Planned layout

```
tools/        Python/C# helper scripts (STFS extractor, XNB unpacker, ...)
extracted/    Raw files pulled out of the STFS package (generated; reproducible from tools/)
decomp/       ILSpy output (C# project) — the main working area
assets/       Converted content: PNG textures/fonts, OBJ models, .fxo effects (generated, gitignored)
docs/         Notes on formats, game structure, findings
```

## Conventions

- The original packages, `extracted/` and `assets/` are gitignored (copyrighted, and regenerable).
- Never edit the original package files; all outputs must be regenerable from them via `tools/`.
- Record format discoveries (STFS quirks, XNB reader types, custom map formats) in `docs/`.
- Decompiled code: keep ILSpy's raw output committed first, then make cleanup/renaming changes as separate commits so diffs stay reviewable.

## Decompiled code (`decomp/`)

- `decomp/PhysCoop/`: the game, about 9k lines. Namespaces are `PhysCoop` (gameplay plus GameStateManagement screens), `PhysCoop.Oak` (`Draw2D`, from the developer's own "Oak" engine), and `BallGame_Editor` (`StageData`, the stage XML schema, apparently left over from an earlier "BallGame" project).
- `decomp/FarseerProject/`: Farseer Physics 2.0, about 11k lines.
- Known decompiler artifacts: 8 `goto IL_00bc` in `PhysCoop/PhysCoop/InputState.cs`. The generated csprojs target `net35` with broken `HintPath`s, so they won't build as-is.
