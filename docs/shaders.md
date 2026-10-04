# Shaders

The 5 original effects are compiled Xbox 360 effects. The container is D3DX `fx_2_0`, stored big-endian, with Xenos GPU microcode inside, so they can't run on PC. They were rewritten in HLSL (`src/StorageInc/Shaders/*.fx`) from the decompiled microcode, which makes them reconstructions of the original math rather than approximations.

    python tools/build_shaders.py      # fxc (Windows SDK) -> Shaders/*.fxb, committed

The csproj copies the `.fxb` files into `Content/`. FNA's ContentManager loads a raw `.fxb` when there's no `.xnb` with that name.

| Effect | Used by | What it does |
|---|---|---|
| `Brightness` | ScreenManager.PostProcessImage (SpriteBatch) | `rgb * Intensity`, alpha untouched. PS only (ps_2_0). |
| `Deferred1Scene` (`MultipleTargets`) | GameplayScreen G-buffer pass | MRT: albedo (a=0), normal*0.5+0.5 (a=0), depth z/w. |
| `ShadowMap` | Spot-light shadow pass | depth z/w from the light's view. |
| `Deferred2Lights` (`DeferredSpotLight`) | One full-screen pass per light | Reconstructs world position from depth; shadow test with 0.01 bias; `dot(n, -L) * strength * pow(coneDot, decay)` inside the cone, not saturated; added to the previous shading. |
| `Deferred3Final` (`CombineColorAndShading`) | Full-screen combine | `albedo * (xAmbient + shading.r)` |

This is the same structure (and parameter naming) as Riemer Grootjans' XNA deferred shading tutorial.

## How the originals were analysed

1. **Effect structure:** `tools/fx_dump.py`. It parses the D3DX effect binary, following Wine's `d3dx9_36/effect.c`, and prints parameter types, samplers with their states, and techniques/passes. Xbox quirks found:
   - State ids are the PC ids **minus 54**. The Xbox table lacks the fixed-function states.
   - Sampler state values use **Xenos numbering**: filter POINT=0, LINEAR=1; address WRAP=0, MIRROR=1, CLAMP=2. These are each one less than the D3D9 values.

   Running the dumper on the compiled `.fxb` gives the same interface as the originals: names, types, techniques, and sampler states up to that renumbering.
2. **Shader code:**
   - Each effect embeds Xenos shader containers (magic `102A11xx`; header size and code size follow the magic).
   - [XenosRecomp](https://github.com/hedge-dev/XenosRecomp) (MIT) converts a container to register-level HLSL. It only builds with Clang; for MSVC, map `__builtin_bswap16/32/64` to `_byteswap_ushort/_ulong/_uint64` in `pch.h`. Run it as `XenosRecomp <container.bin> <out.hlsl> XenosRecomp/shader_common.h`.
   - The Brightness pixel shader was also decoded by hand from the ALU word (MULv `r0 * c0.xxxx` → oC0.xyz, MAXs `r0.w` → oC0.w). This agrees with XenosRecomp.
   - `g_HalfPixelOffset` in XenosRecomp's output is its own addition, not part of the originals.
   - Literal constants recovered: `(2, -1)` for UV → clip space, `0.5`, and the shadow bias `-0.01`.

## Notes

- The deferred effects use vs_3_0/ps_3_0 like the originals. Brightness is ps_2_0 because it pairs with SpriteBatch's own vertex shader.
- The full-screen passes don't apply a half-pixel offset, matching the originals. FNA emulates XNA/D3D9 pixel-centre rules.
