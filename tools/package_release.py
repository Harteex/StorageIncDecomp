#!/usr/bin/env python3
"""Publish self-contained Release builds of the FNA port and pack them into archives.

Usage:
    python tools/package_release.py [rid ...]

Default RIDs: win-x64, linux-x64, osx-arm64, osx-x64 (linux-arm64 also works). Each build goes to
dist/<rid>/StorageInc/ and is packed as dist/StorageInc-<rid>.zip (Windows) or .tar.gz (Linux,
macOS). The tarballs are written here rather than by a system tar so the executable keeps its
+x bit even when packing on Windows.

Needs the converted content (tools/build_content.py) and the native libraries
(tools/fetch_fnalibs.py). The archives contain the game's content. The original author allows
releasing the port only as long as nobody makes money from it in any way. dist/ is gitignored.
"""
import os
import shutil
import subprocess
import sys
import tarfile
import zipfile

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
DIST = os.path.join(ROOT, "dist")
PROJECT = os.path.join(ROOT, "src", "StorageInc", "StorageInc.csproj")
DEFAULT_RIDS = ["win-x64", "linux-x64", "osx-arm64", "osx-x64"]
NAME = "StorageInc"


def publish(rid):
    out = os.path.join(DIST, rid, NAME)
    shutil.rmtree(os.path.join(DIST, rid), ignore_errors=True)
    subprocess.run(
        ["dotnet", "publish", PROJECT, "-c", "Release", "-r", rid, "--self-contained", "-o", out,
         "-p:DebugType=none", "-p:DebugSymbols=false", "-nologo", "-v", "q"],
        check=True,
    )
    return out


def check(out, rid):
    content = sum(len(files) for _, _, files in os.walk(os.path.join(out, "Content")))
    native = {
        "win": ["SDL3.dll", "FNA3D.dll", "FAudio.dll"],
        "linux": ["libSDL3.so.0", "libFNA3D.so.0", "libFAudio.so.0"],
        "osx": ["libSDL3.0.dylib", "libFNA3D.0.dylib", "libFAudio.0.dylib"],
    }[rid.split("-")[0]]
    missing = [n for n in native if not os.path.exists(os.path.join(out, n))]
    if missing or content == 0:
        raise SystemExit(f"{rid}: missing native libraries {missing} or content ({content} files)")
    return content


def pack(out, rid):
    if rid.startswith("win"):
        path = os.path.join(DIST, f"{NAME}-{rid}.zip")
        with zipfile.ZipFile(path, "w", zipfile.ZIP_DEFLATED) as z:
            for base, _, files in os.walk(out):
                for f in files:
                    full = os.path.join(base, f)
                    z.write(full, os.path.join(NAME, os.path.relpath(full, out)))
        return path
    path = os.path.join(DIST, f"{NAME}-{rid}.tar.gz")
    exe = os.path.join(out, NAME)

    def mode(info):
        full = os.path.join(out, os.path.relpath(info.name, NAME))
        if info.isdir() or os.path.normcase(full) == os.path.normcase(exe):
            info.mode = 0o755
        else:
            info.mode = 0o644
        info.uid = info.gid = 0
        info.uname = info.gname = ""
        return info

    with tarfile.open(path, "w:gz") as t:
        t.add(out, NAME, filter=mode)
    return path


def main():
    rids = sys.argv[1:] or DEFAULT_RIDS
    for rid in rids:
        print(f"== {rid}")
        out = publish(rid)
        content = check(out, rid)
        archive = pack(out, rid)
        print(f"  {out} ({content} content files)")
        print(f"  {archive} ({os.path.getsize(archive) / 1e6:.1f} MB)")


if __name__ == "__main__":
    main()
