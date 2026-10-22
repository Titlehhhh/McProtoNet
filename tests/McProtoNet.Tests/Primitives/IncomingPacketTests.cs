using McProtoNet.Primitives;
using McProtoNet.Tests.Infrastructure;

namespace McProtoNet.Tests.Primitives;

/// <summary>
///     A packet from a reader holds one reference to the block behind its body. Dispose gives it back
///     exactly once; Retain hands out a copy with a reference of its own, so the copy outlives the
///     original. A packet over plain memory owns nothing.
/// </summary>
public class IncomingPacketTests
{
    [Fact]
    public void Dispose_ReleasesTheBlock_Once()
    {
        var pool = new CountingArrayPool();
        var block = new PooledBlock(pool, 8);
        block.Array[0] = 7;
        var packet = new IncomingPacket(1, block, 0, 4);

        Assert.Equal(4, packet.Body.Length);
        Assert.Equal(7, packet.Body.Span[0]);

        packet.Dispose();
        Assert.Equal(0, pool.OnLoan);
        Assert.True(packet.Body.IsEmpty);

        packet.Dispose();
        Assert.Empty(pool.Violations);
    }

    [Fact]
    public void Retain_KeepsTheBlockAlive_UntilTheCopyIsDisposed()
    {
        var pool = new CountingArrayPool();
        var block = new PooledBlock(pool, 8);
        block.Array[1] = 9;
        var packet = new IncomingPacket(1, block, 1, 3);

        var kept = packet.Retain();
        packet.Dispose();

        Assert.Equal(1, pool.OnLoan);
        Assert.Equal(1, kept.Id);
        Assert.Equal(3, kept.Body.Length);
        Assert.Equal(9, kept.Body.Span[0]);

        kept.Dispose();
        Assert.Equal(0, pool.OnLoan);
        Assert.Empty(pool.Violations);
    }

    [Fact]
    public void Retain_AfterDispose_Throws()
    {
        var block = new PooledBlock(new CountingArrayPool(), 8);
        var packet = new IncomingPacket(1, block, 0, 8);
        packet.Dispose();

        Assert.Throws<ObjectDisposedException>(() => packet.Retain());
    }

    [Fact]
    public void Borrow_OwnsNothing_ButRetainFromItDoes()
    {
        var pool = new CountingArrayPool();
        var block = new PooledBlock(pool, 8);
        var packet = new IncomingPacket(1, block, 0, 4);

        var borrowed = packet.Borrow();
        Assert.Equal(4, borrowed.Body.Length);
        borrowed.Dispose();
        Assert.Equal(1, pool.OnLoan);

        var kept = packet.Borrow().Retain();
        packet.Dispose();
        Assert.Equal(1, pool.OnLoan);
        Assert.Equal(4, kept.Body.Length);

        kept.Dispose();
        Assert.Equal(0, pool.OnLoan);
        Assert.Empty(pool.Violations);
    }

    [Fact]
    public void Borrow_AfterDispose_Throws()
    {
        var block = new PooledBlock(new CountingArrayPool(), 8);
        var packet = new IncomingPacket(1, block, 0, 8);
        packet.Dispose();

        Assert.Throws<ObjectDisposedException>(() => packet.Borrow());
    }

    [Fact]
    public void PacketOverPlainMemory_OwnsNothing()
    {
        byte[] body = [1, 2, 3];
        var packet = new IncomingPacket(5, body);

        var copy = packet.Retain();
        packet.Dispose();

        Assert.Equal<byte[]>([1, 2, 3], copy.Body.ToArray());
        copy.Dispose();
        Assert.True(copy.Body.IsEmpty);
    }

    [Fact]
    public void FullLength_CountsTheIdAndTheBody()
    {
        var block = new PooledBlock(new CountingArrayPool(), 8);
        using var packet = new IncomingPacket(300, block, 2, 5);

        Assert.Equal(2 + 5, packet.FullLength);
    }
}
