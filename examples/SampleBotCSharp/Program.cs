using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Runtime.Intrinsics;
using McProtoNet.Protocol;
using SampleBotCSharp;
using System.Security.Cryptography;
using DotNext.Buffers;
using McProtoNet.Client;
using McProtoNet.Serialization;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace SampleBotCSharp;

class Program
{
    

    public static async Task Main(string[] args)
    {
        var tcp = new TcpClient();
        
        await tcp.ConnectAsync("127.0.0.1", 25565);

        var stream = tcp.GetStream();

        var client = PipelinesMinecraftClient.Create(stream,773);

        var handshake = new HandshakePacket()
        {
            ProtocolVersion = 773,
            NextState = 2,
            ServerHost = "127.0.0.1",
            ServerPort = 25565
        };
        
        
    }

    
}
public interface IPacket 
{
    void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion); 
}
    
class HandshakePacket : IPacket
{
    public int ProtocolVersion { get; set; }
    public string ServerHost { get; set; }
    public ushort ServerPort { get; set; }
    public int NextState { get; set; }
        
    public void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(ProtocolVersion);
        writer.WriteString(ServerHost);
        writer.WriteUnsignedShort(ServerPort);
        writer.WriteVarInt(NextState);
    }
}
static class Ext
{
    extension(PipelinesMinecraftClient client)
    {
        public async ValueTask SendPacket(IPacket packet, int id, CancellationToken cancellationToken = default)
        {
            
            var writer = new MinecraftPrimitiveWriter();
            
            writer.WriteVarInt(id);
            packet.Serialize(ref writer, client.ProtocolVersion);

            using var memory = writer.GetWrittenMemory();
            await client.SendPacketAsync(memory.Memory, cancellationToken);
        }
    }
}