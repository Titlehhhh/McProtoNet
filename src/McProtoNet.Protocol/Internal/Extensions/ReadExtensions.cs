using System.Runtime.CompilerServices;
using McProtoNet.Abstractions;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public static class ReadExtensions
{
    public static async IAsyncEnumerable<T> OnPacket<T>(this IMinecraftClient client,
        CancellationToken cancellationToken) where T : IServerPacket
    {
        await foreach (var packet in client.ReceivePackets(cancellationToken))
        {
            var id = PacketIdHelper.GetPacketId(client.ProtocolVersion, T.PacketId);
            if (packet.Id == id && T.IsSupportedVersionStatic(client.ProtocolVersion))
            {
                T serverPacket =
                    (T)PacketFactory.CreateClientboundPacket(client.ProtocolVersion, packet.Id, T.PacketId.State);
                MinecraftPrimitiveReader reader = new MinecraftPrimitiveReader(packet.Data);
                serverPacket.Deserialize(ref reader, client.ProtocolVersion);
                yield return serverPacket;
            }
        }
    }

    public static bool TryDeserialize<T>(this InputPacket inPacket, int protcolVersion, out T? outPacket)
        where T : IServerPacket
    {
        try
        {
            var id = PacketIdHelper.GetPacketId(protcolVersion, T.PacketId);
            if (inPacket.Id != id || !T.IsSupportedVersionStatic(protcolVersion))
            {
                outPacket = default;
                return false;
            }

            var serverPacket = PacketFactory.CreateClientboundPacket(protcolVersion, inPacket.Id, T.PacketId.State);
            MinecraftPrimitiveReader reader = new MinecraftPrimitiveReader(inPacket.Data);
            serverPacket.Deserialize(ref reader, protcolVersion);
            outPacket = (T)serverPacket;
            return true;
        }
        catch
        {
            outPacket = default;
            return false;
        }
    }


    public static async IAsyncEnumerable<IServerPacket> OnAllPackets(this IMinecraftClient client,
        PacketState state,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var p in client.ReceivePackets(cancellationToken))
        {
            IServerPacket? packet = null;
            try
            {
                packet = PacketFactory.CreateClientboundPacket(client.ProtocolVersion, p.Id, state);
                MinecraftPrimitiveReader reader = new MinecraftPrimitiveReader(p.Data);
                packet.Deserialize(ref reader, client.ProtocolVersion);
            }
            catch (KeyNotFoundException)
            {
            }

            if (packet is not null)
            {
                yield return packet;
            }
        }
    }


    private static int ReadLength(this ref MinecraftPrimitiveReader reader, LengthFormat lengthFormat)
    {
        switch (lengthFormat)
        {
            case LengthFormat.VarInt:
                return reader.ReadVarInt();
            case LengthFormat.Byte:
                return reader.ReadUnsignedByte();
            case LengthFormat.Short:
                return reader.ReadSignedShort();
            case LengthFormat.Int:
                return reader.ReadSignedInt();
            default:
                throw new ArgumentOutOfRangeException(nameof(lengthFormat), lengthFormat, null);
        }
    }

    public static byte[] ReadBuffer(this ref MinecraftPrimitiveReader reader, LengthFormat lengthFormat)
    {
        var len = reader.ReadLength(lengthFormat);
        return reader.ReadBuffer(len);
    }

    public static T[] ReadArray<T>(this ref MinecraftPrimitiveReader reader, int len,
        ReadDelegate<T> readDelegate)

    {
        if (len == 0)
            return [];

        T[] arr = new T[len];
        for (int i = 0; i < len; i++)
        {
            arr[i] = readDelegate(ref reader);
        }

        return arr;
    }

    public static T[] ReadArray<T>(this ref MinecraftPrimitiveReader reader, LengthFormat lengthFormat,
        ReadDelegate<T> readDelegate)

    {
        var len = reader.ReadLength(lengthFormat);
        return ReadArray(ref reader, len, readDelegate);
    }

    public static T[] ReadArray<T, TReader>(this ref MinecraftPrimitiveReader reader, LengthFormat lengthFormat)
        where TReader : IArrayReader<T>
    {
        int length = reader.ReadLength(lengthFormat);
        return ReadArray<T, TReader>(ref reader, length);
    }

    public static T[] ReadArray<T, TReader>(this ref MinecraftPrimitiveReader reader, int length)
        where TReader : IArrayReader<T>
    {
        T[] result = new T[length];
        TReader.Read(ref reader, 0, result);
        return result;
    }

    public static T? ReadOptional<T>(this ref MinecraftPrimitiveReader reader, ReadDelegate<T> readDelegate)
    {
        if (reader.ReadBoolean())
            return readDelegate(ref reader);
        return default;
    }
}

public interface IArrayReader<T>
{
    static abstract void Read(ref MinecraftPrimitiveReader reader, int protocolVersion, Span<T> destination);
}