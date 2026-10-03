#!/usr/bin/env python3
"""Download FNA's native libraries (SDL3, FNA3D, FAudio, Theorafile) into fnalibs/.

Usage:
    python tools/fetch_fnalibs.py

Takes the latest "fnalibs" artifact of FNA-XNA/fnalibs-dailies on main. GitHub only serves
Actions artifacts to signed-in users, so this goes through nightly.link, which redirects to
GitHub's own artifact storage.
"""
import io
import os
import shutil
import urllib.request
import zipfile

URL = "https://nightly.link/FNA-XNA/fnalibs-dailies/workflows/ci/main/fnalibs.zip"
ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
OUT = os.path.join(ROOT, "fnalibs")


def main():
    print(f"downloading {URL}")
    with urllib.request.urlopen(URL) as r:
        data = r.read()
    print(f"{len(data)} bytes")
    shutil.rmtree(OUT, ignore_errors=True)
    with zipfile.ZipFile(io.BytesIO(data)) as z:
        z.extractall(OUT)
    for d in sorted(os.listdir(OUT)):
        print(" ", d)


if __name__ == "__main__":
    main()
