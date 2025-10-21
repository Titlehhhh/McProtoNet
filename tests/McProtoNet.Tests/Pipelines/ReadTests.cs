using System.Buffers;
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
        await sender.SendPacketAsync(new byte[500], Token());

        ms.Position = 0;
        var pipeReader = PipeReader.Create(ms);

        var reader = new MinecraftPacketPipeReader(pipeReader);

        var packet1 = await reader.ReadPacketAsync(Token());
        var packet2 = await reader.ReadPacketAsync(Token());


        Assert.Equal(500, packet2.FullLength);

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

        await sender.SendPacketAsync(new byte[500], Token());
        await sender.SendPacketAsync(new byte[500], Token());

        ms.Position = 0;
        var pipeReader = PipeReader.Create(new ReadOnlySequence<byte>(ms.ToArray()));
        
        var reader = new MinecraftPacketPipeReader(pipeReader);

        int count = 0;
        NewInputPacket test = default;
        await foreach (var packet in reader.ReadPacketsAsync(TestContext.Current.CancellationToken))
        {
            var lengg = packet.FullLength;
            test = packet;
            count++;
            if (count == 2)
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