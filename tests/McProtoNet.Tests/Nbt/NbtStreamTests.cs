using System.Buffers;
using McProtoNet.NBT;

namespace McProtoNet.Tests.Nbt;

/// <summary>
/// The Stream side of McProtoNet.NBT — <see cref="NbtReader" /> and <see cref="NbtWriter" /> —
/// plus the input-safety guards every reader shares: tag type range, declared lengths,
/// nesting depth, duplicate names.
/// </summary>
public class NbtStreamTests
{
    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>A stream that refuses to seek, to exercise the buffered skip path.</summary>
    private sealed class ForwardOnlyStream(byte[] data) : Stream
    {
        private readonly MemoryStream _inner = new(data);

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Flush() { }
        public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }

    private static NbtCompound BuildKitchenSink(string? name)
    {
        return new NbtCompound(name)
        {
            new NbtByte("byte", 0xAB),
            new NbtShort("short", -1234),
            new NbtInt("int", int.MinValue),
            new NbtLong("long", long.MaxValue),
            new NbtFloat("float", 3.14f),
            new NbtDouble("double", 2.71828),
            new NbtString("string", "Minecraft — привет\0😀"),
            new NbtByteArray("bytes", [1, 2, 3, 255]),
            new NbtIntArray("ints", [int.MinValue, -1, 0, 1, int.MaxValue]),
            new NbtLongArray("longs", [long.MinValue, -1L, 0L, 1L, long.MaxValue]),
            new NbtList("intList", [new NbtInt(10), new NbtInt(20), new NbtInt(30)]),
            new NbtList("emptyList"),
            new NbtCompound("nested")
            {
                new NbtString("deep", "value"),
                new NbtList("deepList", [new NbtCompound { new NbtByte("flag", 1) }])
            }
        };
    }

    private static byte[] WriteToStream(NbtTag tag, bool writeRootName)
    {
        var stream = new MemoryStream();
        NbtWriter.WriteTag(stream, tag, writeRootName);
        return stream.ToArray();
    }

    private static byte[] WriteToBuffer(NbtTag tag, bool writeRootName)
    {
        var buffer = new ArrayBufferWriter<byte>();
        NbtBufferWriter.WriteTag(buffer, tag, writeRootName);
        return buffer.WrittenSpan.ToArray();
    }

    private static ReadOnlySequence<byte> Segmented(byte[] data, int segmentSize)
    {
        var first = new Segment(data.AsMemory(0, Math.Min(segmentSize, data.Length)));
        var last = first;
        for (var offset = segmentSize; offset < data.Length; offset += segmentSize)
            last = last.Append(data.AsMemory(offset, Math.Min(segmentSize, data.Length - offset)));
        return new ReadOnlySequence<byte>(first, 0, last, last.Memory.Length);
    }

    private sealed class Segment : ReadOnlySequenceSegment<byte>
    {
        public Segment(ReadOnlyMemory<byte> memory) => Memory = memory;

        public Segment Append(ReadOnlyMemory<byte> memory)
        {
            var next = new Segment(memory) { RunningIndex = RunningIndex + Memory.Length };
            Next = next;
            return next;
        }
    }

    /// <summary>A chain of <paramref name="depth" /> compounds nested one inside the next.</summary>
    private static byte[] NestedCompounds(int depth)
    {
        var bytes = new List<byte> { (byte)NbtTagType.Compound, 0x00, 0x00 };
        for (var i = 1; i < depth; i++)
            bytes.AddRange([(byte)NbtTagType.Compound, 0x00, 0x01, (byte)'c']);
        for (var i = 0; i < depth; i++)
            bytes.Add((byte)NbtTagType.End);
        return bytes.ToArray();
    }

    private static NbtTag? ReadFromSpan(byte[] bytes, bool readRootName = true)
    {
        var reader = new NbtSpanReader(bytes);
        return reader.ReadAsTag<NbtTag>(readRootName);
    }

    private static NbtTag? ReadFromSequence(byte[] bytes, bool readRootName = true)
    {
        var sequence = new SequenceReader<byte>(Segmented(bytes, 1));
        return NbtSequenceReader.ReadTag(ref sequence, readRootName);
    }

    // ── Stream round-trips ────────────────────────────────────────────────────

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void StreamWriter_And_StreamReader_RoundTrip(bool writeRootName)
    {
        var original = BuildKitchenSink(writeRootName ? "root" : null);

        var bytes = WriteToStream(original, writeRootName);
        var restored = NbtReader.ReadTag(new MemoryStream(bytes), writeRootName);

        Assert.Equal(original.ToString(), restored!.ToString());
        Assert.Equal(bytes, WriteToBuffer(original, writeRootName));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void StreamBytes_ReadBack_ByEveryReader(bool writeRootName)
    {
        var original = BuildKitchenSink(writeRootName ? "root" : null);
        var bytes = WriteToStream(original, writeRootName);

        Assert.Equal(original.ToString(), ReadFromSpan(bytes, writeRootName)!.ToString());
        Assert.Equal(original.ToString(), ReadFromSequence(bytes, writeRootName)!.ToString());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void NonCompoundRoot_RoundTrips(bool writeRootName)
    {
        var original = new NbtString(writeRootName ? "root" : null, "chat component as a bare string");

        var bytes = WriteToStream(original, writeRootName);
        var restored = NbtReader.ReadTag(new MemoryStream(bytes), writeRootName);

        Assert.Equal(original.Value, Assert.IsType<NbtString>(restored).Value);
        Assert.Equal(original.Value, Assert.IsType<NbtString>(ReadFromSpan(bytes, writeRootName)).Value);
        Assert.Equal(original.Value, Assert.IsType<NbtString>(ReadFromSequence(bytes, writeRootName)).Value);
    }

    [Fact]
    public void NonCompoundRoot_IsWalkableByThePullReader()
    {
        var bytes = WriteToStream(new NbtInt("answer", 42), true);

        var reader = new NbtReader(new MemoryStream(bytes));

        Assert.True(reader.ReadToFollowing());
        Assert.Equal(NbtTagType.Int, reader.TagType);
        Assert.Equal("answer", reader.RootName);
        Assert.Equal(42, reader.ReadValueAs<int>());
        Assert.False(reader.ReadToFollowing());
        Assert.True(reader.IsAtStreamEnd);
    }

    [Fact]
    public void PullReader_ReadAsTag_RebuildsTheWholeTree()
    {
        var original = BuildKitchenSink("root");
        var bytes = WriteToStream(original, true);

        var reader = new NbtReader(new MemoryStream(bytes));
        var restored = reader.ReadAsTag();

        Assert.Equal(original.ToString(), restored.ToString());
    }

    [Fact]
    public void PullReader_NetworkRoot_HasNoName()
    {
        var bytes = WriteToStream(BuildKitchenSink(null), false);

        var reader = new NbtReader(new MemoryStream(bytes), readRootName: false);
        var restored = reader.ReadAsTag();

        Assert.Null(restored.Name);
        Assert.Equal(13, Assert.IsType<NbtCompound>(restored).Count);
    }

    [Fact]
    public void PullReader_Skip_WorksOnANonSeekableStream()
    {
        var bytes = WriteToStream(BuildKitchenSink("root"), true);

        var reader = new NbtReader(new ForwardOnlyStream(bytes));

        Assert.True(reader.ReadToFollowing("deep"));
        Assert.Equal("value", reader.ReadValueAs<string>());
    }

    [Fact]
    public void PullWriter_ProducesTheSameBytesAsTheTagWriter()
    {
        var expected = WriteToStream(new NbtCompound("root") { new NbtInt("a", 1), new NbtString("b", "two") },
            true);

        var stream = new MemoryStream();
        var writer = new NbtWriter(stream, "root");
        writer.WriteInt("a", 1);
        writer.WriteString("b", "two");
        writer.Finish();

        Assert.Equal(expected, stream.ToArray());
    }

    [Fact]
    public void PullWriter_NetworkRoot_OmitsTheName()
    {
        var stream = new MemoryStream();
        var writer = new NbtWriter(stream);
        writer.WriteInt("a", 1);
        writer.Finish();

        Assert.Equal(new byte[] { 0x0A, 0x03, 0x00, 0x01, (byte)'a', 0, 0, 0, 1, 0x00 }, stream.ToArray());
    }

    // ── Long strings ──────────────────────────────────────────────────────────

    [Theory]
    [InlineData(1)]
    [InlineData(255)]
    [InlineData(256)]
    [InlineData(257)]
    [InlineData(32767)]
    [InlineData(32768)]
    [InlineData(65535)]
    public void LongStrings_RoundTripThroughEveryPath(int length)
    {
        var value = string.Concat(Enumerable.Range(0, length).Select(i => (char)('a' + i % 26)));
        var tag = new NbtString("s", value);

        var streamBytes = WriteToStream(tag, true);
        var bufferBytes = WriteToBuffer(tag, true);

        Assert.Equal(streamBytes, bufferBytes);
        Assert.Equal(value, Assert.IsType<NbtString>(NbtReader.ReadTag(new MemoryStream(streamBytes))).Value);
        Assert.Equal(value, Assert.IsType<NbtString>(ReadFromSpan(streamBytes)).Value);
        Assert.Equal(value, Assert.IsType<NbtString>(ReadFromSequence(streamBytes)).Value);
    }

    // ── Large arrays over a stream ────────────────────────────────────────────

    [Theory]
    [InlineData(1)]
    [InlineData(65535)]
    [InlineData(65536)]
    [InlineData(65537)]
    [InlineData(200_000)]
    public void LargeArrays_GrowCorrectly_OnASeekableAndAForwardOnlyStream(int length)
    {
        var bytes = Enumerable.Range(0, length).Select(i => (byte)(i * 31)).ToArray();
        var ints = Enumerable.Range(0, length).Select(i => i * -7).ToArray();
        var longs = Enumerable.Range(0, length).Select(i => (long)i * -1_000_000_007).ToArray();
        var tag = new NbtCompound("root")
        {
            new NbtByteArray("b", bytes),
            new NbtIntArray("i", ints),
            new NbtLongArray("l", longs)
        };

        var encoded = WriteToStream(tag, true);

        foreach (var stream in new Stream[] { new MemoryStream(encoded), new ForwardOnlyStream(encoded) })
        {
            var restored = Assert.IsType<NbtCompound>(NbtReader.ReadTag(stream));
            Assert.Equal(bytes, restored.Get<NbtByteArray>("b")!.Value);
            Assert.Equal(ints, restored.Get<NbtIntArray>("i")!.Value);
            Assert.Equal(longs, restored.Get<NbtLongArray>("l")!.Value);
        }

        var fromSpan = Assert.IsType<NbtCompound>(ReadFromSpan(encoded));
        Assert.Equal(bytes, fromSpan.Get<NbtByteArray>("b")!.Value);
        Assert.Equal(ints, fromSpan.Get<NbtIntArray>("i")!.Value);
        Assert.Equal(longs, fromSpan.Get<NbtLongArray>("l")!.Value);
    }

    [Fact]
    public void TruncatedArray_OnAForwardOnlyStream_Throws()
    {
        var tag = new NbtCompound("root") { new NbtLongArray("l", new long[100_000]) };
        var encoded = WriteToStream(tag, true);

        Assert.Throws<EndOfStreamException>(() =>
            NbtReader.ReadTag(new ForwardOnlyStream(encoded[..(encoded.Length / 2)])));
    }

    [Fact]
    public void String_LongerThan65535Bytes_IsRejected()
    {
        var tag = new NbtString("s", new string('a', 65536));

        Assert.Throws<NbtFormatException>(() => WriteToBuffer(tag, true));
        Assert.Throws<NbtFormatException>(() => WriteToStream(tag, true));
    }

    [Fact]
    public void MultiByteString_CountsBytesNotChars_AgainstTheLimit()
    {
        Assert.Throws<NbtFormatException>(() => WriteToBuffer(new NbtString("s", new string('я', 32768)), true));

        var justFits = new NbtString("s", new string('я', 32767));
        Assert.Equal(65534, ModifiedUtf8.GetByteCount(justFits.Value));
        Assert.Equal(justFits.Value, Assert.IsType<NbtString>(ReadFromSpan(WriteToBuffer(justFits, true))).Value);
    }

    // ── Input safety ──────────────────────────────────────────────────────────

    [Fact]
    public void TagTypeOutOfRange_IsRejectedByEveryReader()
    {
        byte[] bytes = [0x0D, 0x00, 0x00];

        Assert.Throws<NbtFormatException>(() => ReadFromSpan(bytes));
        Assert.Throws<NbtFormatException>(() => ReadFromSequence(bytes));
        Assert.Throws<NbtFormatException>(() => NbtReader.ReadTag(new MemoryStream(bytes)));
    }

    [Fact]
    public void NegativeArrayLength_IsRejectedByEveryReader()
    {
        byte[] bytes = [(byte)NbtTagType.ByteArray, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF];

        Assert.Throws<NbtFormatException>(() => ReadFromSpan(bytes));
        Assert.Throws<NbtFormatException>(() => ReadFromSequence(bytes));
        Assert.Throws<NbtFormatException>(() => NbtReader.ReadTag(new MemoryStream(bytes)));
    }

    [Theory]
    [InlineData(NbtTagType.ByteArray)]
    [InlineData(NbtTagType.IntArray)]
    [InlineData(NbtTagType.LongArray)]
    public void HugeClaimedLength_FailsWithoutAllocating(NbtTagType type)
    {
        byte[] bytes = [(byte)type, 0x00, 0x00, 0x7F, 0xFF, 0xFF, 0xFF];

        Assert.Throws<NbtFormatException>(() => ReadFromSpan(bytes));
        Assert.Throws<NbtFormatException>(() => ReadFromSequence(bytes));
        Assert.Throws<NbtFormatException>(() => NbtReader.ReadTag(new MemoryStream(bytes)));
    }

    [Fact]
    public void HugeClaimedListLength_FailsWithoutAllocating()
    {
        byte[] bytes =
            [(byte)NbtTagType.List, 0x00, 0x00, (byte)NbtTagType.Compound, 0x7F, 0xFF, 0xFF, 0xFF];

        Assert.Throws<NbtFormatException>(() => ReadFromSpan(bytes));
        Assert.Throws<NbtFormatException>(() => ReadFromSequence(bytes));
    }

    [Fact]
    public void NestingAtTheLimit_IsAccepted()
    {
        var bytes = NestedCompounds(512);

        Assert.NotNull(ReadFromSpan(bytes));
        Assert.NotNull(ReadFromSequence(bytes));
        Assert.NotNull(NbtReader.ReadTag(new MemoryStream(bytes)));
    }

    [Fact]
    public void NestingPastTheLimit_ThrowsInsteadOfOverflowingTheStack()
    {
        var bytes = NestedCompounds(2000);

        Assert.Throws<NbtFormatException>(() => ReadFromSpan(bytes));
        Assert.Throws<NbtFormatException>(() => ReadFromSequence(bytes));
        Assert.Throws<NbtFormatException>(() => NbtReader.ReadTag(new MemoryStream(bytes)));
    }

    [Fact]
    public void WritingPastTheDepthLimit_Throws()
    {
        var root = new NbtCompound("root");
        var current = root;
        for (var i = 0; i < 600; i++)
        {
            var child = new NbtCompound("c");
            current.Add(child);
            current = child;
        }

        Assert.Throws<NbtFormatException>(() => WriteToBuffer(root, true));
    }

    [Fact]
    public void DuplicateCompoundNames_LastOneWins()
    {
        List<byte> bytes = [(byte)NbtTagType.Compound, 0x00, 0x00];
        bytes.AddRange([(byte)NbtTagType.Int, 0x00, 0x01, (byte)'a', 0, 0, 0, 1]);
        bytes.AddRange([(byte)NbtTagType.Int, 0x00, 0x01, (byte)'a', 0, 0, 0, 2]);
        bytes.Add((byte)NbtTagType.End);
        var data = bytes.ToArray();

        foreach (var tag in new[] { ReadFromSpan(data), ReadFromSequence(data), NbtReader.ReadTag(new MemoryStream(data)) })
        {
            var compound = Assert.IsType<NbtCompound>(tag);
            Assert.Single(compound);
            Assert.Equal(2, compound.Get<NbtInt>("a")!.Value);
        }
    }

    [Fact]
    public void EmptyList_SerializesWithTheEndElementType()
    {
        var tag = new NbtCompound("root") { new NbtList("empty") };

        var bytes = WriteToBuffer(tag, true);

        Assert.Equal(bytes, WriteToStream(tag, true));
        var list = Assert.IsType<NbtCompound>(ReadFromSpan(bytes)).Get<NbtList>("empty")!;
        Assert.Empty(list);
        Assert.Equal(NbtTagType.End, list.ListType);
        Assert.Equal(NbtTagType.End, Assert.IsType<NbtCompound>(ReadFromSequence(bytes)).Get<NbtList>("empty")!.ListType);
        Assert.Equal(NbtTagType.End,
            Assert.IsType<NbtCompound>(NbtReader.ReadTag(new MemoryStream(bytes))).Get<NbtList>("empty")!.ListType);
    }

    [Fact]
    public void NonEmptyListOfEndElements_IsRejected()
    {
        byte[] bytes = [(byte)NbtTagType.List, 0x00, 0x00, (byte)NbtTagType.End, 0, 0, 0, 3];

        Assert.Throws<NbtFormatException>(() => ReadFromSpan(bytes));
        Assert.Throws<NbtFormatException>(() => ReadFromSequence(bytes));
    }

    [Fact]
    public void TruncatedData_ThrowsNbtFormatException()
    {
        var bytes = WriteToBuffer(BuildKitchenSink("root"), true);

        Assert.Throws<NbtFormatException>(() => ReadFromSpan(bytes[..(bytes.Length / 2)]));
        Assert.Throws<NbtFormatException>(() => ReadFromSequence(bytes[..(bytes.Length / 2)]));
    }

    // ── Pull reader: roots that are not compounds ─────────────────────────────

    [Fact]
    public void PullReader_RootList_WalksToTheEndInsteadOfThrowing()
    {
        var bytes = WriteToStream(new NbtList("root", [new NbtByte(5), new NbtByte(6)]), true);

        var reader = new NbtReader(new MemoryStream(bytes));

        Assert.True(reader.ReadToFollowing());
        Assert.Equal(NbtTagType.List, reader.TagType);
        Assert.True(reader.ReadToFollowing());
        Assert.Equal((byte)5, reader.ReadValueAs<byte>());
        Assert.True(reader.ReadToFollowing());
        Assert.Equal((byte)6, reader.ReadValueAs<byte>());
        Assert.False(reader.ReadToFollowing());
        Assert.True(reader.IsAtStreamEnd);
        Assert.False(reader.IsInErrorState);
    }

    [Fact]
    public void PullReader_EmptyRootList_Ends()
    {
        var bytes = WriteToStream(new NbtList("root"), true);

        var reader = new NbtReader(new MemoryStream(bytes));

        Assert.True(reader.ReadToFollowing());
        Assert.False(reader.ReadToFollowing());
        Assert.True(reader.IsAtStreamEnd);
        Assert.False(reader.IsInErrorState);
    }

    [Fact]
    public void PullReader_RootList_ReadAsTag_RebuildsTheList()
    {
        var original = new NbtList("root", [new NbtString("a"), new NbtString("b")]);
        var bytes = WriteToStream(original, true);

        var restored = new NbtReader(new MemoryStream(bytes)).ReadAsTag();

        Assert.Equal(original.ToString(), restored.ToString());
    }

    [Fact]
    public void PullReader_ListOfEndElements_Throws()
    {
        byte[] bytes =
        [
            (byte)NbtTagType.Compound, 0x00, 0x00,
            (byte)NbtTagType.List, 0x00, 0x01, (byte)'l', (byte)NbtTagType.End, 0, 0, 0, 2,
            (byte)NbtTagType.End
        ];

        var reader = new NbtReader(new MemoryStream(bytes));

        Assert.True(reader.ReadToFollowing());
        Assert.Throws<NbtFormatException>(() => reader.ReadToFollowing());
    }

    [Fact]
    public void PullReader_NestingPastTheLimit_Throws()
    {
        var deep = new NbtReader(new MemoryStream(NestedCompounds(2000)));
        Assert.Throws<NbtFormatException>(() =>
        {
            while (deep.ReadToFollowing()) { }
        });

        var atLimit = new NbtReader(new MemoryStream(NestedCompounds(512)));
        while (atLimit.ReadToFollowing()) { }
        Assert.True(atLimit.IsAtStreamEnd);
    }

    [Fact]
    public void PullReader_ArrayLengthThatOverflowsTheSkipMath_Throws()
    {
        byte[] bytes =
        [
            (byte)NbtTagType.Compound, 0x00, 0x00,
            (byte)NbtTagType.IntArray, 0x00, 0x01, (byte)'a', 0x40, 0x00, 0x00, 0x00,
            (byte)NbtTagType.Int, 0x00, 0x01, (byte)'z', 0, 0, 0, 7,
            (byte)NbtTagType.End
        ];

        var reader = new NbtReader(new MemoryStream(bytes));

        Assert.True(reader.ReadToFollowing());
        Assert.True(reader.ReadToFollowing());
        Assert.Equal(NbtTagType.IntArray, reader.TagType);
        Assert.Throws<NbtFormatException>(() => reader.ReadToFollowing());
    }

    [Fact]
    public void PullReader_ReadListAsArray_MidList_ReadsTheRemainingElements()
    {
        var bytes = WriteToStream(
            new NbtCompound("root")
            {
                new NbtList("l", [new NbtInt(1), new NbtInt(2), new NbtInt(3), new NbtInt(4)]),
                new NbtInt("z", 99)
            }, true);

        var reader = new NbtReader(new MemoryStream(bytes));
        Assert.True(reader.ReadToFollowing("l"));
        Assert.True(reader.ReadToFollowing());

        Assert.Equal([1, 2, 3, 4], reader.ReadListAsArray<int>());
        Assert.True(reader.ReadToFollowing("z"));
        Assert.Equal(99, reader.ReadValueAs<int>());
    }

    // ── Pull writer ───────────────────────────────────────────────────────────

    [Fact]
    public void PullWriter_ArrayWithOffset_WritesExactlyThatWindow()
    {
        var stream = new MemoryStream();
        var writer = new NbtWriter(stream, "root");
        writer.WriteIntArray("i", [1, 2, 3, 4, 5, 6], 2, 3);
        writer.WriteLongArray("l", [10L, 20L, 30L, 40L], 1, 2);
        writer.Finish();

        var restored = Assert.IsType<NbtCompound>(NbtReader.ReadTag(new MemoryStream(stream.ToArray())));
        Assert.Equal([3, 4, 5], restored.Get<NbtIntArray>("i")!.Value);
        Assert.Equal([20L, 30L], restored.Get<NbtLongArray>("l")!.Value);
    }

    [Fact]
    public void PullWriter_ByteArrayFromAShortStream_ThrowsInsteadOfSpinning()
    {
        var writer = new NbtWriter(new MemoryStream(), "root");

        Assert.Throws<EndOfStreamException>(() =>
            writer.WriteByteArray("b", new MemoryStream(new byte[10]), 100));
    }

    [Fact]
    public void PullWriter_EmptyList_UsesTheEndElementType()
    {
        var stream = new MemoryStream();
        var writer = new NbtWriter(stream, "root");
        writer.BeginList("empty", NbtTagType.End, 0);
        writer.EndList();
        writer.Finish();

        Assert.Equal(WriteToStream(new NbtCompound("root") { new NbtList("empty") }, true), stream.ToArray());
    }

    [Fact]
    public void PullWriter_NonEmptyListOfEndElements_IsRejected()
    {
        var writer = new NbtWriter(new MemoryStream(), "root");

        Assert.Throws<ArgumentOutOfRangeException>(() => writer.BeginList("l", NbtTagType.End, 2));
    }

    [Fact]
    public void StreamWriter_NestingPastTheLimit_ThrowsInsteadOfOverflowingTheStack()
    {
        var root = new NbtCompound("root");
        var current = root;
        for (var i = 0; i < 5000; i++)
        {
            var child = new NbtCompound("c");
            current.Add(child);
            current = child;
        }

        Assert.Throws<NbtFormatException>(() => WriteToStream(root, true));
    }

    [Fact]
    public void MalformedStringBytes_ThrowNbtFormatException()
    {
        byte[] bytes = [(byte)NbtTagType.String, 0x00, 0x00, 0x00, 0x02, 0xC3, 0x41];

        Assert.Throws<NbtFormatException>(() => ReadFromSpan(bytes));
        Assert.Throws<NbtFormatException>(() => ReadFromSequence(bytes));
        Assert.Throws<NbtFormatException>(() => NbtReader.ReadTag(new MemoryStream(bytes)));
    }
}
