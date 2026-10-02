#!/usr/bin/env python3
"""Extract files from an Xbox 360 STFS package (CON/LIVE/PIRS).

Usage:
    python tools/stfs_extract.py <package> [out_dir] [--list] [--no-verify]

Format notes (see docs/stfs.md):
  - Header is big-endian, except the volume descriptor's file-table fields (little-endian).
  - Data blocks are 0x1000 bytes, interleaved with SHA-1 hash tables. Each level-0 hash
    table covers 0xAA blocks; each level-1 table covers 0xAA * 0xAA = 0x70E4 blocks.
  - "Male" packages (block separation bit 0 == 0) keep two copies of every hash table;
    "female" packages (read-only LIVE/PIRS content) keep one.
"""
import argparse
import hashlib
import os
import struct
import sys

BLOCK_SIZE = 0x1000
HASHES_PER_TABLE = 0xAA
HASHES_PER_L1 = 0x70E4
HASH_ENTRY_SIZE = 0x18
FILE_ENTRY_SIZE = 0x40


def u24le(b, o):
    return b[o] | (b[o + 1] << 8) | (b[o + 2] << 16)


def u24be(b, o):
    return (b[o] << 16) | (b[o + 1] << 8) | b[o + 2]


class StfsPackage:
    def __init__(self, path):
        with open(path, "rb") as f:
            self.data = f.read()
        d = self.data
        self.magic = d[0:4].decode("ascii", "replace")
        if self.magic not in ("CON ", "LIVE", "PIRS"):
            raise ValueError(f"not an STFS package (magic {d[0:4]!r})")

        self.header_size = struct.unpack_from(">I", d, 0x340)[0]
        self.content_type = struct.unpack_from(">I", d, 0x344)[0]
        self.title_id = struct.unpack_from(">I", d, 0x360)[0]
        self.display_name = d[0x411:0x411 + 0x80].decode("utf-16-be").split("\0")[0]

        vd = 0x379
        self.block_separation = d[vd + 2]
        self.file_table_block_count = struct.unpack_from("<H", d, vd + 3)[0]
        self.file_table_block_num = u24le(d, vd + 5)
        self.allocated_blocks = struct.unpack_from(">I", d, vd + 0x1C)[0]

        # 0 = female (one hash table copy), 1 = male (two copies).
        self.shift = 0 if (self.block_separation & 1) else 1
        self.first_table_addr = (self.header_size + 0xFFF) & 0xFFFFF000
        self.block_step0 = 0xAB if self.shift == 0 else 0xAC

    # --- block addressing -------------------------------------------------

    def backing_block_num(self, block):
        """Map a logical data block number to its physical block index."""
        sh = self.shift
        r = (((block + HASHES_PER_TABLE) // HASHES_PER_TABLE) << sh) + block
        if block < HASHES_PER_TABLE:
            return r
        if block < HASHES_PER_L1:
            return r + (((block + HASHES_PER_L1) // HASHES_PER_L1) << sh)
        return (1 << sh) + r + (((block + HASHES_PER_L1) // HASHES_PER_L1) << sh)

    def block_addr(self, block):
        return (self.backing_block_num(block) << 12) + self.first_table_addr

    def read_block(self, block):
        a = self.block_addr(block)
        return self.data[a:a + BLOCK_SIZE]

    def level0_hash_block_num(self, block):
        sh = self.shift
        if block < HASHES_PER_TABLE:
            return 0
        n = (block // HASHES_PER_TABLE) * self.block_step0
        n += ((block // HASHES_PER_L1) + 1) << sh
        if block // HASHES_PER_L1 == 0:
            return n
        return n + (1 << sh)

    def hash_entry(self, block):
        """Return (sha1, status, next_block) for a logical data block."""
        if self.shift == 1:
            # Male packages: which of the two table copies is live is decided by the
            # parent table. Not needed for read-only LIVE/PIRS content yet.
            raise NotImplementedError("male (two hash table) packages are not supported yet")
        a = (self.level0_hash_block_num(block) << 12) + self.first_table_addr
        a += (block % HASHES_PER_TABLE) * HASH_ENTRY_SIZE
        e = self.data[a:a + HASH_ENTRY_SIZE]
        return e[:20], e[20], u24be(e, 21)

    def verify_block(self, block):
        sha, _, _ = self.hash_entry(block)
        return hashlib.sha1(self.read_block(block)).digest() == sha

    # --- file table ---------------------------------------------------------

    def block_chain(self, start, count, consecutive):
        if consecutive:
            return list(range(start, start + count))
        chain, b = [], start
        for _ in range(count):
            chain.append(b)
            b = self.hash_entry(b)[2]
        return chain

    def read_file_table(self):
        entries = []
        blocks = self.block_chain(self.file_table_block_num, self.file_table_block_count, False)
        for blk in blocks:
            raw = self.read_block(blk)
            for off in range(0, BLOCK_SIZE, FILE_ENTRY_SIZE):
                e = raw[off:off + FILE_ENTRY_SIZE]
                flags = e[0x28]
                name_len = flags & 0x3F
                if name_len == 0:
                    continue
                entries.append({
                    "index": len(entries),
                    "name": e[:name_len].decode("ascii", "replace"),
                    "is_dir": bool(flags & 0x80),
                    "consecutive": bool(flags & 0x40),
                    "blocks": u24le(e, 0x29),
                    "start": u24le(e, 0x2F),
                    "parent": struct.unpack_from(">h", e, 0x32)[0],
                    "size": struct.unpack_from(">I", e, 0x34)[0],
                })
        return entries

    def full_path(self, entries, entry):
        parts = [entry["name"]]
        p = entry["parent"]
        while p != -1:
            parts.append(entries[p]["name"])
            p = entries[p]["parent"]
        return "/".join(reversed(parts))

    def read_file(self, entry, verify=True):
        chain = self.block_chain(entry["start"], entry["blocks"], entry["consecutive"])
        bad = [b for b in chain if verify and not self.verify_block(b)]
        data = b"".join(self.read_block(b) for b in chain)[:entry["size"]]
        return data, bad


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("package")
    ap.add_argument("out_dir", nargs="?", default="extracted")
    ap.add_argument("--list", action="store_true", help="only list the file table")
    ap.add_argument("--no-verify", action="store_true", help="skip SHA-1 block verification")
    args = ap.parse_args()

    pkg = StfsPackage(args.package)
    print(f"magic={pkg.magic} title_id={pkg.title_id:08X} content_type={pkg.content_type:08X} "
          f"name={pkg.display_name!r}")
    print(f"header_size=0x{pkg.header_size:X} first_table=0x{pkg.first_table_addr:X} "
          f"{'female' if pkg.shift == 0 else 'male'} allocated_blocks={pkg.allocated_blocks} "
          f"file_table=block {pkg.file_table_block_num} x{pkg.file_table_block_count}")

    entries = pkg.read_file_table()
    total_bad = 0
    for e in entries:
        path = pkg.full_path(entries, e)
        if e["is_dir"]:
            print(f"  [dir]            {path}/")
            if not args.list:
                os.makedirs(os.path.join(args.out_dir, path), exist_ok=True)
            continue
        print(f"  {e['size']:>10}  {'C' if e['consecutive'] else ' '}  {path}")
        if args.list:
            continue
        data, bad = pkg.read_file(e, verify=not args.no_verify)
        if bad:
            total_bad += len(bad)
            print(f"    WARNING: {len(bad)} block(s) failed SHA-1 verification: {bad[:10]}")
        out = os.path.join(args.out_dir, path)
        os.makedirs(os.path.dirname(out) or ".", exist_ok=True)
        with open(out, "wb") as f:
            f.write(data)

    print(f"{len(entries)} entries" + ("" if args.list else f", {total_bad} bad blocks"))
    return 1 if total_bad else 0


if __name__ == "__main__":
    sys.exit(main())
