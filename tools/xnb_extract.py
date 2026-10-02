#!/usr/bin/env python3
"""Convert Xbox 360 XNA 3.x .xnb content into editable files.

Usage:
    python tools/xnb_extract.py [content_dir] [out_dir] [--raw]

  Texture2D  -> .png (mip level 0)
  SpriteFont -> .png (glyph sheet) + .json (glyph rects, cropping, kerning, ...)
  Model      -> .obj + .mtl (+ .json with bones / mesh parts / vertex declarations)
  Effect     -> .fxo (raw compiled Xbox 360 effect bytecode)
  --raw also writes the decompressed XNB body as .xnb.bin.

Xbox specifics (see docs/xnb.md): the ContentReader stream itself is little-endian, but
GPU-bound data is big-endian: Color pixels are A,R,G,B bytes; DXT blocks are made of
byte-swapped 16-bit words; vertex and index buffers are big-endian.
"""
import argparse
import json
import os
import struct
import sys
import zlib

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from lzx import decompress_xnb_payload  # noqa: E402

SURFACE_COLOR, SURFACE_DXT1, SURFACE_DXT3, SURFACE_DXT5 = 1, 28, 30, 32

# XNA 3.x VertexElementFormat -> (struct format, component count)
VERTEX_FORMATS = {0: (">f", 1), 1: (">2f", 2), 2: (">3f", 3), 3: (">4f", 4), 4: (">4B", 4)}
VERTEX_FORMAT_SIZES = {0: 4, 1: 8, 2: 12, 3: 16, 4: 4, 5: 4, 6: 4, 7: 8, 8: 4, 9: 4, 10: 8,
                       11: 4, 12: 8, 13: 4, 14: 4, 15: 4, 16: 8}
VERTEX_USAGES = {0: "Position", 1: "BlendWeight", 2: "BlendIndices", 3: "Normal", 4: "PointSize",
                 5: "TextureCoordinate", 6: "Tangent", 7: "Binormal", 8: "TessellateFactor",
                 10: "Color", 11: "Fog", 12: "Depth", 13: "Sample"}


# --- PNG / pixel helpers --------------------------------------------------------------

def write_png(path, width, height, rgba):
    raw = b"".join(b"\0" + rgba[y * width * 4:(y + 1) * width * 4] for y in range(height))

    def chunk(tag, data):
        return struct.pack(">I", len(data)) + tag + data + struct.pack(">I", zlib.crc32(tag + data))

    with open(path, "wb") as f:
        f.write(b"\x89PNG\r\n\x1a\n")
        f.write(chunk(b"IHDR", struct.pack(">IIBBBBB", width, height, 8, 6, 0, 0, 0)))
        f.write(chunk(b"IDAT", zlib.compress(raw, 9)))
        f.write(chunk(b"IEND", b""))


def argb_to_rgba(data):
    out = bytearray(len(data))
    out[0::4] = data[1::4]
    out[1::4] = data[2::4]
    out[2::4] = data[3::4]
    out[3::4] = data[0::4]
    return bytes(out)


def swap16(data):
    out = bytearray(len(data))
    out[0::2] = data[1::2]
    out[1::2] = data[0::2]
    return bytes(out)


def _rgb565(c):
    r, g, b = (c >> 11) & 31, (c >> 5) & 63, c & 31
    return (r << 3) | (r >> 2), (g << 2) | (g >> 4), (b << 3) | (b >> 2)


def _color_block(block, out, x0, y0, width, height, dxt1):
    c0, c1, idx = struct.unpack_from("<HHI", block)
    p0, p1 = _rgb565(c0), _rgb565(c1)
    if c0 > c1 or not dxt1:
        p2 = tuple((2 * a + b) // 3 for a, b in zip(p0, p1)) + (255,)
        p3 = tuple((a + 2 * b) // 3 for a, b in zip(p0, p1)) + (255,)
    else:
        p2 = tuple((a + b) // 2 for a, b in zip(p0, p1)) + (255,)
        p3 = (0, 0, 0, 0)
    pal = (p0 + (255,), p1 + (255,), p2, p3)
    for i in range(16):
        x, y = x0 + (i & 3), y0 + (i >> 2)
        if x < width and y < height:
            o = (y * width + x) * 4
            out[o:o + 4] = bytes(pal[(idx >> (2 * i)) & 3])


def decode_dxt(data, width, height, fmt):
    """Decode DXT1/3/5 (already converted to PC little-endian word order) to RGBA."""
    out = bytearray(width * height * 4)
    bsize = 8 if fmt == SURFACE_DXT1 else 16
    pos = 0
    for by in range(0, height, 4):
        for bx in range(0, width, 4):
            block = data[pos:pos + bsize]
            pos += bsize
            if fmt == SURFACE_DXT1:
                _color_block(block, out, bx, by, width, height, True)
                continue
            _color_block(block[8:], out, bx, by, width, height, False)
            if fmt == SURFACE_DXT3:
                bits = int.from_bytes(block[:8], "little")
                alphas = [((bits >> (4 * i)) & 15) * 17 for i in range(16)]
            else:
                a0, a1 = block[0], block[1]
                pal = [a0, a1]
                if a0 > a1:
                    pal += [((7 - i) * a0 + i * a1) // 7 for i in range(1, 7)]
                else:
                    pal += [((5 - i) * a0 + i * a1) // 5 for i in range(1, 5)] + [0, 255]
                bits = int.from_bytes(block[2:8], "little")
                alphas = [pal[(bits >> (3 * i)) & 7] for i in range(16)]
            for i in range(16):
                x, y = bx + (i & 3), by + (i >> 2)
                if x < width and y < height:
                    out[(y * width + x) * 4 + 3] = alphas[i]
    return bytes(out)


# --- XNB container --------------------------------------------------------------------

def read_xnb_body(path):
    d = open(path, "rb").read()
    if d[:3] != b"XNB":
        raise ValueError("not an XNB file")
    platform, version, flags = chr(d[3]), d[4], d[5]
    if version != 3:
        raise ValueError(f"unsupported XNB version {version}")
    if flags & 0x80:
        size = struct.unpack_from("<I", d, 10)[0]
        return platform, decompress_xnb_payload(d[14:], size)
    return platform, d[10:]


class ContentReader:
    def __init__(self, body):
        self.d = body
        self.p = 0
        n = self.read7()
        self.readers = []
        for _ in range(n):
            self.readers.append(self.read_string())
            self.i32()  # reader version
        self.shared_count = self.read7()

    # primitives
    def read7(self):
        v = s = 0
        while True:
            b = self.d[self.p]
            self.p += 1
            v |= (b & 0x7F) << s
            s += 7
            if not b & 0x80:
                return v

    def take(self, n):
        v = self.d[self.p:self.p + n]
        self.p += n
        return v

    def unpack(self, fmt):
        v = struct.unpack_from(fmt, self.d, self.p)
        self.p += struct.calcsize(fmt)
        return v

    def i32(self):
        return self.unpack("<i")[0]

    def u32(self):
        return self.unpack("<I")[0]

    def f32(self):
        return self.unpack("<f")[0]

    def u8(self):
        return self.take(1)[0]

    def read_string(self):
        return self.take(self.read7()).decode("utf-8")

    def read_char(self):
        b = self.d[self.p]
        n = 1 if b < 0x80 else 2 if b < 0xE0 else 3 if b < 0xF0 else 4
        return self.take(n).decode("utf-8")

    # objects
    def reader_name(self, type_id):
        full = self.readers[type_id - 1]
        return full.split(",")[0].split("[[")[0].rsplit(".", 1)[-1]

    def read_object(self):
        tid = self.read7()
        if tid == 0:
            return None
        name = self.reader_name(tid)
        fn = getattr(self, "read_" + name.replace("`1", "").replace("Reader", ""), None)
        if fn is None:
            raise NotImplementedError(f"no reader for {self.readers[tid - 1]}")
        self._type_id = tid
        return fn()

    def read_root(self):
        obj = self.read_object()
        shared = [self.read_object() for _ in range(self.shared_count)]
        return obj, shared

    def read_String(self):
        return self.read_string()

    def read_Texture2D(self):
        fmt, w, h, levels = self.unpack("<iIII")
        mips = [self.take(self.u32()) for _ in range(levels)]
        return {"type": "Texture2D", "format": fmt, "width": w, "height": h, "mips": mips}

    def read_Effect(self):
        return {"type": "Effect", "bytecode": self.take(self.u32())}

    def read_List(self):
        # Only value-type lists occur here (Rectangle, Char, Vector3): elements have no type id.
        elem = self.readers[self._type_id - 1]
        count = self.u32()
        if "Rectangle" in elem:
            return [list(self.unpack("<4i")) for _ in range(count)]
        if "Char" in elem:
            return [self.read_char() for _ in range(count)]
        if "Vector3" in elem:
            return [list(self.unpack("<3f")) for _ in range(count)]
        raise NotImplementedError(f"list of {elem}")

    def read_SpriteFont(self):
        tex = self.read_object()
        glyphs = self.read_object()
        cropping = self.read_object()
        chars = self.read_object()
        line_spacing = self.i32()
        spacing = self.f32()
        kerning = self.read_object()
        default_char = self.read_char() if self.u8() else None
        return {"type": "SpriteFont", "texture": tex, "glyphs": glyphs, "cropping": cropping,
                "characters": chars, "line_spacing": line_spacing, "spacing": spacing,
                "kerning": kerning, "default_char": default_char}

    def read_VertexDeclaration(self):
        elems = []
        for _ in range(self.u32()):
            stream, offset = self.unpack("<HH")
            fmt, method, usage, usage_index = self.take(4)
            elems.append({"stream": stream, "offset": offset, "format": fmt, "method": method,
                          "usage": VERTEX_USAGES.get(usage, usage), "usage_index": usage_index})
        return {"type": "VertexDeclaration", "elements": elems}

    def read_VertexBuffer(self):
        return {"type": "VertexBuffer", "data": self.take(self.u32())}

    def read_IndexBuffer(self):
        sixteen = bool(self.u8())
        return {"type": "IndexBuffer", "sixteen_bit": sixteen, "data": self.take(self.u32())}

    def read_BasicEffect(self):
        texture = self.read_string()  # external reference
        diffuse = self.unpack("<3f")
        emissive = self.unpack("<3f")
        specular = self.unpack("<3f")
        power, alpha = self.unpack("<2f")
        vertex_color = bool(self.u8())
        return {"type": "BasicEffect", "texture": texture, "diffuse": diffuse, "emissive": emissive,
                "specular": specular, "specular_power": power, "alpha": alpha,
                "vertex_color_enabled": vertex_color}

    def read_Model(self):
        nbones = self.u32()

        def bone_ref():
            v = self.u8() if nbones <= 255 else self.u32()
            return v - 1 if v else None

        bones = []
        for _ in range(nbones):
            name = self.read_object()
            bones.append({"name": name, "transform": list(self.unpack("<16f"))})
        for b in bones:
            b["parent"] = bone_ref()
            b["children"] = [bone_ref() for _ in range(self.u32())]
        decls = [self.read_object() for _ in range(self.u32())]
        meshes = []
        for _ in range(self.u32()):
            m = {"name": self.read_object(), "parent_bone": bone_ref(),
                 "bounding_sphere": list(self.unpack("<4f"))}
            m["vertex_buffer"] = self.read_object()
            m["index_buffer"] = self.read_object()
            m["tag"] = self.read_object()
            parts = []
            for _ in range(self.u32()):
                stream_offset, base_vertex, num_vertices, start_index, prim_count = self.unpack("<5i")
                decl = self.i32()  # index into the model's vertex declaration table
                tag = self.read_object()
                effect = self.read7() - 1  # shared resource index
                parts.append({"stream_offset": stream_offset, "base_vertex": base_vertex,
                              "num_vertices": num_vertices, "start_index": start_index,
                              "primitive_count": prim_count, "vertex_declaration": decl,
                              "tag": tag, "effect": effect})
            m["parts"] = parts
            meshes.append(m)
        root = bone_ref()
        tag = self.read_object()
        return {"type": "Model", "bones": bones, "vertex_declarations": decls, "meshes": meshes,
                "root_bone": root, "tag": tag}


# --- exporters ------------------------------------------------------------------------

def texture_rgba(tex, level=0):
    w, h = tex["width"] >> level or 1, tex["height"] >> level or 1
    data = tex["mips"][level]
    if tex["format"] == SURFACE_COLOR:
        return w, h, argb_to_rgba(data)
    if tex["format"] in (SURFACE_DXT1, SURFACE_DXT3, SURFACE_DXT5):
        return w, h, decode_dxt(swap16(data), w, h, tex["format"])
    raise NotImplementedError(f"surface format {tex['format']}")


def export_texture(tex, out_base):
    w, h, rgba = texture_rgba(tex)
    write_png(out_base + ".png", w, h, rgba)


def export_font(font, out_base):
    export_texture(font["texture"], out_base)
    meta = {k: v for k, v in font.items() if k not in ("texture", "type")}
    meta["texture_format"] = font["texture"]["format"]
    with open(out_base + ".json", "w", encoding="utf-8") as f:
        json.dump(meta, f, ensure_ascii=False, indent=1)


def mat_mul(a, b):
    """Row-major 4x4 multiply (XNA row-vector convention: v * a * b)."""
    return [sum(a[r * 4 + k] * b[k * 4 + c] for k in range(4)) for r in range(4) for c in range(4)]


def transform_point(v, m, w=1.0):
    x, y, z = v
    return [x * m[0] + y * m[4] + z * m[8] + w * m[12],
            x * m[1] + y * m[5] + z * m[9] + w * m[13],
            x * m[2] + y * m[6] + z * m[10] + w * m[14]]


def export_model(model, shared, out_base):
    bones = model["bones"]
    absolute = [None] * len(bones)

    def abs_transform(i):
        if absolute[i] is None:
            b = bones[i]
            t = b["transform"]
            absolute[i] = t if b["parent"] is None else mat_mul(t, abs_transform(b["parent"]))
        return absolute[i]

    name = os.path.basename(out_base)
    obj = [f"# Storage Inc model {name}", f"mtllib {name}.mtl"]
    mtl = []
    meta = {"bones": bones, "meshes": [], "shared": []}
    for i, s in enumerate(shared):
        if s["type"] == "BasicEffect":
            mtl += [f"newmtl effect{i}", "Kd %.4f %.4f %.4f" % tuple(s["diffuse"]),
                    "Ke %.4f %.4f %.4f" % tuple(s["emissive"]),
                    "Ks %.4f %.4f %.4f" % tuple(s["specular"]), f"Ns {s['specular_power']:.4f}",
                    f"d {s['alpha']:.4f}"]
            if s["texture"]:
                tex = s["texture"].replace("\\", "/")
                mtl.append(f"map_Kd {tex}.png")
            mtl.append("")
        meta["shared"].append(s)

    vbase = tbase = nbase = 1
    for mesh in model["meshes"]:
        world = abs_transform(mesh["parent_bone"])
        vb, ib = mesh["vertex_buffer"]["data"], mesh["index_buffer"]
        ifmt = ">H" if ib["sixteen_bit"] else ">I"
        isize = struct.calcsize(ifmt)
        obj.append(f"o {mesh['name']}")
        for pi, part in enumerate(mesh["parts"]):
            decl = model["vertex_declarations"][part["vertex_declaration"]]["elements"]
            stride = max(e["offset"] + VERTEX_FORMAT_SIZES[e["format"]] for e in decl)
            by_usage = {(e["usage"], e["usage_index"]): e for e in decl}
            pos_e = by_usage.get(("Position", 0))
            nrm_e = by_usage.get(("Normal", 0))
            uv_e = by_usage.get(("TextureCoordinate", 0))

            def read_elem(e, vi):
                fmt, _ = VERTEX_FORMATS[e["format"]]
                return struct.unpack_from(fmt, vb, part["stream_offset"] + vi * stride + e["offset"])

            first = part["base_vertex"]
            count = part["num_vertices"]
            for vi in range(first, first + count):
                obj.append("v %.6f %.6f %.6f" % tuple(transform_point(read_elem(pos_e, vi), world)))
                if uv_e:
                    u, v = read_elem(uv_e, vi)[:2]
                    obj.append(f"vt {u:.6f} {1 - v:.6f}")
                if nrm_e:
                    obj.append("vn %.6f %.6f %.6f" % tuple(transform_point(read_elem(nrm_e, vi), world, 0.0)))
            obj.append(f"g {mesh['name']}_part{pi}")
            obj.append(f"usemtl effect{part['effect']}")
            for t in range(part["primitive_count"]):
                idx = [struct.unpack_from(ifmt, ib["data"], (part["start_index"] + t * 3 + k) * isize)[0]
                       for k in range(3)]
                # XNA is clockwise-front; OBJ expects counter-clockwise.
                corners = []
                for k in (0, 2, 1):
                    local = idx[k]  # relative to base_vertex (DrawIndexedPrimitives semantics)
                    vt = f"{tbase + local}" if uv_e else ""
                    vn = f"{nbase + local}" if nrm_e else ""
                    corners.append(f"{vbase + local}/{vt}/{vn}" if (vt or vn) else f"{vbase + local}")
                obj.append("f " + " ".join(corners))
            vbase += count
            tbase += count if uv_e else 0
            nbase += count if nrm_e else 0
            part_meta = dict(part, vertex_stride=stride, vertex_elements=decl)
            meta["meshes"].append({"name": mesh["name"], "part": pi,
                                   "bounding_sphere": mesh["bounding_sphere"], **part_meta})

    with open(out_base + ".obj", "w") as f:
        f.write("\n".join(obj) + "\n")
    with open(out_base + ".mtl", "w") as f:
        f.write("\n".join(mtl) + "\n")
    for s in meta["shared"]:
        s.pop("data", None)
    with open(out_base + ".json", "w") as f:
        json.dump(meta, f, indent=1, default=str)


def export_effect(effect, out_base):
    with open(out_base + ".fxo", "wb") as f:
        f.write(effect["bytecode"])


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("content_dir", nargs="?", default="extracted/584E07D1/Content")
    ap.add_argument("out_dir", nargs="?", default="assets")
    ap.add_argument("--raw", action="store_true", help="also write decompressed XNB bodies")
    args = ap.parse_args()

    counts, failures = {}, 0
    for root, _, files in os.walk(args.content_dir):
        for fn in sorted(files):
            if not fn.lower().endswith(".xnb"):
                continue
            src = os.path.join(root, fn)
            rel = os.path.relpath(src, args.content_dir)
            out_base = os.path.join(args.out_dir, os.path.splitext(rel)[0])
            os.makedirs(os.path.dirname(out_base), exist_ok=True)
            try:
                platform, body = read_xnb_body(src)
                if args.raw:
                    with open(out_base + ".xnb.bin", "wb") as f:
                        f.write(body)
                r = ContentReader(body)
                obj, shared = r.read_root()
                kind = obj["type"]
                {"Texture2D": lambda: export_texture(obj, out_base),
                 "SpriteFont": lambda: export_font(obj, out_base),
                 "Model": lambda: export_model(obj, shared, out_base),
                 "Effect": lambda: export_effect(obj, out_base)}[kind]()
                if r.p != len(body):
                    print(f"  note: {rel}: {len(body) - r.p} trailing bytes")
                counts[kind] = counts.get(kind, 0) + 1
            except Exception as e:  # keep going, report at the end
                failures += 1
                print(f"FAILED {rel}: {e!r}")
    print(", ".join(f"{v} {k}" for k, v in sorted(counts.items())) + f", {failures} failed")
    return 1 if failures else 0


if __name__ == "__main__":
    sys.exit(main())
