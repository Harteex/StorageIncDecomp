#!/usr/bin/env python3
"""Compile the port's effects (src/StorageInc/Shaders/*.fx) to FNA effect binaries (.fxb).

Usage:
    python tools/build_shaders.py [--fxc PATH]

FNA loads D3D9 effect binaries (fx_2_0), which only Microsoft's fxc.exe produces; it ships
with the Windows SDK. The .fxb files are committed next to their sources so the game can
be built on any OS without fxc. The csproj copies them into Content/ under the asset names
the game loads.
"""
import argparse
import glob
import os
import subprocess
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SHADERS = os.path.join(ROOT, "src", "StorageInc", "Shaders")


def find_fxc(explicit):
    if explicit:
        return explicit
    kits = os.path.join(os.environ.get("ProgramFiles(x86)", r"C:\Program Files (x86)"), "Windows Kits", "10", "bin")
    candidates = sorted(glob.glob(os.path.join(kits, "10.*", "x64", "fxc.exe")))
    return candidates[-1] if candidates else None


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--fxc", help="path to fxc.exe (default: newest Windows 10/11 SDK)")
    args = ap.parse_args()
    fxc = find_fxc(args.fxc)
    if not fxc or not os.path.exists(fxc):
        sys.exit("fxc.exe not found; install the Windows SDK or pass --fxc")

    failures = 0
    for src in sorted(glob.glob(os.path.join(SHADERS, "*.fx"))):
        dst = os.path.splitext(src)[0] + ".fxb"
        r = subprocess.run([fxc, "-nologo", "-T", "fx_2_0", "-O3", "-Fo", dst, src], capture_output=True, text=True)
        messages = [l for l in (r.stdout + r.stderr).splitlines()
                    if l.strip() and "X4717" not in l and "compilation object save succeeded" not in l]
        status = "ok" if r.returncode == 0 else "FAILED"
        print(f"{status:6} {os.path.basename(src)}")
        for m in messages:
            print("       " + m)
        failures += r.returncode != 0
    return 1 if failures else 0


if __name__ == "__main__":
    sys.exit(main())
