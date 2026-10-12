# Porting notes (XNA 3.0 Xbox → FNA)

The game was built with **XNA Game Studio 3.0** for Xbox 360 (`Microsoft.Xna.Framework.RuntimeProfile` = `Xbox 360.v3.0`, XNB format version 3). FNA implements the **XNA 4.0** API, so the port is a 3.0 → 4.0 API migration plus replacements for Xbox-only services.

## Setup

    git submodule update --init --recursive   # external/FNA (+ its own submodules)
    python tools/fetch_fnalibs.py             # native libs -> fnalibs/ (gitignored)
    python tools/build_content.py             # Xbox content -> build/Content (gitignored)
    dotnet build StorageInc.slnx

- `src/StorageInc` is the game. It started as a copy of `decomp/PhysCoop` and targets net8.0, referencing `external/FNA/FNA.Core.csproj`.
- `src/FarseerPhysics` is Farseer 2.0, a copy of `decomp/FarseerProject`. The assembly name is kept as `FarseerProject`.
- `fnalibs/x64/*.dll` is copied to the output directory by the csproj.
- **Never modify `external/FNA`.** It is upstream FNA, and FNA does not accept AI-generated contributions.

## Decompiler artifacts and how they were fixed

`decomp/` was produced without the XNA 3.0 assemblies, which aren't available.

- **Do not decompile with FNA standing in for XNA.** That was tried: ILSpy then maps numeric enum constants to FNA's *XNA 4.0* enum names, and many XNA enums were renumbered between 3.0 and 4.0. The output compiles but is wrong. For example, `(SurfaceFormat)1` (3.0 Color) became `SurfaceFormat.Bgr565`, and `(SurfaceFormat)22` (3.0 Single) became `Dxt5SrgbEXT`. Keeping the raw numbers makes every enum visible so it can be translated deliberately.
- `tools/fix_decomp.py <csproj>` fixes the mechanical issues from compiler diagnostics:
  - **CS1620**: `ref` → `out` for XNA out-parameters (132 sites in Farseer).
  - **CS0266**: missing `(int)` casts when `PlayerIndex` indexes an array, and missing casts from int to `Buttons`/`Keys` (81 sites in the game).
- **Base calls rendered as casts.** `((Game)this).Draw(gameTime)` would recurse forever. The IL confirms it was a non-virtual `call`, i.e. `base.Draw`. Two sites were fixed: `PhysCoop.Draw` and `ScreenManager.Initialize`. The compiler cannot catch these, so check any new `((BaseType)this).SameMethod(` pattern.
- ILSpy's `//IL_xxxx: Unknown result type…` comments were stripped from `src/` but kept in `decomp/`.

## XNA 3.0 enum values (D3D9 numbering)

These are the raw values found in the decompiled code and what they were translated to:

| Enum | 3.0 value → meaning | XNA 4 replacement |
|---|---|---|
| SurfaceFormat | 1 = Color, 22 = Single, 28 = Dxt1, 32 = Dxt5 | `SurfaceFormat.Color` / `.Single` |
| CullMode | 1 None, 2 CW, 3 CCW | `RasterizerState.CullCounterClockwise` |
| Blend | 1 Zero, 2 One, 5 SourceAlpha, 6 InverseSourceAlpha | `BlendState.Opaque` (One/Zero) |
| BlendFunction | 1 Add | |
| CompareFunction | 1 Never … 8 Always | (alpha test removed in XNA 4) |
| TextureAddressMode | 1 Wrap, 2 Mirror, 3 Clamp | `TextureAddressMode.Wrap` |
| TextureFilter | 1 Point, 2 Linear | `TextureFilter.Linear` |
| PrimitiveType | 4 TriangleList, 5 TriangleStrip | `PrimitiveType.TriangleList/Strip` |
| BufferUsage | 8 WriteOnly | `BufferUsage.WriteOnly` |
| ClearOptions | 1 Target, 2 DepthBuffer | same flags |
| SpriteBlendMode | 0 None, 1 AlphaBlend, 2 Additive | `BlendState.Opaque` / `.NonPremultiplied` / `.Additive` |

These enums have the same values in 3.0 and 4.0, so the remaining raw casts are safe: `PlayerIndex`, `Buttons`, `Keys`, `SpriteEffects`, `AudioStopOptions`.

## API changes made

- **Rendering:** XNA 4 has no `ResolveBackBuffer`. `ScreenManager` now renders each frame into `SceneTarget` (1280×720, PreserveContents), and `PostProcessImage` draws it to the back buffer with the brightness effect, letterboxed to the window. Every place that used to switch back to the back buffer with `SetRenderTarget(0, null)` now switches back to `SceneTarget`.
- **SpriteBatch:** XNA 3 alpha blending is **non-premultiplied**, while XNA 4's `BlendState.AlphaBlend` is premultiplied. Every `Begin()` therefore uses `BlendState.NonPremultiplied`, including calls that had no arguments.
- **Deferred renderer (`GameplayScreen`)**, structured like Riemer's XNA deferred shading tutorial:
  - MRT is set with `SetRenderTargets(color, normal, depth)`. The render targets now own their depth buffers (`colorTarget` and `shadowTarget` use Depth24).
  - `GetTexture()` is gone; the render target itself is the texture.
  - `Effect.Begin/End` and `Pass.Begin/End` became `Pass.Apply()`.
  - Vertex declarations come from the vertex type, and `Vertices[0].SetSource` became `SetVertexBuffer`.
- **FrameRateCounter:** `LoadGraphicsContent(bool)` became `LoadContent()`.
- **`src/StorageInc/Compat/GamerServices.cs`:** a stand-in for XNA GamerServices, which FNA does not provide.
  - Sign-in works like the Xbox, without the profile picker. Pressing Start signs in the main player. On Game Setup, X toggles sign-in for any other controller. On Xbox, X opened the sign-in Guide, where players also signed out.
  - The number of signed-in players speeds up the period clock (`TimeLine.Update`: `players * 0.54 + 0.4`), so a connected controller must not count as a player until it signs in.
  - During a map, pressing Start on a controller that isn't signed in joins it to the game, taking over that player's empty truck. On Xbox, players joined mid-map by signing in from the system Guide; the game already allows it (`allowJoinInGame`).
  - A sign-in survives a disconnect, so during a map an unplugged controller pauses the game as on Xbox. Game Setup signs out non-main players whose controller is gone, since they can no longer press X.
  - Always the full version: no trial mode, marketplace or Guide UI.
  - Gamer pictures are coloured placeholders.
- **`src/StorageInc/Compat/Storage.cs`:** XNA 3 `OpenContainer()` and `container.Path` on real directories, stored under `Documents/SavedGames/<title>/AllPlayers` (XDG data dir elsewhere). `TitleLocation` is the executable's directory.

## Content

FNA rejects XNB version 3. Its texture reader can byte-swap Xbox data, but its model and vertex-buffer readers can't, so the Xbox content can't be loaded as-is. `tools/build_content.py` instead re-encodes everything as **XNA 4.0 Windows XNBs** (version 5, platform `w`, uncompressed) into `build/Content`. The csproj copies that folder to `Content/` next to the executable, so the game's `Content.Load` calls work unchanged.

| Type | Conversion |
|---|---|
| Texture2D (157) | SurfaceFormat 3.0 → 4.0 (Color 1→0, Dxt1 28→4, Dxt3 30→5, Dxt5 32→6). Color pixels ARGB→RGBA, DXT 16-bit words byte-swapped. All mip levels kept. |
| SpriteFont (2) | Glyph sheet as above, tables copied. |
| Model (11) | The XNA 3.0 per-mesh buffers and declaration table become XNA 4.0 shared buffers, one vertex buffer per part (`VertexOffset` 0, declaration embedded), with big-endian vertex and index data swapped. `VertexElementUsage` is renumbered (3.0 TextureCoordinate 5 → 4.0 2). `VertexDeclarationReader` must be listed in the XNB header even though it's read "raw". |
| stages/*.xml | Copied as-is. |
| Effects (5) | Not converted. They were rewritten in HLSL from the decompiled Xbox microcode (`src/StorageInc/Shaders`). See `docs/shaders.md`. |
| XACT (6) | `.xgs`/`.xsb` converted to little-endian (`tools/xact_endian.py`; FAudio misreads big-endian floats). `.xwb` rebuilt as PC wave banks with 16-bit PCM decoded by ffmpeg (`tools/build_audio.py`). See `docs/audio.md`. |

`dotnet run --project tools/ContentCheck` loads every converted XNB through FNA. For models it reads the vertices back and checks their bounds and that normals are unit length. Current result: 170/170 load, and all normals are unit length.

`python tools/build_content.py` needs ffmpeg for the wave banks. It's found on PATH or at `C:\Program2\ffmpeg\bin`.

## Runtime status

The game runs on Windows (FNA, Vulkan). Checked so far:
- Start screen, main menu, Game Setup (profile slots, map preview), loading into level 1.
- The level renders: floor, walls, forklifts, gates, HUD, deferred lighting.
- Saves are written to and loaded from `Documents/SavedGames/Storage Inc`.

Port fixes found at runtime:
- **First gameplay frame drew with a zero view matrix.** `GameplayScreen` set `viewMatrix` only in `Update` while active, but `Draw` runs first. Inverting the zero matrix gave NaNs (a garbage frame on Xbox, an exception in FNA debug builds). It's now initialised in the constructor with the same camera.
- **Projection aspect ratio** uses the native 1280×720 instead of the back buffer, so resizing the window doesn't distort the scene.
- **Crash when goods arrived (Farseer).** `SelectiveSweepCollider.StubComparer` broke ties as `left.Begin ? -1 : 1` without looking at `right`, so two Begin stubs at the same coordinate each compared "less" than the other. .NET CF's sort tolerated that; .NET 8's `List.Sort` can detect it and throws `ArgumentException: … IComparer.Compare() method returns inconsistent results`. New goods spawn aligned with existing ones, which triggered it. The fix keeps the order (by value, Begin before End) but treats same-kind ties as equal. `Arbiter.CompareSeperation` was also made NaN-safe with `CompareTo`.
- **Washed-out, striped overlay after a few maps.** The original relied on new GPU resources being blank. The shadow map was never cleared (color or depth), and `blackImage`, the starting shading for the first light, was never filled. With FNA/Vulkan a new texture or render target holds whatever was in that memory before. Once earlier maps' buffers had been freed by the garbage collector, a new map inherited them, and their shadow depths and shading washed over the scene with stripes. The map's GPU resources were also never disposed (`UnloadContent` only unloaded content). Fixes:
  - the shadow map is cleared to far (1.0, so empty areas are lit) each pass;
  - `blackImage` is filled with zeros;
  - the render targets, `blackImage` and the wall vertex buffers are disposed when a map ends.

  Reproduced with eight map restarts in a row (3 of 8 corrupted). After the fix all 8 match a healthy frame.
- **Truck motor at full volume all the time.** FAudio misread the Xbox `.xgs` floats, so the `Volume` RPC never attenuated it. Fixed by converting the files to little-endian (see `docs/audio.md`).
- **No gameplay sounds at all (motor, goods, gates).** This started after fnalibs was re-fetched for the Linux/macOS builds. The FAudio in use then was 26.09, which misparses XACT 3.0 variation events; the version used before the re-fetch is unknown. Menu sounds and music still worked. Fixed by fetching fnalibs again (FAudio 26.10). `ContentCheck` now checks the FAudio version (see `docs/audio.md`).

How these were reproduced without a gamepad: a temporary local edit skipped the "gamepad disconnected" pause, then a script sent keys (held ~150 ms, since the game polls key state) to reach a level and let goods arrive.

Original quirks kept:
- On a brand-new save, the Game Setup map preview is blank until the next launch. `CreateNewGame` never calls `NewMapSelected`.
- Gameplay pauses when the main player's gamepad isn't connected. The menus also work with the keyboard (Enter/Space, Esc, arrow keys), but driving needs a gamepad.

## Still to do

1. **More play-testing.** Gamepad play has been tested on Windows: driving and handling, rendering, goods arriving over time, motor sound and saving all work. Still untested: 2–4 players, all 100 stages, and the full set of sounds.
2. **Run on Linux and macOS.** The builds are prepared (see below) but not yet run on either OS.

## Display options and resolution (port feature)

The original renders everything for a fixed 1280×720 screen. The port keeps that layout but renders the 3D scene at the window's resolution:

- **Scene target:** `ScreenManager.SceneTarget` is sized to the largest 16:9 area of the back buffer (`RenderWidth` × `RenderHeight`) and is recreated when the window size changes. Other aspect ratios are letterboxed.
- **G-buffer:** the screen-space targets (color, normal, depth, shading) follow that size (`GameplayScreen.EnsureGBuffer`), so geometry and lighting are rendered at full resolution. The shadow map is light-space and keeps its original 1280×720, which keeps the original shadow look.
- **2D:** all 2D stays in the original 1280×720 coordinate system.
  - Code that read `GraphicsDevice.Viewport` for layout, `TitleSafeArea` or `Viewport.Project` now uses `ScreenManager.LogicalViewport`, a fixed 1280×720 viewport.
  - Every sprite batch that draws into the scene applies `ScreenManager.SpriteScale`.
  - UI art is still 720p art, scaled.

  - Sprites that follow moving 3D objects must not be rounded to whole logical pixels. Above 720p that steps by several real pixels and they visibly shake against the smoothly moving object. The goods status badges (`Goods.DrawInfo2D`, originally an integer `Rectangle`) are now drawn at a float position snapped to the real scene-target pixel grid (`ScreenManager.SnapToRenderPixel`). That gives 1-pixel steps like the original had at 720p, and they stay crisp.

  Before this change, fullscreen at a non-720p size would have mis-laid the UI. Code running in `Update` saw the back buffer's viewport instead of the scene's.
- **Options:** **Display** (Windowed/Fullscreen), **Window Size** (1280×720 to 3840×2160) and **VSync** were added to the Game Options menu.
  - Defaults (Y) resets them too.
  - Alt+Enter toggles fullscreen, and the game's menu input ignores Enter while Alt is held. The window can also be resized freely.
  - Fullscreen is FNA's borderless fullscreen at the desktop resolution.
- **Storage:** the options are stored in `Display.cfg` next to the other settings, not in the original `GlobalSettings.cfg`. The original only loads that after "Press Start" and the storage check, but the window mode is needed before the window is created (`Port/DisplayOptions.cs`).
- **Checked:** at 1280×720, 1920×1080 windowed and 2560×1440 fullscreen, including toggling mid-level. The HUD markers stay aligned with the goods.

## Linux and macOS

- **Path separators:** the game's `\\` asset paths are fine. Every load goes through FNA's `TitleContainer`, which normalizes separators: `ContentManager`, external references, and the `AudioEngine`/`WaveBank`/`SoundBank` file names. The few direct `System.IO` uses build paths with `Path.Combine`: stage XML, the map preview `File.Exists`, and saves.
- **Case sensitivity:** Linux filesystems are case-sensitive, and FNA's case-insensitive fallback (`CASE_SENSITIVITY_HACK`) is off by default. `python tools/check_content_paths.py` resolves every name with exact case against the built `Content/` folder. That covers names in code, all 100 stages and map previews, and the texture references inside the models. All 293 match.
- **Saves:** `Documents/SavedGames/Storage Inc` on Windows, `~/Library/Application Support/Storage Inc` on macOS, and `$XDG_DATA_HOME` (default `~/.local/share`) `/Storage Inc` on Linux. These are the same roots FNA's own StorageDevice uses.
- **Native libraries:** `FnaLibs.targets` copies the right fnalibs folder (`x64`, `lib64`, `libaarch64` or `osx`). It uses `-r <rid>` when publishing, otherwise the OS and CPU doing the build. `FNA.dll.config` maps the library names per OS. The projects no longer force x64, so ARM64 Linux and Apple Silicon run natively.
- **Publishing:** `python tools/package_release.py` publishes self-contained Release builds for `win-x64`, `linux-x64`, `osx-arm64` and `osx-x64` (or the RIDs given as arguments) to `dist/<rid>/StorageInc/`. It checks each build for its native libraries and the 281 content files, then packs it as `dist/StorageInc-<rid>.zip` (Windows) or `.tar.gz` (Linux/macOS). The tarballs are written by Python so the executable keeps its +x bit when packed on Windows. `dist/` is gitignored, since the builds contain the game content.
  - If the folder is copied from Windows, run `chmod +x StorageInc` on the target, since Windows filesystems don't keep the executable bit.
  - macOS may quarantine unsigned binaries. Run `xattr -dr com.apple.quarantine <folder>` if it refuses to start.
- **Build tools:** `build_content.py` needs `ffmpeg` on PATH. `build_shaders.py` needs the Windows SDK's `fxc`, but the compiled `.fxb` files are committed, so other platforms don't need it.
