using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Runtime.Intrinsics;
using McProtoNet.Protocol;
using SampleBotCSharp;
using System.Security.Cryptography;
using DotNext.Buffers;
using DotNext.Hosting;
using McProtoNet.Client;
using McProtoNet.Net;
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
        Console.WriteLine("Hi");
        ConsoleLifetimeTokenSource clts = new();
        var tcp = new TcpClient();

        await tcp.ConnectAsync("127.0.0.1", 25565, clts.Token);

        var stream = tcp.GetStream();

        byte[] buffer = new byte[4096];

        var mem = buffer.AsMemory();

        //var gg = await stream.ReadAsync(mem, clts.Token);

        //Console.WriteLine($"gg: {gg}");
        //return;

        var client = PipelinesMinecraftClient.Create(stream, 773);

        var handshake = new HandshakePacket
        {
            ProtocolVersion = 773,
            NextState = 2,
            ServerHost = "127.0.0.1",
            ServerPort = 25565
        };
        await client.SendPacket(handshake, 0x00, clts.Token);
        //await Task.Delay(500, clts.Token);
        await client.SendPacket(new LoginStartPacket
        {
            Name = "McProtoBot",
            UUID = Guid.NewGuid()
        }, 0x00, clts.Token);

        await foreach (var p in client.ReadPacketsAsync(clts.Token))
        {
            Console.WriteLine($"ReadPacket. Id: {p.Id}");
            if (p.Id == 0x03)
            {
                var reader = p.CreateReader();
                int threshold = reader.ReadVarInt();
                Console.WriteLine($"Set compression: {threshold}");
                client.CompressionThreshold = threshold;
            }

            if (p.Id == 0x02)
            {
                await client.SendEmptyPacketAsync(0x03, clts.Token);
                break;
            }
        }
        
        Console.ReadLine();
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

class LoginStartPacket : IPacket
{
    public string Name { get; set; }
    public Guid UUID { get; set; }

    public void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteString(Name);
        writer.WriteUUID(UUID);
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
            var testBuff = memory.Memory.ToArray();
            await client.SendPacketAsync(testBuff, cancellationToken);
        }
    }

    extension(NewInputPacket packet)
    {
        public MinecraftPrimitiveReader CreateReader()
        {
            return new MinecraftPrimitiveReader(packet.Data);
        }
        
        
    }
}