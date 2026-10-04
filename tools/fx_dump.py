#!/usr/bin/env python3
"""Dump the structure of a compiled D3DX effect (fx_2_0 container), incl. Xbox 360 ones.

Usage:
    python tools/fx_dump.py <effect.fxo> [...]

Prints parameters (type, class, size, semantic, default value), samplers with their
states, and techniques/passes with their render states, which is everything needed
to rewrite an effect in HLSL except the shader code itself. Xbox 360 effects are the
same layout as PC ones but big-endian; the endianness is detected from the magic.
Shader blobs (Xenos microcode on Xbox) are only reported by size.

Xbox 360 differences (observed in this game's effects, see docs/shaders.md):
  - the state table lacks 54 fixed-function entries, so Xbox state ids are PC ids - 54
    (verified for Vertexshader/Pixelshader, Texture and the filter/address sampler states);
  - sampler state values use Xenos numbering: filter POINT=0 LINEAR=1, address WRAP=0
    MIRROR=1 CLAMP=2.

Layout follows Wine's d3dx9_36/effect.c (d3dx_parse_effect and friends); the state
names are its state_table, i.e. the D3D9 render/sampler state enums in table order.
"""
import struct
import sys

STATE_NAMES = [
    "D3DRS_ZENABLE", "D3DRS_FILLMODE", "D3DRS_SHADEMODE", "D3DRS_ZWRITEENABLE",
    "D3DRS_ALPHATESTENABLE", "D3DRS_LASTPIXEL", "D3DRS_SRCBLEND", "D3DRS_DESTBLEND",
    "D3DRS_CULLMODE", "D3DRS_ZFUNC", "D3DRS_ALPHAREF", "D3DRS_ALPHAFUNC", "D3DRS_DITHERENABLE",
    "D3DRS_ALPHABLENDENABLE", "D3DRS_FOGENABLE", "D3DRS_SPECULARENABLE", "D3DRS_FOGCOLOR",
    "D3DRS_FOGTABLEMODE", "D3DRS_FOGSTART", "D3DRS_FOGEND", "D3DRS_FOGDENSITY",
    "D3DRS_RANGEFOGENABLE", "D3DRS_STENCILENABLE", "D3DRS_STENCILFAIL", "D3DRS_STENCILZFAIL",
    "D3DRS_STENCILPASS", "D3DRS_STENCILFUNC", "D3DRS_STENCILREF", "D3DRS_STENCILMASK",
    "D3DRS_STENCILWRITEMASK", "D3DRS_TEXTUREFACTOR", "D3DRS_WRAP0", "D3DRS_WRAP1", "D3DRS_WRAP2",
    "D3DRS_WRAP3", "D3DRS_WRAP4", "D3DRS_WRAP5", "D3DRS_WRAP6", "D3DRS_WRAP7", "D3DRS_WRAP8",
    "D3DRS_WRAP9", "D3DRS_WRAP10", "D3DRS_WRAP11", "D3DRS_WRAP12", "D3DRS_WRAP13", "D3DRS_WRAP14",
    "D3DRS_WRAP15", "D3DRS_CLIPPING", "D3DRS_LIGHTING", "D3DRS_AMBIENT", "D3DRS_FOGVERTEXMODE",
    "D3DRS_COLORVERTEX", "D3DRS_LOCALVIEWER", "D3DRS_NORMALIZENORMALS",
    "D3DRS_DIFFUSEMATERIALSOURCE", "D3DRS_SPECULARMATERIALSOURCE", "D3DRS_AMBIENTMATERIALSOURCE",
    "D3DRS_EMISSIVEMATERIALSOURCE", "D3DRS_VERTEXBLEND", "D3DRS_CLIPPLANEENABLE", "D3DRS_POINTSIZE",
    "D3DRS_POINTSIZE_MIN", "D3DRS_POINTSIZE_MAX", "D3DRS_POINTSPRITEENABLE",
    "D3DRS_POINTSCALEENABLE", "D3DRS_POINTSCALE_A", "D3DRS_POINTSCALE_B", "D3DRS_POINTSCALE_C",
    "D3DRS_MULTISAMPLEANTIALIAS", "D3DRS_MULTISAMPLEMASK", "D3DRS_PATCHEDGESTYLE",
    "D3DRS_DEBUGMONITORTOKEN", "D3DRS_INDEXEDVERTEXBLENDENABLE", "D3DRS_COLORWRITEENABLE",
    "D3DRS_TWEENFACTOR", "D3DRS_BLENDOP", "D3DRS_POSITIONDEGREE", "D3DRS_NORMALDEGREE",
    "D3DRS_SCISSORTESTENABLE", "D3DRS_SLOPESCALEDEPTHBIAS", "D3DRS_ANTIALIASEDLINEENABLE",
    "D3DRS_MINTESSELLATIONLEVEL", "D3DRS_MAXTESSELLATIONLEVEL", "D3DRS_ADAPTIVETESS_X",
    "D3DRS_ADAPTIVETESS_Y", "D3DRS_ADAPTIVETESS_Z", "D3DRS_ADAPTIVETESS_W",
    "D3DRS_ENABLEADAPTIVETESSELLATION", "D3DRS_TWOSIDEDSTENCILMODE", "D3DRS_CCW_STENCILFAIL",
    "D3DRS_CCW_STENCILZFAIL", "D3DRS_CCW_STENCILPASS", "D3DRS_CCW_STENCILFUNC",
    "D3DRS_COLORWRITEENABLE1", "D3DRS_COLORWRITEENABLE2", "D3DRS_COLORWRITEENABLE3",
    "D3DRS_BLENDFACTOR", "D3DRS_SRGBWRITEENABLE", "D3DRS_DEPTHBIAS",
    "D3DRS_SEPARATEALPHABLENDENABLE", "D3DRS_SRCBLENDALPHA", "D3DRS_DESTBLENDALPHA",
    "D3DRS_BLENDOPALPHA", "D3DTSS_COLOROP", "D3DTSS_COLORARG0", "D3DTSS_COLORARG1",
    "D3DTSS_COLORARG2", "D3DTSS_ALPHAOP", "D3DTSS_ALPHAARG0", "D3DTSS_ALPHAARG1",
    "D3DTSS_ALPHAARG2", "D3DTSS_RESULTARG", "D3DTSS_BUMPENVMAT00", "D3DTSS_BUMPENVMAT01",
    "D3DTSS_BUMPENVMAT10", "D3DTSS_BUMPENVMAT11", "D3DTSS_TEXCOORDINDEX", "D3DTSS_BUMPENVLSCALE",
    "D3DTSS_BUMPENVLOFFSET", "D3DTSS_TEXTURETRANSFORMFLAGS", "D3DTSS_CONSTANT", "NPatchMode", "FVF",
    "D3DTS_PROJECTION", "D3DTS_VIEW", "D3DTS_WORLD", "D3DTS_TEXTURE0", "MaterialDiffuse",
    "MaterialAmbient", "MaterialSpecular", "MaterialEmissive", "MaterialPower", "LightType",
    "LightDiffuse", "LightSpecular", "LightAmbient", "LightPosition", "LightDirection",
    "LightRange", "LightFallOff", "LightAttenuation0", "LightAttenuation1", "LightAttenuation2",
    "LightTheta", "LightPhi", "LightEnable", "Vertexshader", "Pixelshader", "VertexShaderConstantF",
    "VertexShaderConstantB", "VertexShaderConstantI", "VertexShaderConstant",
    "VertexShaderConstant1", "VertexShaderConstant2", "VertexShaderConstant3",
    "VertexShaderConstant4", "PixelShaderConstantF", "PixelShaderConstantB", "PixelShaderConstantI",
    "PixelShaderConstant", "PixelShaderConstant1", "PixelShaderConstant2", "PixelShaderConstant3",
    "PixelShaderConstant4", "Texture", "AddressU", "AddressV", "AddressW", "BorderColor",
    "MagFilter", "MinFilter", "MipFilter", "MipMapLodBias", "MaxMipLevel", "MaxAnisotropy",
    "SRGBTexture", "ElementIndex", "DMAPOffset", "Sampler",
]

TYPES = ["void", "bool", "int", "float", "string", "texture", "texture1D", "texture2D", "texture3D",
         "textureCUBE", "sampler", "sampler1D", "sampler2D", "sampler3D", "samplerCUBE",
         "pixelshader", "vertexshader"]
CLASSES = ["scalar", "vector", "matrix_rows", "matrix_columns", "object", "struct"]
OBJECT_CLASS = 4

# Friendly names for common D3D9 enum values.
ENUM_VALUES = {
    "AddressU": {1: "WRAP", 2: "MIRROR", 3: "CLAMP", 4: "BORDER", 5: "MIRRORONCE"},
    "MagFilter": {0: "NONE", 1: "POINT", 2: "LINEAR", 3: "ANISOTROPIC"},
    "D3DRS_CULLMODE": {1: "NONE", 2: "CW", 3: "CCW"},
    "D3DRS_SRCBLEND": {1: "ZERO", 2: "ONE", 3: "SRCCOLOR", 4: "INVSRCCOLOR", 5: "SRCALPHA",
                       6: "INVSRCALPHA", 7: "DESTALPHA", 8: "INVDESTALPHA", 9: "DESTCOLOR",
                       10: "INVDESTCOLOR"},
    "D3DRS_ZFUNC": {1: "NEVER", 2: "LESS", 3: "EQUAL", 4: "LESSEQUAL", 5: "GREATER", 6: "NOTEQUAL",
                    7: "GREATEREQUAL", 8: "ALWAYS"},
    "D3DRS_BLENDOP": {1: "ADD", 2: "SUBTRACT", 3: "REVSUBTRACT", 4: "MIN", 5: "MAX"},
}
XBOX_ENUM_VALUES = {
    "AddressU": {0: "WRAP", 1: "MIRROR", 2: "CLAMP", 3: "MIRRORONCE"},
    "MagFilter": {0: "POINT", 1: "LINEAR", 2: "NONE", 4: "ANISOTROPIC"},
}
XBOX_STATE_OFFSET = 54
for alias, base in [("AddressV", "AddressU"), ("AddressW", "AddressU"), ("MinFilter", "MagFilter"),
                    ("MipFilter", "MagFilter"), ("D3DRS_DESTBLEND", "D3DRS_SRCBLEND"),
                    ("D3DRS_ALPHAFUNC", "D3DRS_ZFUNC")]:
    ENUM_VALUES[alias] = ENUM_VALUES[base]
    if base in XBOX_ENUM_VALUES:
        XBOX_ENUM_VALUES[alias] = XBOX_ENUM_VALUES[base]


class Effect:
    def __init__(self, raw):
        magic = raw[:4]
        if magic == b"\xfe\xff\x09\x01":
            self.e = ">"
            self.xbox = True
        elif magic == b"\x01\x09\xff\xfe":
            self.e = "<"
            self.xbox = False
        else:
            raise ValueError(f"not an fx_2_0 effect: {magic.hex()}")
        start = struct.unpack_from(self.e + "I", raw, 4)[0]
        self.data = raw[8:]
        self.p = start
        self.object_params = {}
        self.shader_objects = {}
        self.blobs = {}

    def u32(self):
        v = struct.unpack_from(self.e + "I", self.data, self.p)[0]
        self.p += 4
        return v

    def u32_at(self, off):
        return struct.unpack_from(self.e + "I", self.data, off)[0]

    def name_at(self, off):
        size = self.u32_at(off)
        return self.data[off + 4:off + 4 + size].split(b"\0")[0].decode("ascii", "replace") if size else ""

    def typedef(self, off):
        t = {}
        t["type"], t["class"] = self.u32_at(off), self.u32_at(off + 4)
        t["name"], t["semantic"] = self.name_at(self.u32_at(off + 8)), self.name_at(self.u32_at(off + 12))
        t["elements"] = self.u32_at(off + 16)
        t["rows"] = t["columns"] = 0
        if t["class"] == 1:
            t["columns"], t["rows"] = self.u32_at(off + 20), self.u32_at(off + 24)
        elif t["class"] in (0, 2, 3):
            t["rows"], t["columns"] = self.u32_at(off + 20), self.u32_at(off + 24)
        elif t["class"] == 5:
            raise NotImplementedError("struct parameters")
        return t

    def type_str(self, t):
        name = TYPES[t["type"]] if t["type"] < len(TYPES) else f"type{t['type']}"
        if t["class"] == 1:
            name += str(t["columns"])
        elif t["class"] in (2, 3):
            name += f"{t['rows']}x{t['columns']}"
        if t["elements"]:
            name += f"[{t['elements']}]"
        return name

    def value_str(self, t, off):
        count = max(1, t["elements"])
        if t["class"] == OBJECT_CLASS:
            if t["type"] in range(10, 15):
                return None  # samplers are handled separately
            ids = [self.u32_at(off + 4 * i) for i in range(count)]
            return "object " + ",".join(map(str, ids))
        n = max(1, t["rows"]) * max(1, t["columns"]) * count
        raw = [self.data[off + 4 * i:off + 4 * i + 4] for i in range(n)]
        if t["type"] == 3:
            vals = [struct.unpack(self.e + "f", b)[0] for b in raw]
            return ", ".join(f"{v:g}" for v in vals)
        vals = [struct.unpack(self.e + "I", b)[0] for b in raw]
        return ", ".join(map(str, vals))

    def state(self, indent):
        op, index, toff, voff = self.u32(), self.u32(), self.u32(), self.u32()
        if self.xbox:
            op += XBOX_STATE_OFFSET
        name = STATE_NAMES[op] if op < len(STATE_NAMES) else f"state#{op}"
        t = self.typedef(toff)
        if name in ("Texture", "Vertexshader", "Pixelshader") and t["class"] == OBJECT_CLASS:
            obj = self.u32_at(voff)
            target = self.object_params.get(obj, f"object {obj}")
            if name != "Texture":
                self.shader_objects[obj] = name
            return f"{indent}{name}{f'[{index}]' if index else ''} = {target}"
        val = self.value_str(t, voff)
        enums = XBOX_ENUM_VALUES if self.xbox and name in XBOX_ENUM_VALUES else ENUM_VALUES
        if t["type"] in (1, 2) and t["class"] == 0 and name in enums:
            v = self.u32_at(voff)
            val = f"{enums[name].get(v, v)} ({v})"
        return f"{indent}{name}{f'[{index}]' if index else ''} = {val}"

    def sampler_states(self, voff, indent):
        save = self.p
        self.p = voff
        count = self.u32()
        lines = [self.state(indent) for _ in range(count)]
        self.p = save
        return lines

    def annotations(self, count, indent):
        out = []
        for _ in range(count):
            toff, voff = self.u32(), self.u32()
            t = self.typedef(toff)
            out.append(f"{indent}<{self.type_str(t)} {t['name']} = {self.value_str(t, voff)}>")
        return out

    def dump(self):
        out = []
        nparams, ntech, nshaders, nobjects = self.u32(), self.u32(), self.u32(), self.u32()
        out.append(f"{nparams} parameters, {ntech} techniques, {nobjects} objects")
        params = []
        for _ in range(nparams):
            toff, voff, flags, nanno = self.u32(), self.u32(), self.u32(), self.u32()
            t = self.typedef(toff)
            params.append((t, voff, flags, self.annotations(nanno, "      ")))
            if t["class"] == OBJECT_CLASS and t["type"] not in range(10, 15):
                for i in range(max(1, t["elements"])):
                    self.object_params[self.u32_at(voff + 4 * i)] = t["name"]
        for t, voff, flags, annos in params:
            sem = f" : {t['semantic']}" if t["semantic"] else ""
            line = f"  {self.type_str(t)} {t['name']}{sem}"
            if t["type"] in range(10, 15):
                out.append(line + " = sampler_state {")
                out += self.sampler_states(voff, "      ")
                out.append("  }")
                continue
            val = self.value_str(t, voff)
            if t["class"] != OBJECT_CLASS and val and any(c not in "0., " for c in val):
                line += f" = {{{val}}}"
            out.append(line + ";" + (f"  // flags {flags:#x}" if flags else ""))
            out += annos
        for _ in range(ntech):
            name = self.name_at(self.u32())
            nanno, npass = self.u32(), self.u32()
            out.append(f"technique {name}")
            out += self.annotations(nanno, "  ")
            for _ in range(npass):
                pname = self.name_at(self.u32())
                pnanno, nstates = self.u32(), self.u32()
                out.append(f"  pass {pname}")
                out += self.annotations(pnanno, "    ")
                out += [self.state("    ") for _ in range(nstates)]
        nstrings, nresources = self.u32(), self.u32()
        for _ in range(nstrings):
            obj, size = self.u32(), self.u32()
            self.blobs[obj] = self.data[self.p:self.p + size]
            self.p += (size + 3) & ~3
        for obj, kind in sorted(self.shader_objects.items()):
            out.append(f"{kind} (object {obj}): {len(self.blobs.get(obj, b''))} bytes")
        return "\n".join(out)


def main():
    for path in sys.argv[1:]:
        print(f"=== {path}")
        print(Effect(open(path, "rb").read()).dump())


if __name__ == "__main__":
    main()
