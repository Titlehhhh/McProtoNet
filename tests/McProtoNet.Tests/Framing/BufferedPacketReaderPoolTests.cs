using System.Buffers;
using McProtoNet.Primitives;
using McProtoNet.Tests.Infrastructure;
using McProtoNet.Transport.Framing;

namespace McProtoNet.Tests.Framing;

/// <summary>
///     The buffered reader shares its blocks with the packets it hands out. A packet from a loop is
///     borrowed for one step; a retained one stays valid past the next batch and past the reader. The
///     reader overwrites a block in place only while it is the sole holder, and otherwise moves on to
///     a fresh one. Every array goes back to the pool exactly once, when the last holder lets go.
/// </summary>
public class BufferedPacketReaderPoolTests
{
    private static byte[] Frame(int id, byte[] body, int compressionThreshold)
    {
        var writer = new ArrayBufferWriter<byte>(body.Length + 16);
        writer.WritePacket(id, body, compressionThreshold);
        return writer.WrittenSpan.ToArray();
    }

    private static byte[] Body(int length, byte seed)
    {
        var body = new byte[length];
        for (var i = 0; i < body.Length; i++) body[i] = (byte)(seed + i % 7);
        return body;
    }

    private static async Task<IncomingPacket> ReadOne(BufferedPacketReader reader, CancellationToken token)
    {
        var batch = await reader.ReadBatchAsync(token);
        Assert.Equal(1, batch.Count);
        foreach (var packet in batch) return packet.Retain();
        throw new InvalidOperationException("no packet");
    }

    /// <summary>With threshold 0 every body goes through the arena; with -1 it stays in the read block.</summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task PacketsOfOneBatch_SurviveTheNextBatch_AndTheReader(int threshold)
    {
        var token = TestContext.Current.CancellationToken;
        var pool = new CountingArrayPool();
        var wire = new ScriptedReadStream();
        var reader = new BufferedPacketReader(wire, threshold, pool: pool);

        wire.Push(Frame(1, [9, 8, 7], threshold));
        var first = await ReadOne(reader, token);

        wire.Push(Frame(2, [1, 2], threshold));
        var second = await ReadOne(reader, token);

        Assert.Equal<byte[]>([9, 8, 7], first.Body.ToArray());
        Assert.Equal<byte[]>([1, 2], second.Body.ToArray());

        reader.Dispose();
        Assert.Equal(1, first.Id);
        Assert.Equal<byte[]>([9, 8, 7], first.Body.ToArray());
        Assert.True(pool.OnLoan > 0);

        first.Dispose();
        second.Dispose();
        Assert.Equal(0, pool.OnLoan);
        Assert.Empty(pool.Violations);
    }

    /// <summary>
    ///     Frames of 600 bytes through a 1024-byte block: the second one never fits at the tail, so the
    ///     reader has to make room before every read. Alone, it shifts the tail down in place and rents
    ///     nothing new.
    /// </summary>
    [Fact]
    public async Task SoleHolder_CompactsInPlace_AndRentsNothingNew()
    {
        var token = TestContext.Current.CancellationToken;
        var pool = new CountingArrayPool();
        var wire = new MemoryStream();
        for (var i = 0; i < 6; i++) wire.Write(Frame(i, Body(600, (byte)i), -1));
        wire.Position = 0;

        var reader = new BufferedPacketReader(wire, -1, initialCapacity: 1024, pool: pool);

        var seen = 0;
        await foreach (var packet in reader.ReadPacketsAsync(token))
        {
            Assert.Equal(Body(600, (byte)seen), packet.Body.ToArray());
            packet.Dispose();
            seen++;
        }

        Assert.Equal(6, seen);
        Assert.Equal(1, pool.Rents);

        reader.Dispose();
        Assert.Equal(0, pool.OnLoan);
        Assert.Empty(pool.Violations);
    }

    /// <summary>The same stream, but the first packet is retained: its bytes must not move under it.</summary>
    [Fact]
    public async Task RetainedPacket_MakesTheReaderMoveToAFreshBlock_AndStaysIntact()
    {
        var token = TestContext.Current.CancellationToken;
        var pool = new CountingArrayPool();
        var wire = new MemoryStream();
        for (var i = 0; i < 6; i++) wire.Write(Frame(i, Body(600, (byte)i), -1));
        wire.Position = 0;

        var reader = new BufferedPacketReader(wire, -1, initialCapacity: 1024, pool: pool);

        var held = default(IncomingPacket);
        var seen = 0;
        await foreach (var packet in reader.ReadPacketsAsync(token))
        {
            Assert.Equal(Body(600, (byte)seen), packet.Body.ToArray());
            if (seen == 0) held = packet.Retain();
            seen++;
        }

        Assert.Equal(6, seen);
        Assert.True(pool.Rents > 1);
        Assert.Equal(Body(600, 0), held.Body.ToArray());

        reader.Dispose();
        Assert.Equal(Body(600, 0), held.Body.ToArray());

        held.Dispose();
        Assert.Equal(0, pool.OnLoan);
        Assert.Empty(pool.Violations);
    }

    /// <summary>
    ///     Two inflated bodies of 40 KiB in one batch outgrow the 64 KiB arena between them. The first
    ///     packet keeps pointing into the arena it was inflated into; nothing is copied.
    /// </summary>
    [Fact]
    public async Task ArenaOutgrownMidBatch_KeepsTheEarlierPacketsValid()
    {
        var token = TestContext.Current.CancellationToken;
        var pool = new CountingArrayPool();
        var wire = new MemoryStream();
        wire.Write(Frame(1, Body(40 * 1024, 1), 0));
        wire.Write(Frame(2, Body(40 * 1024, 2), 0));
        wire.Position = 0;

        var reader = new BufferedPacketReader(wire, 0, pool: pool);

        var batch = await reader.ReadBatchAsync(token);
        Assert.Equal(2, batch.Count);

        // the read block, the first arena and the arena that replaced it: nothing else was rented
        Assert.Equal(3, pool.Rents);

        var packets = new List<IncomingPacket>();
        foreach (var packet in batch) packets.Add(packet.Retain());

        Assert.Equal(Body(40 * 1024, 1), packets[0].Body.ToArray());
        Assert.Equal(Body(40 * 1024, 2), packets[1].Body.ToArray());

        var next = await reader.ReadBatchAsync(token);
        Assert.True(next.IsCompleted);
        Assert.Equal(Body(40 * 1024, 1), packets[0].Body.ToArray());

        reader.Dispose();
        foreach (var packet in packets) packet.Dispose();
        Assert.Equal(0, pool.OnLoan);
        Assert.Empty(pool.Violations);
    }

    [Fact]
    public async Task ReadPacketsAsync_LendsPackets_RetainKeepsThem()
    {
        var token = TestContext.Current.CancellationToken;
        var pool = new CountingArrayPool();
        var wire = new MemoryStream();
        for (var i = 0; i < 4; i++) wire.Write(Frame(i, Body(300, (byte)i), 0));
        wire.Position = 0;

        var reader = new BufferedPacketReader(wire, 0, pool: pool);

        var packets = new List<IncomingPacket>();
        await foreach (var packet in reader.ReadPacketsAsync(token)) packets.Add(packet.Retain());
        reader.Dispose();

        for (var i = 0; i < 4; i++)
        {
            Assert.Equal(i, packets[i].Id);
            Assert.Equal(Body(300, (byte)i), packets[i].Body.ToArray());
        }

        foreach (var packet in packets) packet.Dispose();
        Assert.Equal(0, pool.OnLoan);
        Assert.Empty(pool.Violations);
    }

    /// <summary>A packet from the loop is borrowed: disposing it returns nothing, Retain keeps it.</summary>
    [Fact]
    public async Task LoopPacket_IsBorrowed_ItsDisposeReturnsNothing()
    {
        var token = TestContext.Current.CancellationToken;
        var pool = new CountingArrayPool();
        var wire = new MemoryStream(Frame(1, Body(300, 1), -1));

        var reader = new BufferedPacketReader(wire, -1, pool: pool);

        IncomingPacket borrowed = default;
        IncomingPacket kept = default;
        await foreach (var packet in reader.ReadPacketsAsync(token))
        {
            borrowed = packet;
            kept = packet.Retain();
        }

        reader.Dispose();
        borrowed.Dispose();
        Assert.Equal(1, pool.OnLoan);
        Assert.Equal(Body(300, 1), kept.Body.ToArray());

        kept.Dispose();
        Assert.Equal(0, pool.OnLoan);
        Assert.Empty(pool.Violations);
    }

    /// <summary>
    ///     Once the reader is gone, the enumerator is the only thing keeping the block alive: it must
    ///     take the next packet before it lets the previous one go.
    /// </summary>
    [Fact]
    public async Task Enumerator_OwnsThePacketItStandsOn_UntilItMovesOrEnds()
    {
        var token = TestContext.Current.CancellationToken;
        var pool = new CountingArrayPool();
        var wire = new MemoryStream();
        for (var i = 0; i < 3; i++) wire.Write(Frame(i, Body(100, (byte)i), -1));
        wire.Position = 0;

        var reader = new BufferedPacketReader(wire, -1, pool: pool);
        var batch = await reader.ReadBatchAsync(token);
        Assert.Equal(3, batch.Count);

        var enumerator = batch.GetEnumerator();
        Assert.True(enumerator.MoveNext());
        var first = enumerator.Current;

        reader.Dispose();
        Assert.Equal(1, pool.OnLoan);
        Assert.Equal(Body(100, 0), first.Body.ToArray());

        // every read of Current is a borrow: disposing them changes nothing
        var again = enumerator.Current;
        first.Dispose();
        again.Dispose();
        Assert.Equal(1, pool.OnLoan);

        Assert.True(enumerator.MoveNext());
        Assert.Equal(Body(100, 1), enumerator.Current.Body.ToArray());
        Assert.True(enumerator.MoveNext());
        Assert.False(enumerator.MoveNext());
        Assert.Equal(0, pool.OnLoan);

        enumerator.Dispose();
        Assert.Equal(0, pool.OnLoan);
        Assert.Empty(pool.Violations);
    }

    [Fact]
    public async Task Batch_EnumeratedTwice_LeavesTheReaderTheSoleHolder()
    {
        var token = TestContext.Current.CancellationToken;
        var pool = new CountingArrayPool();
        var wire = new MemoryStream();
        for (var i = 0; i < 3; i++) wire.Write(Frame(i, Body(100, (byte)i), -1));
        wire.Position = 0;

        var reader = new BufferedPacketReader(wire, -1, pool: pool);
        var batch = await reader.ReadBatchAsync(token);

        var seen = 0;
        foreach (var packet in batch) seen += packet.Body.Length;
        foreach (var packet in batch) seen += packet.Body.Length;
        Assert.Equal(600, seen);

        reader.Dispose();
        Assert.Equal(0, pool.OnLoan);
        Assert.Empty(pool.Violations);
    }

    /// <summary>
    ///     A read that was in flight when the reader was disposed still completes, but its batch is
    ///     dead: the reader let go of the blocks when the read ended.
    /// </summary>
    [Fact]
    public async Task Dispose_MidRead_TheReadCompletes_ButItsBatchIsDead()
    {
        var token = TestContext.Current.CancellationToken;
        var pool = new CountingArrayPool();
        var wire = new ScriptedReadStream();
        var reader = new BufferedPacketReader(wire, -1, pool: pool);

        // the read parks inside the stream before the call returns, so Dispose lands mid-read
        var reading = reader.ReadBatchAsync(token).AsTask();
        reader.Dispose();
        Assert.Equal(1, pool.OnLoan);

        wire.Push(Frame(1, Body(100, 1), -1));
        var batch = await reading.WaitAsync(TimeSpan.FromSeconds(10), token);

        Assert.Equal(1, batch.Count);
        Assert.Equal(0, pool.OnLoan);
        Assert.Throws<ObjectDisposedException>(() =>
        {
            foreach (var packet in batch) _ = packet.Body.Length;
        });
        Assert.Empty(pool.Violations);
    }

    /// <summary>
    ///     A read still parked in the stream when the reader is disposed may have the operating system
    ///     writing into the block. The block goes back only once that read has ended, and the reader
    ///     refuses every read after it.
    /// </summary>
    [Fact]
    public async Task Dispose_MidRead_ReleasesTheBlocks_OnlyWhenTheReadEnds()
    {
        var token = TestContext.Current.CancellationToken;
        var pool = new CountingArrayPool();
        var gate = new GateStream();
        var reader = new BufferedPacketReader(gate, -1, pool: pool);

        var reading = reader.ReadBatchAsync(token).AsTask();
        await gate.ReadStarted.WaitAsync(TimeSpan.FromSeconds(10), token);

        reader.Dispose();
        Assert.Equal(1, pool.OnLoan);

        gate.ReleaseRead(0);
        await Record.ExceptionAsync(() => reading);

        Assert.Equal(0, pool.OnLoan);
        Assert.Empty(pool.Violations);

        var after = await Record.ExceptionAsync(async () => await reader.ReadBatchAsync(token));
        Assert.IsType<ObjectDisposedException>(after);
    }
}
