using McProtoNet.Tests.Infrastructure;
using McProtoNet.Transport.Framing;

namespace McProtoNet.Tests.Framing;

/// <summary>
///     The buffered reader must hand out exactly what the one-at-a-time reader hands out, byte for
///     byte, over the same stream — under every threshold, with and without a cipher, and no matter
///     how the bytes are sliced on the way in.
/// </summary>
public class BufferedPacketReaderTests
{
    public static TheoryData<int, bool> Modes => new()
    {
        { -1, false }, { -1, true },
        { 0, false }, { 0, true },
        { 256, false }, { 256, true }
    };

    [Theory]
    [MemberData(nameof(Modes))]
    public async Task MatchesStreamReader_ByteForByte(int threshold, bool encrypted)
    {
        var token = TestContext.Current.CancellationToken;
        var packets = Frames.Sample(seed: 7);
        var wire = Frames.Build(packets, threshold, encrypted);

        var expected = await ReadWithStreamReader(wire, threshold, encrypted, chunkSeed: 11, token);
        var actual = await ReadWithBufferedReader(wire, threshold, encrypted, chunkSeed: 13, token);

        AssertSame(packets, expected);
        AssertSame(packets, actual);
    }

    [Theory]
    [MemberData(nameof(Modes))]
    public async Task SurvivesEveryChunkSize(int threshold, bool encrypted)
    {
        var token = TestContext.Current.CancellationToken;
        var small = Frames.SmallSample(seed: 21);
        var smallWire = Frames.Build(small, threshold, encrypted);
        foreach (var maxChunk in new[] { 1, 2, 3, 7 })
        {
            var actual = await ReadWithBufferedReader(smallWire, threshold, encrypted, chunkSeed: maxChunk, token,
                maxChunk);
            AssertSame(small, actual);
        }

        var packets = Frames.Sample(seed: 21, repeats: 1);
        var wire = Frames.Build(packets, threshold, encrypted);
        foreach (var maxChunk in new[] { 64, 999, 4096, 64 * 1024 })
        {
            var actual = await ReadWithBufferedReader(wire, threshold, encrypted, chunkSeed: maxChunk, token,
                maxChunk);
            AssertSame(packets, actual);
        }
    }

    [Theory]
    [MemberData(nameof(Modes))]
    public async Task ReadPacketsAsync_EndsAtEndOfStream(int threshold, bool encrypted)
    {
        var token = TestContext.Current.CancellationToken;
        var packets = Frames.Sample(seed: 31, repeats: 1);
        var wire = Frames.Build(packets, threshold, encrypted);

        using var cipher = Frames.Decryptor(encrypted);
        using var reader = new BufferedPacketReader(new ChunkedReadStream(wire, 5), threshold, cipher);

        var read = new List<TestPacket>();
        await foreach (var packet in reader.ReadPacketsAsync(token))
            read.Add(new TestPacket(packet.Id, packet.Body.ToArray()));

        AssertSame(packets, read);
    }

    [Fact]
    public async Task EmptyStream_YieldsCompletedEmptyBatch()
    {
        var token = TestContext.Current.CancellationToken;
        using var reader = new BufferedPacketReader(new MemoryStream([]));

        var batch = await reader.ReadBatchAsync(token);

        Assert.Equal(0, batch.Count);
        Assert.True(batch.IsCompleted);
    }

    [Fact]
    public async Task HalfFrameThenEof_Throws()
    {
        var token = TestContext.Current.CancellationToken;
        var packets = new List<TestPacket> { new(1, new byte[64]) };
        var wire = Frames.Build(packets, -1, false);

        using var reader = new BufferedPacketReader(new MemoryStream(wire[..(wire.Length / 2)]));

        await Assert.ThrowsAsync<EndOfStreamException>(async () => await reader.ReadBatchAsync(token));
    }

    [Fact]
    public async Task Batch_DeliversEveryWholeFrameThatArrivedTogether()
    {
        var token = TestContext.Current.CancellationToken;
        var packets = Frames.Sample(seed: 41, repeats: 1);
        var wire = Frames.Build(packets, -1, false);

        // one giant read: the whole stream lands in the buffer at once
        using var reader = new BufferedPacketReader(new MemoryStream(wire), initialCapacity: wire.Length + 16);
        var batch = await reader.ReadBatchAsync(token);

        Assert.Equal(packets.Count, batch.Count);
        Assert.False(batch.IsCompleted);
    }

    private static async Task<List<TestPacket>> ReadWithStreamReader(byte[] wire, int threshold, bool encrypted,
        int chunkSeed, CancellationToken token)
    {
        using var cipher = Frames.Decryptor(encrypted);
        using var reader = new PacketStreamReader(new ChunkedReadStream(wire, chunkSeed), leaveOpen: true)
        {
            CompressionThreshold = threshold,
            Cipher = cipher
        };

        var read = new List<TestPacket>();
        while (true)
        {
            try
            {
                var packet = await reader.ReadPacketAsync(token);
                read.Add(new TestPacket(packet.Id, packet.Body.ToArray()));
            }
            catch (EndOfStreamException)
            {
                return read;
            }
        }
    }

    private static async Task<List<TestPacket>> ReadWithBufferedReader(byte[] wire, int threshold, bool encrypted,
        int chunkSeed, CancellationToken token, int maxChunk = 64 * 1024)
    {
        using var cipher = Frames.Decryptor(encrypted);
        using var reader = new BufferedPacketReader(new ChunkedReadStream(wire, chunkSeed, maxChunk), threshold,
            cipher);

        var read = new List<TestPacket>();
        while (true)
        {
            var batch = await reader.ReadBatchAsync(token);
            foreach (var packet in batch)
                read.Add(new TestPacket(packet.Id, packet.Body.ToArray()));

            if (batch.IsCompleted) return read;
        }
    }

    private static void AssertSame(IReadOnlyList<TestPacket> expected, IReadOnlyList<TestPacket> actual)
    {
        Assert.Equal(expected.Count, actual.Count);
        for (var i = 0; i < expected.Count; i++)
        {
            Assert.Equal(expected[i].Id, actual[i].Id);
            Assert.Equal(expected[i].Body, actual[i].Body);
        }
    }
}
