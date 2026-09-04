using System.Buffers;
using System.Runtime.InteropServices;
using McProtoNet.Primitives;

namespace McProtoNet.Tests.Primitives;

/// <summary>
///     The packet owns one pooled buffer and hands it back exactly once. A second Dispose must find
///     nothing left to return: two returns put the same array in the pool twice, and the pool then
///     serves it to two owners at the same time.
/// </summary>
public class OutgoingPacketTests
{
    /// <summary>Dispose empties the packet itself, so there is nothing left for a second call to return.</summary>
    [Fact]
    public void Dispose_LeavesNothingToReturn()
    {
        var packet = new OutgoingPacket(MemoryOwner<byte>.Allocate(1024));

        Assert.Equal(1024, packet.Memory.Length);

        packet.Dispose();

        Assert.True(packet.Memory.IsEmpty);
        Assert.True(packet.Span.IsEmpty);
    }

    /// <summary>The second Dispose does nothing and does not throw.</summary>
    [Fact]
    public void Dispose_Twice_DoesNotThrow()
    {
        var packet = new OutgoingPacket(MemoryOwner<byte>.Allocate(64));

        packet.Dispose();
        packet.Dispose();
    }

    /// <summary>
    ///     After two Dispose calls the pool must not hold the buffer twice. The check is one-sided: a
    ///     test running beside this one can take the array before this one asks for it, which only hides
    ///     the second copy, and nothing except a double return can make the same array come back twice.
    /// </summary>
    [Fact]
    public void Dispose_Twice_DoesNotPutTheBufferInThePoolTwice()
    {
        const int length = 1024;

        var owner = MemoryOwner<byte>.Allocate(length);
        MemoryMarshal.TryGetArray<byte>(owner.Memory, out var segment);
        var array = segment.Array!;

        var packet = new OutgoingPacket(owner);
        packet.Dispose();
        packet.Dispose();

        var rented = new List<byte[]>();
        try
        {
            for (var i = 0; i < 16; i++) rented.Add(ArrayPool<byte>.Shared.Rent(length));

            Assert.True(rented.Count(candidate => ReferenceEquals(candidate, array)) <= 1,
                "the pool served the same array to two rents: it was returned twice");
        }
        finally
        {
            foreach (var buffer in rented.Distinct()) ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}
