using System.Buffers;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using DotNext.Buffers;
using DotNext.Hosting;
using DotNext.IO.Pipelines;
using McProtoNet.Client;
using McProtoNet.Net;
using McProtoNet.Net.Zlib;
using McProtoNet.Serialization;

namespace SampleBotCSharp;

class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Hi");
        using ConsoleLifetimeTokenSource clts = new();
        await using var gg = clts.Token.Register(() => { Console.WriteLine("Отмена..."); });
        List<Task> tasks = new List<Task>();
        for (int i = 0; i < 130; i++)
        {
            var nick = $"McProto_{i:D3}";
            var bot = Task.Run(async () =>
            {
                try
                {
                    for (int j = 1; j <= 3; j++)
                    {
                        var time = TimeSpan.FromSeconds(j*20D * Random.Shared.NextDouble());
                        await Task.Delay(time, clts.Token);
                        await RunBot(nick, clts.Token);
                    }
                }
                catch (OperationCanceledException) when (clts.IsCancellationRequested)
                {
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
            });
            tasks.Add(bot);
        }

        await Task.WhenAll(tasks);
        Console.WriteLine("Успешно завершено. Нажмите кнопку для остановки");
        Console.ReadLine();
    }

    private static async Task RunBot(string nick, CancellationToken cancellationToken)
    {
        var tcp = new TcpClient();

        await tcp.ConnectAsync("127.0.0.1", 25565, cancellationToken);

        var stream = tcp.GetStream();

        await using var client =
            PipelinesMinecraftClient.Create(stream, 772);

        var handshake = new HandshakePacket
        {
            ProtocolVersion = 772,
            NextState = 2,
            ServerHost = "127.0.0.1",
            ServerPort = 25565
        };
        await client.SendPacketAsync(handshake, 0x00, cancellationToken);
        await client.SendPacketAsync(new LoginStartPacket
        {
            Name = nick,
            UUID = Guid.NewGuid()
        }, 0x00, cancellationToken);

        await foreach (var p in client.ReadPacketsAsync(cancellationToken))
        {
            if (p.Id == 0x03)
            {
                var reader = p.CreateReader();
                int threshold = reader.ReadVarInt();
                client.CompressionThreshold = threshold;
            }

            if (p.Id == 0x02)
            {
                await client.SendEmptyPacketAsync(0x03, cancellationToken);
                break;
            }
        }


        await client.SendPacketAsync((ref writer, version) =>
        {
            writer.WriteString("ru_RU");
            writer.WriteSignedByte(16);
            writer.WriteVarInt(0);
            writer.WriteBoolean(true);
            writer.WriteUnsignedByte(127);
            writer.WriteVarInt(0);
            writer.WriteBoolean(false);
            writer.WriteBoolean(false);
            writer.WriteVarInt(0);
        }, 0x00, cancellationToken);
        await foreach (var p in client.ReadPacketsAsync(cancellationToken))
        {
            if (p.Id == 0x04) //KeepAlive
            {
                await client.SendPacketAsync((ref writer, _) => { writer.Write(p.Data); }, 0x04, cancellationToken);
            }
            else if (p.Id == 0x01) //Payload
            {
                await client.SendPacketAsync((ref writer, _) => { writer.Write(p.Data); }, 0x02, cancellationToken);
            }
            else if (p.Id == 0x0E)
            {
                await client.SendPacketAsync((ref writer, _) => { writer.WriteVarInt(0); }, 0x07, cancellationToken);
            }
            else if (p.Id == 0x03)
            {
                await client.SendEmptyPacketAsync(0x03, cancellationToken);
                break;
            }
        }

        await foreach (var p in client.ReadPacketsAsync(cancellationToken))
        {
            if (p.Id == 0x26) //KeepAlive
            {
                await client.SendPacketAsync(
                    (ref writer, _) => { writer.Write(p.Data); }, 0x1B, cancellationToken);
            }
        }
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
    public delegate void WriteAction(ref MinecraftPrimitiveWriter writer, int protocolVersion);

    extension(PipelinesMinecraftClient client)
    {
        public async ValueTask SendPacketAsync(IPacket packet, int id, CancellationToken cancellationToken = default)
        {
            Span<byte> buffer = stackalloc byte[128];
            var writer = new MinecraftPrimitiveWriter(buffer);
            try
            {
                writer.WriteVarInt(id);
                packet.Serialize(ref writer, client.ProtocolVersion);
                client.PacketWriter.WritePacket(writer.WrittenSpan);
            }
            finally
            {
                writer.Dispose();
            }


            var result = await client.PacketWriter.FlushAsync(cancellationToken);
            result.ThrowIfCancellationRequested(cancellationToken);
            if (result.IsCompleted)
            {
                throw new InvalidOperationException("Flush failed");
            }
        }

        public async ValueTask SendPacketAsync(WriteAction write, int id, CancellationToken cancellationToken = default)
        {
            var writer = new MinecraftPrimitiveWriter();
            try
            {
                writer.WriteVarInt(id);
                write(ref writer, client.ProtocolVersion);
            }
            catch
            {
                writer.Dispose();
                throw;
            }

            using var memory = writer.GetWrittenMemory();
            await client.SendPacketAsync(memory.Memory, cancellationToken);
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