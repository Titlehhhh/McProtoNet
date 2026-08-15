using System.Buffers;
using McProtoNet.Net;

namespace McProtoNet.Tests.StreamBased;

public class PacketReader
{
    [Fact]
    public async Task ReadPacketAsync_ShouldRoundTripFrames_AndThrowOnEof()
    {
        var token = TestContext.Current.CancellationToken;

        var ms = new MemoryStream();
        var sender = new MinecraftPacketSender(ms, leaveOpen: true);
        byte[] first = [0x05, 1, 2, 3, 4, 5];
        byte[] second = [0x07, 9, 8, 7];
        await sender.SendPacketAsync(first, token);
        await sender.SendPacketAsync(second, token);

        ms.Position = 0;
        using var reader = new MinecraftPacketReader(ms, ArrayPool<byte>.Shared, leaveOpen: true);

        var packet1 = await reader.ReadPacketAsync(token);
        Assert.Equal(0x05, packet1.Id);
        Assert.Equal(first[1..], packet1.Data.ToArray());

        var packet2 = await reader.ReadPacketAsync(token);
        Assert.Equal(0x07, packet2.Id);
        Assert.Equal(second[1..], packet2.Data.ToArray());

        await Assert.ThrowsAsync<EndOfStreamException>(async () => await reader.ReadPacketAsync(token));
    }
}
