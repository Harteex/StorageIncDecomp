"""LZX decompressor for XNA content (.xnb) files.

Port of the libmspack / MonoGame LzxDecoder algorithm. XNB uses a 64 KB window (16 bits),
no E8 (intel) translation, and splits the stream into chunks that each start on a fresh
16-bit-aligned bitstream; decoder state (window, repeat offsets, tree lengths, current
block) carries across chunks.
"""

MIN_MATCH = 2
NUM_CHARS = 256
BLOCKTYPE_VERBATIM, BLOCKTYPE_ALIGNED, BLOCKTYPE_UNCOMPRESSED = 1, 2, 3
PRETREE_NUM_ELEMENTS = 20
ALIGNED_NUM_ELEMENTS = 8
NUM_PRIMARY_LENGTHS = 7
NUM_SECONDARY_LENGTHS = 249

EXTRA_BITS = []
_j = 0
for _i in range(0, 52, 2):
    EXTRA_BITS += [_j, _j]
    if _i != 0 and _j < 17:
        _j += 1
POSITION_BASE = []
_j = 0
for _eb in EXTRA_BITS:
    POSITION_BASE.append(_j)
    _j += 1 << _eb


class LzxError(Exception):
    pass


class _Bits:
    """MSB-first reader over 16-bit little-endian words."""

    def __init__(self, data, pos=0):
        self.data = data
        self.pos = pos
        self.buf = 0
        self.n = 0

    def ensure(self, k):
        while self.n < k:
            d, p = self.data, self.pos
            lo = d[p] if p < len(d) else 0
            hi = d[p + 1] if p + 1 < len(d) else 0
            self.pos = p + 2
            self.buf = (self.buf << 16) | (hi << 8) | lo
            self.n += 16

    def peek(self, k):
        self.ensure(k)
        return (self.buf >> (self.n - k)) & ((1 << k) - 1)

    def remove(self, k):
        self.n -= k
        self.buf &= (1 << self.n) - 1

    def read(self, k):
        if k == 0:
            return 0
        v = self.peek(k)
        self.remove(k)
        return v

    def reset(self):
        self.buf = 0
        self.n = 0


class _Huffman:
    """Canonical Huffman decoder using a full lookup table of 2^maxlen entries."""

    def __init__(self, lengths):
        self.maxlen = max(lengths) if lengths else 0
        if self.maxlen == 0:
            self.table = None
            return
        size = 1 << self.maxlen
        table = [None] * size
        code = 0
        for length in range(1, self.maxlen + 1):
            for sym, l in enumerate(lengths):
                if l != length:
                    continue
                shift = self.maxlen - length
                start = code << shift
                if start + (1 << shift) > size:
                    raise LzxError("over-subscribed Huffman table")
                entry = (sym, length)
                for k in range(start, start + (1 << shift)):
                    table[k] = entry
                code += 1
            code <<= 1
        self.table = table

    def decode(self, bits):
        if self.table is None:
            raise LzxError("decode from empty Huffman table")
        entry = self.table[bits.peek(self.maxlen)]
        if entry is None:
            raise LzxError("invalid Huffman code")
        bits.remove(entry[1])
        return entry[0]


class LzxDecoder:
    def __init__(self, window_bits=16):
        if not 15 <= window_bits <= 21:
            raise LzxError("unsupported window size")
        self.window_size = 1 << window_bits
        self.window = bytearray(self.window_size)
        self.window_posn = 0
        posn_slots = {20: 42, 21: 50}.get(window_bits, window_bits << 1)
        self.main_elements = NUM_CHARS + (posn_slots << 3)
        self.R0 = self.R1 = self.R2 = 1
        self.maintree_len = [0] * self.main_elements
        self.length_len = [0] * NUM_SECONDARY_LENGTHS
        self.header_read = False
        self.block_type = 0
        self.block_length = 0
        self.block_remaining = 0
        self.maintree = self.lengthtree = self.alignedtree = None

    def _read_lens(self, bits, lens, first, last):
        pretree = _Huffman([bits.read(4) for _ in range(PRETREE_NUM_ELEMENTS)])
        x = first
        while x < last:
            z = pretree.decode(bits)
            if z == 17:
                for _ in range(bits.read(4) + 4):
                    lens[x] = 0
                    x += 1
            elif z == 18:
                for _ in range(bits.read(5) + 20):
                    lens[x] = 0
                    x += 1
            elif z == 19:
                y = bits.read(1) + 4
                z = (lens[x] - pretree.decode(bits)) % 17
                for _ in range(y):
                    lens[x] = z
                    x += 1
            else:
                lens[x] = (lens[x] - z) % 17
                x += 1

    def _read_block_header(self, bits):
        if self.block_type == BLOCKTYPE_UNCOMPRESSED:
            if self.block_length & 1:
                bits.pos += 1
            bits.reset()
        self.block_type = bits.read(3)
        hi = bits.read(16)
        lo = bits.read(8)
        self.block_remaining = self.block_length = (hi << 8) | lo

        if self.block_type == BLOCKTYPE_ALIGNED:
            self.alignedtree = _Huffman([bits.read(3) for _ in range(ALIGNED_NUM_ELEMENTS)])
        if self.block_type in (BLOCKTYPE_VERBATIM, BLOCKTYPE_ALIGNED):
            self._read_lens(bits, self.maintree_len, 0, NUM_CHARS)
            self._read_lens(bits, self.maintree_len, NUM_CHARS, self.main_elements)
            self.maintree = _Huffman(self.maintree_len)
            self._read_lens(bits, self.length_len, 0, NUM_SECONDARY_LENGTHS)
            self.lengthtree = _Huffman(self.length_len) if max(self.length_len) else None
        elif self.block_type == BLOCKTYPE_UNCOMPRESSED:
            # Align to a byte boundary (consumes 1-16 bits), then read raw R0..R2.
            bits.ensure(16)
            if bits.n > 16:
                bits.pos -= 2
            bits.reset()
            d, p = bits.data, bits.pos
            self.R0 = int.from_bytes(d[p:p + 4], "little")
            self.R1 = int.from_bytes(d[p + 4:p + 8], "little")
            self.R2 = int.from_bytes(d[p + 8:p + 12], "little")
            bits.pos = p + 12
        else:
            raise LzxError(f"invalid block type {self.block_type}")

    def decompress(self, data, out_len):
        """Decompress one XNB chunk, returning `out_len` bytes."""
        bits = _Bits(data)
        if not self.header_read:
            if bits.read(1):
                bits.read(16)
                bits.read(16)  # intel file size; E8 translation is unused by XNA
            self.header_read = True

        win, wsize = self.window, self.window_size
        start = self.window_posn
        produced_total = 0
        while produced_total < out_len:
            if self.block_remaining == 0:
                self._read_block_header(bits)
            this_run = min(self.block_remaining, out_len - produced_total)
            pos = self.window_posn & (wsize - 1)
            end = pos + this_run

            if self.block_type == BLOCKTYPE_UNCOMPRESSED:
                p = bits.pos
                win[pos:end] = data[p:p + this_run]
                bits.pos = p + this_run
                pos = end
            else:
                aligned = self.block_type == BLOCKTYPE_ALIGNED
                main, lentree = self.maintree, self.lengthtree
                R0, R1, R2 = self.R0, self.R1, self.R2
                while pos < end:
                    sym = main.decode(bits)
                    if sym < NUM_CHARS:
                        win[pos] = sym
                        pos += 1
                        continue
                    sym -= NUM_CHARS
                    match_length = sym & NUM_PRIMARY_LENGTHS
                    if match_length == NUM_PRIMARY_LENGTHS:
                        match_length += lentree.decode(bits)
                    match_length += MIN_MATCH
                    slot = sym >> 3
                    if slot > 2:
                        extra = EXTRA_BITS[slot]
                        off = POSITION_BASE[slot] - 2
                        if aligned and extra >= 3:
                            if extra > 3:
                                off += bits.read(extra - 3) << 3
                            off += self.alignedtree.decode(bits)
                        else:
                            off += bits.read(extra)
                        R2, R1, R0 = R1, R0, off
                    elif slot == 0:
                        off = R0
                    elif slot == 1:
                        off = R1
                        R1, R0 = R0, off
                    else:
                        off = R2
                        R2, R0 = R0, off
                    if pos + match_length > wsize:
                        raise LzxError("match runs past end of window")
                    src = pos - off
                    if src >= 0 and off >= match_length:
                        win[pos:pos + match_length] = win[src:src + match_length]
                    else:
                        for k in range(match_length):
                            win[pos + k] = win[(src + k) % wsize]
                    pos += match_length
                self.R0, self.R1, self.R2 = R0, R1, R2

            run = pos - (self.window_posn & (wsize - 1))
            self.window_posn = pos & (wsize - 1) if pos >= wsize else pos
            self.block_remaining -= run
            if self.block_remaining < 0:
                raise LzxError("match overran block")
            produced_total += run

        if produced_total != out_len:
            raise LzxError(f"chunk produced {produced_total} bytes, expected {out_len}")
        s = start & (wsize - 1)
        if s + out_len <= wsize:
            return bytes(win[s:s + out_len])
        return bytes(win[s:]) + bytes(win[:out_len - (wsize - s)])


def decompress_xnb_payload(data, decompressed_size):
    """Decompress the LZX body of an XNB file (bytes after the 14-byte header)."""
    dec = LzxDecoder(16)
    out = []
    pos = 0
    while pos < len(data):
        hi, lo = data[pos], data[pos + 1]
        if hi == 0xFF:
            frame_size = (lo << 8) | data[pos + 2]
            block_size = (data[pos + 3] << 8) | data[pos + 4]
            pos += 5
        else:
            frame_size = 0x8000
            block_size = (hi << 8) | lo
            pos += 2
        if block_size == 0 or frame_size == 0:
            break
        out.append(dec.decompress(data[pos:pos + block_size], frame_size))
        pos += block_size
    result = b"".join(out)
    if len(result) != decompressed_size:
        raise LzxError(f"decompressed {len(result)} bytes, expected {decompressed_size}")
    return result
