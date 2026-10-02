# STFS container notes

Implemented in `tools/stfs_extract.py`. Run:

    python tools/stfs_extract.py "7F40FC5F96CDEA326B02027C03A249463814C12D58 (my drive)" extracted

## This package

- Magic `LIVE`, header size `0xAD0E`, so the first hash table is at `0xB000`.
- Volume descriptor at `0x379`: block separation `0x01` (**female**, one hash-table copy), file table = 5 blocks starting at block 0, 1813 allocated blocks.
- 299 file table entries. All files are stored in consecutive blocks, and every block passes its SHA-1 check against the level-0 hash table.

## Format summary

- Header fields are big-endian. The exceptions are the volume descriptor's file-table block count (u16) and block number (u24), which are little-endian.
- Blocks are 0x1000 bytes. Each level-0 hash table covers 0xAA data blocks, and each level-1 table covers 0x70E4. Tables are interleaved with the data, so a logical block number has to be mapped to a physical one (`backing_block_num`).
- Hash entry (0x18 bytes): SHA-1[20], status u8, next block u24 BE.
- File entry (0x40 bytes): name[0x28], flags (bits 0-5 = name length, bit 6 = consecutive, bit 7 = directory), blocks u24 LE @0x29, start block u24 LE @0x2F, parent index s16 BE @0x32 (-1 = root), size u32 BE @0x34.
- Male packages (two hash-table copies) are not supported yet. This package doesn't need them.
