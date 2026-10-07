#!/usr/bin/env python3
"""Check that every content path the game uses exists with exactly that case.

Usage:
    python tools/check_content_paths.py [content_dir]

Default content_dir: src/StorageInc/bin/Debug/net8.0/Content (build the game first).

Windows and (usually) macOS filesystems are case-insensitive, Linux is not, and FNA's
case-insensitive lookup (CASE_SENSITIVITY_HACK) is off by default. A name that differs
only in case from the file on disk works on Windows and fails on Linux. This collects:
  - literal asset names in src/StorageInc (Content.Load, AudioEngine/WaveBank/SoundBank),
  - the dynamic stage and map preview names (stages/NNN.xml, stages/mapimages/imageNNN),
  - external references inside the converted model XNBs (BasicEffect textures),
then resolves each one component by component with exact-case matching.
"""
import os
import re
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SRC = os.path.join(ROOT, "src", "StorageInc")

LOAD = re.compile(r'\.Load<\w+>\("([^"]+)"\)')
XACT = re.compile(r'new (?:AudioEngine|WaveBank|SoundBank)\((?:\w+, )?"([^"]+)"\)')
CONTENT_EXTENSIONS = (".xnb", ".fxb")


def exact_case_exists(base, rel):
    """True if base/rel exists with every path component matching case exactly."""
    cur = base
    for part in [p for p in re.split(r"[\\/]+", rel) if p and p != "."]:
        if part == "..":
            cur = os.path.dirname(cur)
            continue
        try:
            entries = os.listdir(cur)
        except (FileNotFoundError, NotADirectoryError):
            return False
        if part not in entries:
            return False
        cur = os.path.join(cur, part)
    return True


def asset_exists(content, name):
    """A ContentManager asset: name + .xnb (or a raw .fxb), relative to Content/."""
    name = re.sub(r"^Content[\\/]", "", name)
    return any(exact_case_exists(content, name + ext) for ext in CONTENT_EXTENSIONS)


def code_references():
    for dirpath, _, files in os.walk(SRC):
        if os.sep + "bin" in dirpath or os.sep + "obj" in dirpath:
            continue
        for fn in files:
            if not fn.endswith(".cs"):
                continue
            path = os.path.join(dirpath, fn)
            text = open(path, encoding="utf-8-sig").read()
            for regex, kind in ((LOAD, "asset"), (XACT, "file")):
                for m in regex.finditer(text):
                    line = text.count("\n", 0, m.start()) + 1
                    yield kind, m.group(1), f"{os.path.relpath(path, ROOT)}:{line}"


def model_references(content):
    """BasicEffect texture references inside converted model XNBs (resolved from the model's folder)."""
    for dirpath, _, files in os.walk(content):
        for fn in files:
            if not fn.endswith(".xnb"):
                continue
            data = open(os.path.join(dirpath, fn), "rb").read()
            if b"BasicEffectReader" not in data:
                continue
            # External references are 7-bit length-prefixed strings ("../textures/Floor_0").
            for m in re.finditer(rb"\.\./", data):
                length = data[m.start() - 1]
                name = data[m.start():m.start() + length].decode("utf-8")
                rel_dir = os.path.relpath(dirpath, content)
                yield "asset", os.path.join(rel_dir, name), os.path.relpath(os.path.join(dirpath, fn), content)


def main():
    content = sys.argv[1] if len(sys.argv) > 1 else os.path.join(SRC, "bin", "Debug", "net8.0", "Content")
    if not os.path.isdir(content):
        sys.exit(f"{content} not found; build the game first or pass the Content folder")
    refs = list(code_references())
    refs += [("file", f"stages/{i:03d}.xml", "GameplayScreen (stage data)") for i in range(1, 101)]
    refs += [("asset", f"stages/mapimages/image{i:03d}", "ScreenManager.NewMapSelected") for i in range(1, 101)]
    refs += list(model_references(content))

    failures = 0
    for kind, name, where in refs:
        if kind == "asset":
            ok = asset_exists(content, name)
        else:
            ok = exact_case_exists(content, re.sub(r"^Content[\\/]", "", name))
        if not ok:
            failures += 1
            print(f"MISSING  {name}  ({where})")
    print(f"{len(refs)} references checked, {failures} missing or case-mismatched")
    return 1 if failures else 0


if __name__ == "__main__":
    sys.exit(main())
