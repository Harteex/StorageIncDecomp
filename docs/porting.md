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
  - Player One is always signed in. Players 2–4 are signed in while their gamepad is connected, and joining a game this way works as it did on Xbox.
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
| Effects (5), XACT (6) | Not converted. See below. |

`dotnet run --project tools/ContentCheck` loads every converted XNB through FNA. For models it reads the vertices back and checks their bounds and that normals are unit length. Current result: 170/170 load, and all normals are unit length.

## Still to do

1. **Effects.** `Deferred1Scene`, `Deferred2Lights`, `Deferred3Final`, `ShadowMap` and `Brightness` must be rewritten in HLSL and compiled for FNA (fx_2_0). The parameter and technique names are known from the code.
2. **Audio.** The XACT banks are Xbox (big-endian, XMA). FNA's XACT needs PC banks, so they must be rebuilt as PCM or ADPCM.
3. **Paths.** Content paths use `\\`, and the stage XML is read from `Content/stages`. These need checking on Linux and macOS.
