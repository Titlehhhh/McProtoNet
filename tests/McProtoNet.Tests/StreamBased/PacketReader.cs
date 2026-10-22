using System.Buffers;
using McProtoNet.Transport.Framing;

namespace McProtoNet.Tests.StreamBased;

public class PacketReader
{
    [Fact]
    public async Task ReadPacketAsync_ShouldRoundTripFrames_AndThrowOnEof()
    {
        var token = TestContext.Current.CancellationToken;

        var ms = new MemoryStream();
        var sender = new PacketStreamWriter(ms, leaveOpen: true);
        byte[] first = [0x05, 1, 2, 3, 4, 5];
        byte[] second = [0x07, 9, 8, 7];
        await sender.WritePacketAsync(first, token);
        await sender.WritePacketAsync(second, token);

        ms.Position = 0;
        using var reader = new PacketStreamReader(ms, ArrayPool<byte>.Shared, leaveOpen: true);

        var packet1 = await reader.ReadPacketAsync(token);
        Assert.Equal(0x05, packet1.Id);
        Assert.Equal(first[1..], packet1.Body.ToArray());

        var packet2 = await reader.ReadPacketAsync(token);
        Assert.Equal(0x07, packet2.Id);
        Assert.Equal(second[1..], packet2.Body.ToArray());

        await Assert.ThrowsAsync<EndOfStreamException>(async () => await reader.ReadPacketAsync(token));
    }
}
