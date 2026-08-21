using System.Buffers;
using System.Text;
using McProtoNet.NBT;

namespace McProtoNet.Tests.Nbt;

/// <summary>
/// Modified UTF-8 (Java DataOutput.writeUTF / DataInput.readUTF) against a naive reference
/// implementation, hand-computed golden vectors, and the malformed-input matrix.
/// </summary>
public class ModifiedUtf8Tests
{
    // ── Naive reference implementation ────────────────────────────────────────

    /// <summary>Encodes one UTF-16 unit at a time, straight from the writeUTF spec.</summary>
    private static byte[] ReferenceEncode(string value)
    {
        var bytes = new List<byte>();
        foreach (var c in value)
            if (c is >= '\u0001' and <= '\u007F')
            {
                bytes.Add((byte)c);
            }
            else if (c <= '\u07FF')
            {
                bytes.Add((byte)(0xC0 | (c >> 6)));
                bytes.Add((byte)(0x80 | (c & 0x3F)));
            }
            else
            {
                bytes.Add((byte)(0xE0 | (c >> 12)));
                bytes.Add((byte)(0x80 | ((c >> 6) & 0x3F)));
                bytes.Add((byte)(0x80 | (c & 0x3F)));
            }

        return bytes.ToArray();
    }

    /// <summary>Decodes with the byte switch of readUTF; returns null for malformed input.</summary>
    private static string? ReferenceDecode(byte[] bytes, bool allowFourByte = true)
    {
        var sb = new StringBuilder();
        var i = 0;
        while (i < bytes.Length)
        {
            int b = bytes[i];
            switch (b >> 4)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                    sb.Append((char)b);
                    i++;
                    break;
                case 12:
                case 13:
                    if (i + 1 >= bytes.Length || (bytes[i + 1] & 0xC0) != 0x80) return null;
                    sb.Append((char)(((b & 0x1F) << 6) | (bytes[i + 1] & 0x3F)));
                    i += 2;
                    break;
                case 14:
                    if (i + 2 >= bytes.Length ||
                        (bytes[i + 1] & 0xC0) != 0x80 || (bytes[i + 2] & 0xC0) != 0x80) return null;
                    sb.Append((char)(((b & 0x0F) << 12) | ((bytes[i + 1] & 0x3F) << 6) | (bytes[i + 2] & 0x3F)));
                    i += 3;
                    break;
                case 15:
                    if (!allowFourByte || b > 0xF4 || i + 3 >= bytes.Length) return null;
                    if ((bytes[i + 1] & 0xC0) != 0x80 || (bytes[i + 2] & 0xC0) != 0x80 ||
                        (bytes[i + 3] & 0xC0) != 0x80) return null;
                    var scalar = ((b & 0x07) << 18) | ((bytes[i + 1] & 0x3F) << 12) |
                                 ((bytes[i + 2] & 0x3F) << 6) | (bytes[i + 3] & 0x3F);
                    if (scalar is < 0x10000 or > 0x10FFFF) return null;
                    sb.Append(char.ConvertFromUtf32(scalar));
                    i += 4;
                    break;
                default:
                    return null;
            }
        }

        return sb.ToString();
    }

    private static byte[] Encode(string value)
    {
        var buffer = new byte[ModifiedUtf8.GetByteCount(value)];
        var written = ModifiedUtf8.GetBytes(value, buffer);
        Assert.Equal(buffer.Length, written);
        return buffer;
    }

    // ── ASCII lengths ─────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(15)]
    [InlineData(16)]
    [InlineData(17)]
    [InlineData(31)]
    [InlineData(32)]
    [InlineData(33)]
    [InlineData(63)]
    [InlineData(64)]
    [InlineData(65)]
    [InlineData(512)]
    [InlineData(513)]
    public void Ascii_RoundTrips_AtEveryVectorBoundary(int length)
    {
        var value = string.Concat(Enumerable.Range(0, length).Select(i => (char)('a' + i % 26)));

        var encoded = Encode(value);

        Assert.Equal(length, encoded.Length);
        Assert.Equal(ReferenceEncode(value), encoded);
        Assert.Equal(value, ModifiedUtf8.GetString(encoded));
        Assert.Equal(length, ModifiedUtf8.GetCharCount(encoded));
    }

    // ── NUL handling ──────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0)]
    [InlineData(15)]
    [InlineData(16)]
    [InlineData(17)]
    [InlineData(31)]
    public void Nul_IsWrittenAsC080_AtAnyPosition(int position)
    {
        var chars = Enumerable.Repeat('x', 32).ToArray();
        chars[position] = '\0';
        var value = new string(chars);

        var encoded = Encode(value);

        Assert.Equal(33, encoded.Length);
        Assert.Equal(0xC0, encoded[position]);
        Assert.Equal(0x80, encoded[position + 1]);
        Assert.Equal(value, ModifiedUtf8.GetString(encoded));
    }

    [Fact]
    public void Nul_AtLastPosition_RoundTrips()
    {
        const string value = "abc\0";

        var encoded = Encode(value);

        Assert.Equal(new byte[] { 0x61, 0x62, 0x63, 0xC0, 0x80 }, encoded);
        Assert.Equal(value, ModifiedUtf8.GetString(encoded));
    }

    [Fact]
    public void AllNul_RoundTrips()
    {
        var value = new string('\0', 64);

        var encoded = Encode(value);

        Assert.Equal(128, encoded.Length);
        Assert.All(Enumerable.Range(0, 64), i =>
        {
            Assert.Equal(0xC0, encoded[i * 2]);
            Assert.Equal(0x80, encoded[i * 2 + 1]);
        });
        Assert.Equal(value, ModifiedUtf8.GetString(encoded));
    }

    [Fact]
    public void RawZeroByte_DecodesToNul()
    {
        Assert.Equal("a\0b", ModifiedUtf8.GetString([0x61, 0x00, 0x62]));
        Assert.True(ModifiedUtf8.IsValid([0x00]));
    }

    // ── BMP boundaries ────────────────────────────────────────────────────────

    [Theory]
    [InlineData('\u0080', new byte[] { 0xC2, 0x80 })]
    [InlineData('\u00E9', new byte[] { 0xC3, 0xA9 })]
    [InlineData('\u07FF', new byte[] { 0xDF, 0xBF })]
    [InlineData('\u0800', new byte[] { 0xE0, 0xA0, 0x80 })]
    [InlineData('\uD7FF', new byte[] { 0xED, 0x9F, 0xBF })]
    [InlineData('\uFFFF', new byte[] { 0xEF, 0xBF, 0xBF })]
    public void BmpBoundaries_MatchTheSpec(char value, byte[] expected)
    {
        var text = value.ToString();

        Assert.Equal(expected, Encode(text));
        Assert.Equal(text, ModifiedUtf8.GetString(expected));
    }

    // ── Surrogates ────────────────────────────────────────────────────────────

    [Fact]
    public void SurrogatePair_IsWrittenAsTwoThreeByteSequences()
    {
        const string value = "😀";

        var encoded = Encode(value);

        Assert.Equal(new byte[] { 0xED, 0xA0, 0xBD, 0xED, 0xB8, 0x80 }, encoded);
        Assert.Equal(value, ModifiedUtf8.GetString(encoded));
    }

    [Theory]
    [InlineData('\uD83D', new byte[] { 0xED, 0xA0, 0xBD })]
    [InlineData('\uDE00', new byte[] { 0xED, 0xB8, 0x80 })]
    public void LoneSurrogate_RoundTrips(char value, byte[] expected)
    {
        var text = value.ToString();

        Assert.Equal(expected, Encode(text));
        Assert.Equal(text, ModifiedUtf8.GetString(expected));
    }

    // ── Mixed runs ────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("abc😀def")]
    [InlineData("한글")]
    [InlineData("Minecraft — привет")]
    public void MixedRuns_MatchTheReference(string value)
    {
        var encoded = Encode(value);

        Assert.Equal(ReferenceEncode(value), encoded);
        Assert.Equal(value, ModifiedUtf8.GetString(encoded));
        Assert.Equal(value.Length, ModifiedUtf8.GetCharCount(encoded));
    }

    [Fact]
    public void LongNonAsciiRun_MatchesTheReference()
    {
        var value = "a" + new string('я', 100) + "b";

        var encoded = Encode(value);

        Assert.Equal(202, encoded.Length);
        Assert.Equal(ReferenceEncode(value), encoded);
        Assert.Equal(value, ModifiedUtf8.GetString(encoded));
    }

    // ── Golden vectors, as Java DataOutputStream.writeUTF produces them ───────

    public static TheoryData<string, byte[]> GoldenVectors => new()
    {
        { "", [] },
        { "Hello", [0x48, 0x65, 0x6C, 0x6C, 0x6F] },
        { "ab\0c", [0x61, 0x62, 0xC0, 0x80, 0x63] },
        { "€", [0xE2, 0x82, 0xAC] },
        { "한글", [0xED, 0x95, 0x9C, 0xEA, 0xB8, 0x80] },
        { "😀", [0xED, 0xA0, 0xBD, 0xED, 0xB8, 0x80] }
    };

    [Theory]
    [MemberData(nameof(GoldenVectors))]
    public void GoldenVectors_EncodeAndDecodeExactly(string value, byte[] expected)
    {
        Assert.Equal(expected.Length, ModifiedUtf8.GetByteCount(value));
        Assert.Equal(expected, Encode(value));
        Assert.Equal(value, ModifiedUtf8.GetString(expected));
    }

    // ── Malformed input ───────────────────────────────────────────────────────

    [Theory]
    [InlineData(new byte[] { 0x80 })]
    [InlineData(new byte[] { 0xC3 })]
    [InlineData(new byte[] { 0xC3, 0x41 })]
    [InlineData(new byte[] { 0xE2, 0x82 })]
    [InlineData(new byte[] { 0xF5, 0x80, 0x80, 0x80 })]
    [InlineData(new byte[] { 0xFE })]
    [InlineData(new byte[] { 0xFF })]
    [InlineData(new byte[] { 0x61, 0x9F, 0x62 })]
    public void MalformedInput_IsRejected(byte[] bytes)
    {
        Assert.False(ModifiedUtf8.IsValid(bytes));
        Assert.Throws<NbtFormatException>(() => ModifiedUtf8.GetString(bytes));
        Assert.Null(ReferenceDecode(bytes));

        var buffer = new char[ModifiedUtf8.GetMaxCharCount(bytes.Length) + 4];
        Assert.Equal(OperationStatus.InvalidData,
            ModifiedUtf8.ToUtf16(bytes, buffer, out _, out _));
    }

    [Fact]
    public void FourByteSequence_LenientAccepts_StrictRejects()
    {
        byte[] bytes = [0xF0, 0x9F, 0x98, 0x80];
        var chars = new char[8];

        Assert.Equal(OperationStatus.Done,
            ModifiedUtf8.ToUtf16(bytes, chars, out var read, out var written));
        Assert.Equal(4, read);
        Assert.Equal(2, written);
        Assert.Equal("😀", new string(chars, 0, written));
        Assert.Equal("😀", ModifiedUtf8.GetString(bytes));
        Assert.True(ModifiedUtf8.IsValid(bytes));

        Assert.Equal(OperationStatus.InvalidData,
            ModifiedUtf8.ToUtf16(bytes, chars, out _, out _, allowFourByteSequences: false));
    }

    [Fact]
    public void FourByteSequence_InTheMiddle_StrictStopsAtIt()
    {
        byte[] bytes = [0x61, 0x62, 0xF0, 0x9F, 0x98, 0x80, 0x63];
        var chars = new char[16];

        Assert.Equal(OperationStatus.InvalidData,
            ModifiedUtf8.ToUtf16(bytes, chars, out var read, out var written, false));
        Assert.Equal(2, read);
        Assert.Equal(2, written);

        Assert.Equal("ab😀c", ModifiedUtf8.GetString(bytes));
    }

    [Theory]
    [InlineData(new byte[] { 0xC1, 0x81 }, "A")]
    [InlineData(new byte[] { 0xE0, 0x80, 0x80 }, "\0")]
    [InlineData(new byte[] { 0xC0, 0x80 }, "\0")]
    [InlineData(new byte[] { 0xE0, 0x81, 0x81 }, "A")]
    public void OverlongForms_AreAcceptedLikeJava(byte[] bytes, string expected)
    {
        Assert.True(ModifiedUtf8.IsValid(bytes));
        Assert.Equal(expected, ModifiedUtf8.GetString(bytes));
        Assert.Equal(expected, ReferenceDecode(bytes));
    }

    [Fact]
    public void EveryLeadByteFromF5ToFF_IsMalformed()
    {
        for (var lead = 0xF5; lead <= 0xFF; lead++)
        {
            byte[] bytes = [(byte)lead, 0x80, 0x80, 0x80];
            Assert.False(ModifiedUtf8.IsValid(bytes));
            Assert.Throws<NbtFormatException>(() => ModifiedUtf8.GetString(bytes));
        }
    }

    // ── Buffer discipline ─────────────────────────────────────────────────────

    [Theory]
    [InlineData("abcdefgh")]
    [InlineData("русский")]
    [InlineData("ab😀cd")]
    [InlineData("a\0b\0c")]
    public void FromUtf16_NeverWritesPastTheDestination(string value)
    {
        var full = Encode(value);

        for (var size = 0; size < full.Length; size++)
        {
            var buffer = new byte[full.Length + 8];
            buffer.AsSpan().Fill(0xCC);

            var status = ModifiedUtf8.FromUtf16(value, buffer.AsSpan(0, size), out var charsRead,
                out var bytesWritten);

            Assert.Equal(OperationStatus.DestinationTooSmall, status);
            Assert.True(bytesWritten <= size);
            Assert.Equal(full.AsSpan(0, bytesWritten).ToArray(), buffer.AsSpan(0, bytesWritten).ToArray());
            Assert.All(Enumerable.Range(bytesWritten, buffer.Length - bytesWritten),
                i => Assert.Equal(0xCC, buffer[i]));
            Assert.Equal(full.AsSpan(0, bytesWritten).ToArray(), Encode(value[..charsRead]));
        }
    }

    [Fact]
    public void ToUtf16_ReportsDestinationTooSmall_WithoutOverrun()
    {
        const string value = "ab😀cd\0e";
        var encoded = Encode(value);

        for (var size = 0; size < value.Length; size++)
        {
            var buffer = new char[value.Length + 4];
            buffer.AsSpan().Fill('\uCCCC');

            var status = ModifiedUtf8.ToUtf16(encoded, buffer.AsSpan(0, size), out _, out var charsWritten);

            Assert.Equal(OperationStatus.DestinationTooSmall, status);
            Assert.True(charsWritten <= size);
            Assert.Equal(value[..charsWritten], new string(buffer, 0, charsWritten));
            Assert.All(Enumerable.Range(charsWritten, buffer.Length - charsWritten),
                i => Assert.Equal('\uCCCC', buffer[i]));
        }
    }

    [Fact]
    public void GetCharCount_CountsSurrogatePairs_BehindFourByteSequences()
    {
        byte[] bytes = [0x61, 0xF0, 0x9F, 0x98, 0x80, 0x62];

        Assert.Equal(4, ModifiedUtf8.GetCharCount(bytes));
        Assert.Equal("a😀b", ModifiedUtf8.GetString(bytes));
    }

    [Fact]
    public void GetCharCount_IsNeverBelowWhatDecodingWrites()
    {
        var random = new Random(20260824);

        for (var iteration = 0; iteration < 5000; iteration++)
        {
            var bytes = new byte[random.Next(0, 24)];
            random.NextBytes(bytes);

            var count = ModifiedUtf8.GetCharCount(bytes);
            Assert.True(count >= 0);

            var buffer = new char[Math.Max(count, 1) + 8];
            ModifiedUtf8.ToUtf16(bytes, buffer, out _, out var charsWritten);
            Assert.True(charsWritten <= count, $"count {count} < written {charsWritten}");
        }
    }

    [Fact]
    public void MaxCounts_BoundTheRealCounts()
    {
        Assert.Equal(0, ModifiedUtf8.GetMaxByteCount(0));
        Assert.Equal(30, ModifiedUtf8.GetMaxByteCount(10));
        Assert.Equal(10, ModifiedUtf8.GetMaxCharCount(10));
        Assert.Throws<ArgumentOutOfRangeException>(() => ModifiedUtf8.GetMaxByteCount(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => ModifiedUtf8.GetMaxCharCount(-1));

        const string value = "a\0р😀";
        Assert.True(ModifiedUtf8.GetByteCount(value) <= ModifiedUtf8.GetMaxByteCount(value.Length));
    }

    // ── Streaming ─────────────────────────────────────────────────────────────

    [Fact]
    public void Encoding_InRandomChunks_MatchesOneCall()
    {
        var random = new Random(20260821);
        var value = string.Concat(Enumerable.Range(0, 2000)
            .Select(i => i % 7 == 0 ? '\0' : i % 5 == 0 ? 'р' : i % 11 == 0 ? '\uD83D' : (char)('a' + i % 26)));
        var expected = Encode(value);

        var pieces = new List<byte>();
        ReadOnlySpan<char> rest = value;
        var buffer = new byte[64];
        while (!rest.IsEmpty)
        {
            var size = random.Next(3, buffer.Length + 1);
            var status = ModifiedUtf8.FromUtf16(rest, buffer.AsSpan(0, size), out var charsRead,
                out var bytesWritten);
            Assert.True(charsRead > 0);
            pieces.AddRange(buffer.AsSpan(0, bytesWritten).ToArray());
            rest = rest[charsRead..];
            if (status == OperationStatus.Done) Assert.True(rest.IsEmpty);
        }

        Assert.Equal(expected, pieces.ToArray());
    }

    [Fact]
    public void Decoding_InRandomChunks_MatchesOneCall()
    {
        var random = new Random(20260822);
        var value = string.Concat(Enumerable.Range(0, 1500)
            .Select(i => i % 7 == 0 ? '\0' : i % 5 == 0 ? 'р' : (char)('a' + i % 26)));
        var encoded = Encode(value);

        var text = new StringBuilder();
        var offset = 0;
        var buffer = new char[32];
        while (offset < encoded.Length)
        {
            var size = random.Next(1, buffer.Length + 1);
            var status = ModifiedUtf8.ToUtf16(encoded.AsSpan(offset), buffer.AsSpan(0, size),
                out var bytesRead, out var charsWritten);
            Assert.True(status is OperationStatus.Done or OperationStatus.DestinationTooSmall);
            Assert.True(bytesRead > 0);
            text.Append(buffer, 0, charsWritten);
            offset += bytesRead;
        }

        Assert.Equal(value, text.ToString());
    }

    // ── Fuzz ──────────────────────────────────────────────────────────────────

    [Fact]
    public void Fuzz_TenThousandStrings_MatchTheReference()
    {
        var random = new Random(20260823);

        for (var iteration = 0; iteration < 10_000; iteration++)
        {
            var length = random.Next(0, 40);
            var chars = new char[length];
            for (var i = 0; i < length; i++)
                chars[i] = random.Next(4) switch
                {
                    0 => (char)random.Next(0x0000, 0x0080),
                    1 => (char)random.Next(0x0080, 0x0800),
                    2 => (char)random.Next(0x0800, 0x10000),
                    _ => (char)random.Next(0xD800, 0xE000)
                };

            var value = new string(chars);
            var expected = ReferenceEncode(value);

            Assert.Equal(expected.Length, ModifiedUtf8.GetByteCount(value));

            var encoded = new byte[expected.Length];
            Assert.Equal(expected.Length, ModifiedUtf8.GetBytes(value, encoded));
            Assert.Equal(expected, encoded);

            Assert.Equal(value.Length, ModifiedUtf8.GetCharCount(encoded));
            Assert.True(ModifiedUtf8.IsValid(encoded));
            Assert.Equal(value, ModifiedUtf8.GetString(encoded));
            Assert.Equal(value, ReferenceDecode(encoded));
        }
    }
}
