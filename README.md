# Storage Inc for PC

A port of **Storage Inc** (Stolpskott Studios, 2009), a cooperative arcade puzzle game for 1–4 local players originally released as an Xbox 360 Indie Game, to Windows, Linux and macOS using [FNA](https://fna-xna.github.io/).

> Work together as goods keep dropping in. Organize stored goods to save both time and well-needed space. Plan ahead, and make sure ordered deliveries are out in time.

All 100 maps are playable, with full co-op support.

## Permission and non-commercial condition

This port is released **with the permission of the original author**, on one condition: **nobody may make money from it in any way.** That means no selling it, no paid downloads, no ads, and no donations or paid tiers tied to it. The same applies to anyone who redistributes or modifies it.

If you enjoy Storage Inc, please support the original developer by checking out their more recent game, **Sharpshooter**, on Steam.

## Playing

Download the archive for your system, unpack it, and run `StorageInc` (`StorageInc.exe` on Windows). No .NET installation is needed.

- **Windows:** tested.
- **Linux and macOS:** builds are provided but not yet tested. On macOS the build is unsigned; if it won't start, run `xattr -dr com.apple.quarantine StorageInc` on the unpacked folder.

Controls:

- **Gamepad required for driving.** Menus also work with the keyboard (Enter/Space, Esc, arrow keys).
- **Joining:** press Start to begin as the main player. On the Game Setup screen, other players press **X** to join or leave. During a map, pressing **Start** on an unused controller joins the game. More players make the game faster, as in the original.
- **Display:** fullscreen, window size and VSync are under Game Options. **Alt+Enter** toggles fullscreen.

Saves are stored in `Documents/SavedGames/Storage Inc` (Windows), `~/Library/Application Support/Storage Inc` (macOS) or `~/.local/share/Storage Inc` (Linux).

## Building from source

This repository contains **no game content**. Building requires your own copy of the original Xbox 360 package. The tools extract it, decompile the game, and convert its content for PC.

Requirements: Python 3, the .NET SDK (8 or newer), ffmpeg, and Git.

```
git submodule update --init --recursive
python tools/stfs_extract.py "<original package>" extracted
python tools/fetch_fnalibs.py
python tools/build_content.py
dotnet build StorageInc.slnx
```

`python tools/package_release.py` builds the release archives into `dist/`. `CLAUDE.md` and `docs/` describe the formats, tools and porting work in detail.

## How the port works

- `decomp/` is the game decompiled with ILSpy, kept unmodified as a reference. `src/` is the ported game.
- The Xbox shaders were decompiled from their microcode and rewritten in HLSL (`docs/shaders.md`).
- Textures, models and fonts are converted from Xbox XNA 3.0 to PC XNA 4.0 format, and the audio from XMA to PCM (`docs/xnb.md`, `docs/audio.md`).
- Xbox Live features (profiles, sign-in, storage selection) are replaced with simple local equivalents (`docs/porting.md`).

## Licenses and credits

- **Storage Inc** by Stolpskott Studios. The game's code (`decomp/`, and the game code in `src/StorageInc`) and its content belong to the original author and are used with permission under the non-commercial condition above. They are **not** covered by the license below.
- **Port code and tools** (`tools/`, the compatibility code in `src/StorageInc/Compat` and `src/StorageInc/Port`, the rewritten shaders): Microsoft Public License (Ms-PL), see [LICENSE](LICENSE).
- **[FNA](https://github.com/FNA-XNA/FNA)** by Ethan Lee and contributors: Microsoft Public License (Ms-PL).
- **Farseer Physics Engine 2.0** (`src/FarseerPhysics`, `decomp/FarseerProject`): Microsoft Permissive License (Ms-PL), the license's original name; it was later renamed the Microsoft Public License. See [src/FarseerPhysics/LICENSE.txt](src/FarseerPhysics/LICENSE.txt).
- Native libraries from [fnalibs](https://github.com/FNA-XNA/fnalibs-dailies): SDL3 (zlib), FNA3D and FAudio (zlib), Theorafile (zlib/BSD).
