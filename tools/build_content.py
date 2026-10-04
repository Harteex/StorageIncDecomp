#!/usr/bin/env python3
"""Build the PC content folder for the FNA port from the original Xbox 360 content.

Usage:
    python tools/build_content.py [content_dir] [out_dir]

Defaults: extracted/584E07D1/Content -> build/Content. The output is copied next to the
game executable by src/StorageInc/StorageInc.csproj.

The Xbox files are XNA 3.0 XNBs (format version 3, platform 'x', big-endian GPU data),
which FNA cannot load. They are re-encoded as XNA 4.0 Windows XNBs (version 5, platform
'w', uncompressed, little-endian) so the game loads them through the stock ContentManager:

  Texture2D   format renumbered (3.0 -> 4.0), Color ARGB -> RGBA, DXT words byte-swapped
  SpriteFont  same as Texture2D for the glyph sheet; tables copied
  Model       per-mesh XNA 3.0 buffers + declaration table -> XNA 4.0 per-part shared
              vertex/index buffers with embedded declarations, data byte-swapped
  Effect      not converted (compiled Xbox shaders); rewritten in src/StorageInc/Shaders
  XACT        .xgs/.xsb copied (FACT reads Xbox banks), .xwb rebuilt as PCM (needs ffmpeg);
              see tools/build_audio.py
  stages/*.xml are copied as-is.
"""
import os
import shutil
import struct
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from build_audio import convert_audio_file  # noqa: E402
from xnb_extract import (ContentReader, SURFACE_COLOR, SURFACE_DXT1, SURFACE_DXT3,  # noqa: E402
                         SURFACE_DXT5, argb_to_rgba, read_xnb_body, swap16)

XNA4_GRAPHICS = "Microsoft.Xna.Framework.Graphics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553"
XNA4_FRAMEWORK = "Microsoft.Xna.Framework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553"


def reader(name, assembly):
    return f"Microsoft.Xna.Framework.Content.{name}, {assembly}"


def list_reader(element):
    return (f"Microsoft.Xna.Framework.Content.ListReader`1[[Microsoft.Xna.Framework.{element}, {XNA4_FRAMEWORK}]], "
            f"{XNA4_FRAMEWORK}")


R_TEXTURE = reader("Texture2DReader", XNA4_GRAPHICS)
R_FONT = reader("SpriteFontReader", XNA4_GRAPHICS)
R_MODEL = reader("ModelReader", XNA4_GRAPHICS)
R_VB = reader("VertexBufferReader", XNA4_GRAPHICS)
R_VERTEXDECL = reader("VertexDeclarationReader", XNA4_GRAPHICS)
R_IB = reader("IndexBufferReader", XNA4_GRAPHICS)
R_BASICEFFECT = reader("BasicEffectReader", XNA4_GRAPHICS)
R_STRING = reader("StringReader", XNA4_FRAMEWORK)
R_LIST_RECT = list_reader("Rectangle")
R_RECT = reader("RectangleReader", XNA4_FRAMEWORK)
R_LIST_CHAR = (f"Microsoft.Xna.Framework.Content.ListReader`1[[System.Char, mscorlib, Version=4.0.0.0, "
               f"Culture=neutral, PublicKeyToken=b77a5c561934e089]], {XNA4_FRAMEWORK}")
R_CHAR = reader("CharReader", XNA4_FRAMEWORK)
R_LIST_VEC3 = list_reader("Vector3")
R_VEC3 = reader("Vector3Reader", XNA4_FRAMEWORK)

# XNA 3.0 SurfaceFormat -> XNA 4.0 SurfaceFormat
SURFACE_4 = {SURFACE_COLOR: 0, SURFACE_DXT1: 4, SURFACE_DXT3: 5, SURFACE_DXT5: 6}
# XNA 3.0 VertexElementUsage (by name) -> XNA 4.0 value
USAGE_4 = {"Position": 0, "Color": 1, "TextureCoordinate": 2, "Normal": 3, "Binormal": 4, "Tangent": 5,
           "BlendIndices": 6, "BlendWeight": 7, "Depth": 8, "Fog": 9, "PointSize": 10, "Sample": 11,
           "TessellateFactor": 12}
# XNA 3.0 VertexElementFormat -> (XNA 4.0 value, size, byte-swap unit)
FORMAT_4 = {0: (0, 4, 4), 1: (1, 8, 4), 2: (2, 12, 4), 3: (3, 16, 4), 4: (4, 4, "color")}


class XnbWriter:
    """Builds an uncompressed XNA 4.0 Windows XNB."""

    def __init__(self):
        self.readers = []
        self.body = bytearray()

    def reader_id(self, name):
        if name not in self.readers:
            self.readers.append(name)
        return self.readers.index(name) + 1

    # primitives
    def u8(self, v):
        self.body += struct.pack("<B", v)

    def i32(self, v):
        self.body += struct.pack("<i", v)

    def u32(self, v):
        self.body += struct.pack("<I", v)

    def f32(self, *vs):
        self.body += struct.pack(f"<{len(vs)}f", *vs)

    def bool(self, v):
        self.u8(1 if v else 0)

    def bytes(self, b):
        self.body += b

    @staticmethod
    def enc7(v):
        out = bytearray()
        while True:
            b = v & 0x7F
            v >>= 7
            out.append(b | (0x80 if v else 0))
            if not v:
                return bytes(out)

    def v7(self, v):
        self.body += self.enc7(v)

    def string(self, s):
        data = s.encode("utf-8")
        self.v7(len(data))
        self.body += data

    def char(self, c):
        self.body += c.encode("utf-8")

    def type_id(self, reader_name):
        self.v7(self.reader_id(reader_name))

    def finish(self, shared_count=0):
        header = bytearray(self.enc7(len(self.readers)))
        for name in self.readers:
            data = name.encode("utf-8")
            header += self.enc7(len(data)) + data + struct.pack("<i", 0)
        header += self.enc7(shared_count)
        payload = bytes(header) + bytes(self.body)
        size = 10 + len(payload)
        return b"XNBw" + bytes([5, 0]) + struct.pack("<I", size) + payload


# --- converters -----------------------------------------------------------------------

def write_texture(w, tex):
    fmt = tex["format"]
    if fmt not in SURFACE_4:
        raise NotImplementedError(f"surface format {fmt}")
    w.type_id(R_TEXTURE)
    w.i32(SURFACE_4[fmt])
    w.u32(tex["width"])
    w.u32(tex["height"])
    w.u32(len(tex["mips"]))
    for mip in tex["mips"]:
        data = argb_to_rgba(mip) if fmt == SURFACE_COLOR else swap16(mip)
        w.u32(len(data))
        w.bytes(data)


def write_font(w, font):
    w.type_id(R_FONT)
    write_texture(w, font["texture"])
    for rects in (font["glyphs"], font["cropping"]):
        w.type_id(R_LIST_RECT)
        w.u32(len(rects))
        for r in rects:
            w.body += struct.pack("<4i", *r)
    w.reader_id(R_RECT)
    w.type_id(R_LIST_CHAR)
    w.u32(len(font["characters"]))
    for c in font["characters"]:
        w.char(c)
    w.reader_id(R_CHAR)
    w.i32(font["line_spacing"])
    w.f32(font["spacing"])
    w.type_id(R_LIST_VEC3)
    w.u32(len(font["kerning"]))
    for k in font["kerning"]:
        w.f32(*k)
    w.reader_id(R_VEC3)
    w.bool(font["default_char"] is not None)
    if font["default_char"] is not None:
        w.char(font["default_char"])


def convert_vertices(data, elements, stride):
    """Byte-swap big-endian Xbox vertex data element by element."""
    out = bytearray(data)
    for base in range(0, len(data) - stride + 1, stride):
        for e in elements:
            _, size, unit = FORMAT_4[e["format"]]
            o = base + e["offset"]
            if unit == "color":  # packed ARGB big-endian -> XNA 4 Color bytes R,G,B,A
                a, r, g, b = data[o:o + 4]
                out[o:o + 4] = bytes((r, g, b, a))
            else:
                for k in range(o, o + size, unit):
                    out[k:k + unit] = data[k:k + unit][::-1]
    return bytes(out)


def write_model(w, model, shared):
    bones = model["bones"]
    nb = len(bones)

    def bone_ref(i):
        v = 0 if i is None else i + 1
        if nb < 255:
            w.u8(v)
        else:
            w.u32(v)

    shared_out = []  # (kind, payload) in shared-resource order

    def add_shared(kind, payload):
        shared_out.append((kind, payload))
        return len(shared_out)  # 1-based

    effect_ids = {}
    for i, s in enumerate(shared):
        if s["type"] != "BasicEffect":
            raise NotImplementedError(f"model shared resource {s['type']}")
        effect_ids[i] = add_shared("effect", s)

    w.type_id(R_MODEL)
    w.u32(nb)
    for b in bones:
        w.type_id(R_STRING)
        w.string(b["name"])
        w.f32(*b["transform"])
    for b in bones:
        bone_ref(b["parent"])
        w.u32(len(b["children"]))
        for c in b["children"]:
            bone_ref(c)

    w.i32(len(model["meshes"]))
    for mesh in model["meshes"]:
        w.type_id(R_STRING)
        w.string(mesh["name"])
        bone_ref(mesh["parent_bone"])
        w.f32(*mesh["bounding_sphere"])
        w.v7(0)  # tag
        ib = mesh["index_buffer"]
        isize = 2 if ib["sixteen_bit"] else 4
        ib_data = swap16(ib["data"]) if isize == 2 else b"".join(
            ib["data"][i:i + 4][::-1] for i in range(0, len(ib["data"]), 4))
        ib_id = add_shared("ib", (ib["sixteen_bit"], ib_data))
        w.i32(len(mesh["parts"]))
        for part in mesh["parts"]:
            decl = model["vertex_declarations"][part["vertex_declaration"]]["elements"]
            stride = max(e["offset"] + FORMAT_4[e["format"]][1] for e in decl)
            start = part["stream_offset"] + part["base_vertex"] * stride
            raw = mesh["vertex_buffer"]["data"][start:start + part["num_vertices"] * stride]
            vb_id = add_shared("vb", (decl, stride, part["num_vertices"], convert_vertices(raw, decl, stride)))
            w.i32(0)  # VertexOffset: this part's vertices start its own buffer
            w.i32(part["num_vertices"])
            w.i32(part["start_index"])
            w.i32(part["primitive_count"])
            w.v7(0)  # tag
            w.v7(vb_id)
            w.v7(ib_id)
            w.v7(effect_ids[part["effect"]])
    bone_ref(model["root_bone"])
    w.v7(0)  # tag

    for kind, payload in shared_out:
        if kind == "effect":
            w.type_id(R_BASICEFFECT)
            w.string(payload["texture"].replace("\\", "/") if payload["texture"] else "")
            w.f32(*payload["diffuse"])
            w.f32(*payload["emissive"])
            w.f32(*payload["specular"])
            w.f32(payload["specular_power"], payload["alpha"])
            w.bool(payload["vertex_color_enabled"])
        elif kind == "vb":
            decl, stride, count, data = payload
            w.type_id(R_VB)
            # The declaration is read "raw", but its reader must still be listed in the header.
            w.reader_id(R_VERTEXDECL)
            w.i32(stride)
            w.i32(len(decl))
            for e in decl:
                w.i32(e["offset"])
                w.i32(FORMAT_4[e["format"]][0])
                w.i32(USAGE_4[e["usage"]])
                w.i32(e["usage_index"])
            w.u32(count)
            w.bytes(data)
        else:
            sixteen, data = payload
            w.type_id(R_IB)
            w.bool(sixteen)
            w.i32(len(data))
            w.bytes(data)
    return len(shared_out)


def convert_xnb(src, dst):
    _, body = read_xnb_body(src)
    r = ContentReader(body)
    obj, shared = r.read_root()
    kind = obj["type"]
    if kind == "Effect":
        return kind, False
    w = XnbWriter()
    shared_count = 0
    if kind == "Texture2D":
        write_texture(w, obj)
    elif kind == "SpriteFont":
        write_font(w, obj)
    elif kind == "Model":
        shared_count = write_model(w, obj, shared)
    else:
        raise NotImplementedError(kind)
    with open(dst, "wb") as f:
        f.write(w.finish(shared_count))
    return kind, True


def main():
    root = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
    content_dir = sys.argv[1] if len(sys.argv) > 1 else os.path.join(root, "extracted", "584E07D1", "Content")
    out_dir = sys.argv[2] if len(sys.argv) > 2 else os.path.join(root, "build", "Content")

    counts, skipped, failures = {}, [], 0
    for dirpath, _, files in os.walk(content_dir):
        for fn in sorted(files):
            src = os.path.join(dirpath, fn)
            rel = os.path.relpath(src, content_dir)
            dst = os.path.join(out_dir, rel)
            os.makedirs(os.path.dirname(dst), exist_ok=True)
            ext = os.path.splitext(fn)[1].lower()
            try:
                if ext == ".xnb":
                    kind, written = convert_xnb(src, dst)
                    if not written:
                        skipped.append(rel)
                        continue
                elif ext in (".xgs", ".xsb", ".xwb"):
                    print(f"  {rel}: {convert_audio_file(src, dst)}")
                    kind = "xact"
                elif ext == ".xml":
                    shutil.copyfile(src, dst)
                    kind = "xml"
                else:
                    skipped.append(rel)
                    continue
                counts[kind] = counts.get(kind, 0) + 1
            except Exception as e:
                failures += 1
                print(f"FAILED {rel}: {e!r}")
    print(", ".join(f"{v} {k}" for k, v in sorted(counts.items())) + f", {failures} failed")
    print(f"not converted ({len(skipped)}): " + ", ".join(skipped))
    return 1 if failures else 0


if __name__ == "__main__":
    sys.exit(main())
