#!/usr/bin/env python3
"""Download FNA's native libraries (SDL3, FNA3D, FAudio, Theorafile) into fnalibs/.

Usage:
    python tools/fetch_fnalibs.py

Takes the latest "fnalibs" (Windows x64/x86, Linux x64/arm64) and "fnalibs-apple" (macOS; only
its osx/ folder is kept) artifacts of FNA-XNA/fnalibs-dailies on main. GitHub only serves
Actions artifacts to signed-in users, so this goes through nightly.link, which redirects to
GitHub's own artifact storage. FnaLibs.targets copies the right folder next to the game.
"""
import io
import os
import shutil
import urllib.request
import zipfile

BASE = "https://nightly.link/FNA-XNA/fnalibs-dailies/workflows/ci/main/"
ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
OUT = os.path.join(ROOT, "fnalibs")


def download(name):
    url = BASE + name + ".zip"
    print(f"downloading {url}")
    with urllib.request.urlopen(url) as r:
        data = r.read()
    print(f"  {len(data)} bytes")
    return zipfile.ZipFile(io.BytesIO(data))


def main():
    shutil.rmtree(OUT, ignore_errors=True)
    download("fnalibs").extractall(OUT)
    apple = download("fnalibs-apple")
    for member in apple.namelist():
        if member.startswith("osx/"):
            apple.extract(member, OUT)
    for d in sorted(os.listdir(OUT)):
        print(" ", d)


if __name__ == "__main__":
    main()
