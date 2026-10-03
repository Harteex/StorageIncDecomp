#!/usr/bin/env python3
"""Fix mechanical ILSpy artifacts using the C# compiler's own diagnostics.

Usage:
    python tools/fix_decomp.py <project.csproj>

ILSpy decompiled without the XNA 3.0 assemblies, so it couldn't see XNA method
signatures or that XNA enums are enums. The compiler pinpoints every resulting site:

  CS1620  "Argument N must be passed with the 'out' keyword"
          -> the `ref` just before the column becomes `out`
  CS0266  "Cannot implicitly convert type '...PlayerIndex' to 'int'" (array indexing)
          -> `(int)` is inserted before the expression at the column
  CS0266  "Cannot implicitly convert type 'int' to '...<Enum>'" for XNA input enums
          -> `(<Enum>)` is inserted before the expression at the column

The project is rebuilt until no fixable diagnostic remains. (CS0269 errors are a side
effect of the ref/out mismatch and disappear.)

Decompiling with FNA as the XNA reference would avoid this, but it maps XNA 3.0 enum
constants to FNA's XNA 4.0 enum names, which are numbered differently. See docs/porting.md.
"""
import re
import subprocess
import sys
from collections import defaultdict

DIAG = re.compile(r"^(?P<file>.+?)\((?P<line>\d+),(?P<col>\d+)\): error (?P<code>CS\d+): (?P<msg>.*?)(?: \[.*\])?$")
# Enums whose numeric values are identical in XNA 3.0 and XNA 4.0, so a cast is safe.
SAFE_ENUMS = {"Buttons", "Keys", "PlayerIndex"}
EXPR = re.compile(r"[\w.]+(?:\(\))?")


def classify(code, msg):
    if code == "CS1620":
        return ("out", None)
    if code == "CS0266":
        m = re.match(r"Cannot implicitly convert type '([\w.]+)' to '([\w.]+)'", msg)
        if m:
            src, dst = m.group(1).rsplit(".", 1)[-1], m.group(2).rsplit(".", 1)[-1]
            if src == "PlayerIndex" and dst == "int":
                return ("cast", "int")
            if src == "int" and dst in SAFE_ENUMS:
                return ("cast", dst)
    return None


def build(project):
    r = subprocess.run(["dotnet", "build", project, "--no-restore", "-v", "q", "-clp:NoSummary"],
                       capture_output=True, text=True)
    sites = defaultdict(set)
    for line in r.stdout.splitlines():
        m = DIAG.match(line.strip())
        if not m:
            continue
        fix = classify(m["code"], m["msg"])
        if fix:
            sites[m["file"]].add((int(m["line"]), int(m["col"])) + fix)
    return sites


def apply(text, col, kind, arg):
    i = col - 1
    if kind == "out":
        if text[i - 4:i] != "ref ":
            raise ValueError("expected 'ref ' before column")
        return text[:i - 4] + "out " + text[i:]
    m = EXPR.match(text, i)
    if not m:
        raise ValueError("no expression at column")
    return text[:i] + f"({arg})" + text[i:]


def main():
    project = sys.argv[1]
    subprocess.run(["dotnet", "restore", project, "-v", "q"], check=True, capture_output=True)
    total = 0
    for _ in range(20):
        sites = build(project)
        if not sites:
            break
        for path, locs in sites.items():
            with open(path, encoding="utf-8-sig") as f:
                lines = f.read().split("\n")
            # Right-to-left so earlier columns on the same line stay valid.
            for line_no, col, kind, arg in sorted(locs, reverse=True):
                try:
                    lines[line_no - 1] = apply(lines[line_no - 1], col, kind, arg)
                except ValueError as e:
                    raise SystemExit(f"{path}:{line_no}:{col}: {e}: {lines[line_no - 1].strip()}")
                total += 1
            with open(path, "w", encoding="utf-8") as f:
                f.write("\n".join(lines))
        print(f"fixed {sum(len(v) for v in sites.values())} sites in {len(sites)} files")
    print(f"done, {total} fixes applied")


if __name__ == "__main__":
    main()
