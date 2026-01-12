using DotNext.IO.Pipelines;
using McProtoNet.Client;
using McProtoNet.Net;
using McProtoNet.Serialization;

namespace SampleBotCSharp;

static class Ext
{
    public delegate void WriteAction(ref MinecraftPrimitiveWriter writer, int protocolVersion);

    extension(PipelinesMinecraftClient client)
    {
        public async ValueTask SendChatAsync(string message, CancellationToken cancellationToken = default)
        {
            await client.SendPacketAsync((ref writer, _) =>
            {
                writer.WriteString(message);
                writer.WriteSignedLong(DateTimeOffset.UtcNow.Ticks);
                writer.WriteSignedLong(0);
                writer.WriteBoolean(false);
                writer.WriteVarInt(0);
                var bitset = (int)Math.Ceiling(20d / 8d);
                writer.WriteBuffer(new byte[bitset]);
                writer.WriteUnsignedByte(0);
            }, 0x08, cancellationToken);
        }
        
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

    extension(Random random)
    {
        public TimeSpan NextTimeSpan(int min, int max)
        {
            return TimeSpan.FromMilliseconds(random.Next(min, max));
        }
    }
}