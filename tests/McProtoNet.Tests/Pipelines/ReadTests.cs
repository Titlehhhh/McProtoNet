using System.Buffers;
using System.Buffers.Binary;
using System.IO.Pipelines;
using McProtoNet.Net;
using McProtoNet.Serialization;

namespace McProtoNet.Tests.Pipelines;

public class ReadTests
{
    [Fact]
    public async ValueTask Test1()
    {
        var ms = new MemoryStream();
        var sender = new MinecraftPacketSender(ms, leaveOpen: true);

        await sender.SendPacketAsync(new byte[500], Token());
        await sender.SendPacketAsync(new byte[501], Token());

        ms.Position = 0;
        var pipeReader = PipeReader.Create(ms);

        var reader = new MinecraftPacketPipeReader(pipeReader);

        var packet1 = await reader.ReadPacketAsync(Token());
        Assert.Equal(500, packet1.FullLength);
        var packet2 = await reader.ReadPacketAsync(Token());
        Assert.Equal(501, packet2.FullLength);


        Assert.Throws<InvalidOperationException>(() =>
        {
            var test = packet1.FullLength;
        });


        return;

        CancellationToken Token
            () => TestContext.Current.CancellationToken;
    }

    [Fact]
    public async ValueTask Test2()
    {
        var ms = new MemoryStream();
        var sender = new MinecraftPacketSender(ms, leaveOpen: true);

        for (int i = 0; i < 100; i++)
        {
            var gg = new byte[500 + i];
            
            BinaryPrimitives.WriteInt32BigEndian(
                gg.AsSpan(1), i); // 1 offset for packet identifier
            await sender.SendPacketAsync(gg, Token());
        }

        ms.Position = 0;
        var pipeReader = PipeReader.Create(ms);
        
        var reader = new MinecraftPacketPipeReader(pipeReader);

        int count = 0;
        NewInputPacket test = default;
        await foreach (var packet in reader.ReadPacketsAsync(TestContext.Current.CancellationToken))
        {
            var lengg = packet.FullLength;

            var packetTest = packet.Data.ToArray();

            var index = BinaryPrimitives.ReadInt32BigEndian(packetTest);
            
            Assert.Equal(500+count,lengg);
            Assert.Equal(count, index);
            test = packet;
            count++;
            
            if (count == 100)
            {
                break;
            }
        }

        Assert.Throws<InvalidOperationException>(() =>
        {
            var len = test.FullLength;
        });


        return;

        CancellationToken Token
            () => TestContext.Current.CancellationToken;
    }
}