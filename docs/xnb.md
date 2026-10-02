# XNB content notes (Xbox 360, XNA 3.x)

Implemented in `tools/lzx.py` and `tools/xnb_extract.py`. Run:

    python tools/xnb_extract.py extracted/584E07D1/Content assets [--raw]

This takes about 10 s and converts all 175 files: 157 Texture2D, 11 Model, 2 SpriteFont and 5 Effect.

## Container

- Header: `XNB` + platform `x` + version `3` + flags (`0x80` = LZX compressed) + u32 LE file size. Compressed files then have a u32 LE decompressed size.
- LZX: 64 KB window, no E8 translation. The body is split into chunks with a 2-byte BE block size, or `FF` + 2-byte frame size + 2-byte block size. The frame size defaults to 0x8000. Each chunk starts a fresh bitstream, but the decoder state carries over between chunks.
- 4 tiny textures are stored uncompressed: `blankTexture`, `fadeTexture`, `hud_Period_Meter`, `RedLight_0`.

## Endianness

The ContentReader stream is **little-endian**. GPU payloads are **big-endian**:

| Data | Xbox layout | PC equivalent |
|---|---|---|
| `SurfaceFormat.Color` (1) | bytes A,R,G,B | B,G,R,A |
| `Dxt1` (28) / `Dxt3` (30) / `Dxt5` (32) | 16-bit words byte-swapped | LE words |
| Vertex buffers | BE floats | LE |
| Index buffers | BE u16 | LE |

Textures are stored linear (not tiled) in the XNB.

## Type readers used

- `Texture2DReader`: i32 format, u32 w, h, levels, then per level u32 size + data.
- `SpriteFontReader`: Texture2D, List<Rectangle> glyphs, List<Rectangle> cropping, List<char> chars, i32 line spacing, f32 spacing, List<Vector3> kerning, bool + char default. Value-type list elements have no type id.
  - `FrameRateFont`: DXT3, digits only.
  - `OakEngineFont64`: Color, the Xbox controller button icons plus text.
- `ModelReader` (XNA 3.1 layout):
  - bones (name object + Matrix), then hierarchy (parent ref, child refs). A bone ref is a u8 if there are 255 or fewer bones, else u32, and 0 means null.
  - **vertex declaration table** (u32 count + objects).
  - meshes: name, parent bone, bounding sphere, VertexBuffer, IndexBuffer, tag, then parts.
  - each part: 5x i32 (stream offset, base vertex, num vertices, start index, primitive count), **i32 vertex declaration index**, tag, effect as a 7-bit shared resource ref.
  - All models use `BasicEffect`, with the texture as an external reference such as `..\textures\Floor_0`.
- `EffectReader`: u32 size + compiled Xbox 360 effect. Dumped as `.fxo`: `Deferred1Scene`, `Deferred2Lights`, `Deferred3Final`, `ShadowMap`, `Brightness`. Porting them will need HLSL rewrites, because Xbox shader bytecode can't be used on PC.

## Output conventions

- PNG = mip level 0, RGBA.
- OBJ vertices are pre-transformed by the absolute bone transform, and faces are rewound from XNA's clockwise order to CCW. V is flipped (`1 - v`).
- The `.json` next to each model or font keeps the raw structure (bones, parts, vertex declarations, effect params, glyph tables).
