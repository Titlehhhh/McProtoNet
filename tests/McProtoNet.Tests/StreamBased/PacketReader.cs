using System.Buffers;
using McProtoNet.Net;

namespace McProtoNet.Tests.StreamBased;

public class PacketReader
{
    [Fact]
    public async Task Test1()
    {
        var ms = new MemoryStream();
        using var reader = new MinecraftPacketReader(ms, ArrayPool<byte>.Shared);

        CancellationToken token = default;
        var packet1 = await reader.ReadPacketAsync(token);
        var packet2 = await reader.ReadPacketAsync(token);

        var data1 = packet1.Data; 
        
        // Or

        while (token.IsCancellationRequested == false)
        {
            var packet = await reader.ReadPacketAsync(token);
            int id = packet.Id;
            ReadOnlySequence<byte> data = packet.Data;
            
            //Handling

        }
        
    }
}