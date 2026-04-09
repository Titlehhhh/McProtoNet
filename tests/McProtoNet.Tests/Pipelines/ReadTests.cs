using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
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

        List<int> lens = [];
        for (int i = 0; i < 100; i++)
        {
            var gg = new byte[500 + i];


            MinecraftPrimitiveWriter writer = new MinecraftPrimitiveWriter();

            writer.WriteVarInt(i); //ID
            writer.WriteBuffer(new byte[500 + i]);

            var array = writer.GetWrittenMemory();

            await sender.SendPacketAsync(array.Memory, Token());
            var len = array.Memory.Length;
            lens.Add(len);
            array.Dispose();
        }

        ms.Position = 0;
        var pipe = new Pipe();

        await pipe.Writer.WriteAsync(ms.ToArray(), TestContext.Current.CancellationToken);
        await pipe.Writer.CompleteAsync();

        var reader = new MinecraftPacketPipeReader(pipe.Reader);

        int count = 0;
        InputPacket test = default;
        await foreach (var packet in reader.ReadPacketsAsync(TestContext.Current.CancellationToken))
        {
            var lengg = packet.FullLength;

            var id = packet.Id;

            Assert.Equal(count, id);
            
            Assert.Equal(lens[count], lengg);
            
            Assert.Equal(500+count, packet.Data.Length);


            test = packet;
            count++;
        }

        Assert.Throws<InvalidOperationException>(() =>
        {
            var len = test.FullLength;
        });
        
        Assert.Equal(100, count);


        return;

        CancellationToken Token
            () => TestContext.Current.CancellationToken;
    }
}