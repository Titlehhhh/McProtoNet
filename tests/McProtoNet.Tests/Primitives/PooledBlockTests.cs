using McProtoNet.Primitives;
using McProtoNet.Tests.Infrastructure;

namespace McProtoNet.Tests.Primitives;

/// <summary>
///     One array, one counter. The block is born with the renter's reference, every holder adds one,
///     and the release that takes the count to zero is the one and only return to the pool.
/// </summary>
public class PooledBlockTests
{
    [Fact]
    public void NewBlock_HoldsOneReference_AndOneArrayOnLoan()
    {
        var pool = new CountingArrayPool();

        var block = new PooledBlock(pool, 100);

        Assert.Equal(1, block.References);
        Assert.Equal(1, pool.OnLoan);
        Assert.True(block.Array.Length >= 100);
    }

    [Fact]
    public void LastRelease_ReturnsTheArrayOnce()
    {
        var pool = new CountingArrayPool();
        var block = new PooledBlock(pool, 100);
        block.Retain();
        block.Retain();

        block.Release();
        block.Release();
        Assert.Equal(1, pool.OnLoan);

        block.Release();
        Assert.Equal(0, pool.OnLoan);
        Assert.Equal(0, block.References);
        Assert.Empty(block.Array);
        Assert.Empty(pool.Violations);
    }

    [Fact]
    public void Retain_AfterTheLastRelease_Throws()
    {
        var pool = new CountingArrayPool();
        var block = new PooledBlock(pool, 16);
        block.Release();

        Assert.Throws<ObjectDisposedException>(block.Retain);
        Assert.Equal(0, pool.OnLoan);
    }

    [Fact]
    public void Release_MoreOftenThanRetained_Throws_AndReturnsNothingTwice()
    {
        var pool = new CountingArrayPool();
        var block = new PooledBlock(pool, 16);
        block.Release();

        Assert.Throws<InvalidOperationException>(block.Release);
        Assert.Empty(pool.Violations);
    }

    [Fact]
    public void IsShared_OnlyWhileSomeoneElseHoldsIt()
    {
        var pool = new CountingArrayPool();
        var block = new PooledBlock(pool, 16);

        Assert.False(block.IsShared);
        block.Retain();
        Assert.True(block.IsShared);
        block.Release();
        Assert.False(block.IsShared);

        block.Release();
    }

    /// <summary>
    ///     A Retain racing the last Release either wins a reference or throws; the array never goes
    ///     back twice and is never handed out after it went back.
    /// </summary>
    [Fact]
    public async Task Retain_RacingTheLastRelease_NeverReturnsTwice()
    {
        var pool = new CountingArrayPool();
        var won = 0;
        var lost = 0;

        for (var round = 0; round < 2000; round++)
        {
            var block = new PooledBlock(pool, 16);
            var releasing = Task.Run(block.Release);
            var retaining = Task.Run(() =>
            {
                try
                {
                    block.Retain();
                    Assert.NotEmpty(block.Array);
                    block.Release();
                    Interlocked.Increment(ref won);
                }
                catch (ObjectDisposedException)
                {
                    Interlocked.Increment(ref lost);
                }
            });
            await Task.WhenAll(releasing, retaining);

            Assert.Equal(0, block.References);
            Assert.Empty(block.Array);
        }

        Assert.Equal(2000, won + lost);
        Assert.Equal(0, pool.OnLoan);
        Assert.Empty(pool.Violations);
    }

    [Fact]
    public async Task ManyHolders_OnManyThreads_ReturnExactlyOnce()
    {
        var pool = new CountingArrayPool();
        var block = new PooledBlock(pool, 1024);
        const int holders = 32;
        for (var i = 0; i < holders; i++) block.Retain();

        await Task.WhenAll(Enumerable.Range(0, holders).Select(_ => Task.Run(() =>
        {
            for (var j = 0; j < 2000; j++)
            {
                block.Retain();
                block.Release();
            }

            block.Release();
        })));
        block.Release();

        Assert.Equal(0, block.References);
        Assert.Equal(0, pool.OnLoan);
        Assert.Empty(pool.Violations);
    }
}
