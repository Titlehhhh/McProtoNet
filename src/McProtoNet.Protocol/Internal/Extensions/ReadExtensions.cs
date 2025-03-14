using System.Diagnostics;
using System.Runtime.CompilerServices;
using McProtoNet.Abstractions;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public static class ReadExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DeserializeCore(ref MinecraftPrimitiveReader reader, IServerPacket packet, int protocolVersion)
    {
        try
        {
            packet.Deserialize(ref reader, protocolVersion);
        }
        catch (Exception ex)
        {
            throw new PacketDeserializationException("Failed to deserialize packet", ex,
                packet.GetPacketId().ToString(), protocolVersion);
        }
    }

    public static async IAsyncEnumerable<TPacket> OnPacket<TPacket>(this IMinecraftClient client,
        CancellationToken cancellationToken) where TPacket : IServerPacket
    {
        await foreach (var packet in client.ReceivePackets(cancellationToken))
        {
            var id = PacketIdHelper.GetPacketId(client.ProtocolVersion, TPacket.PacketId);
            if (packet.Id == id && TPacket.IsSupportedVersionStatic(client.ProtocolVersion))
            {
                TPacket serverPacket =
                    (TPacket)PacketFactory.CreateClientboundPacket(client.ProtocolVersion, packet.Id,
                        TPacket.PacketId.State);
                var reader = new MinecraftPrimitiveReader(packet.Data);
                DeserializeCore(ref reader, serverPacket, client.ProtocolVersion);
                yield return serverPacket;
            }
        }
    }

    public static bool TryDeserialize<T>(this InputPacket inPacket, int protocolVersion, out T? outPacket)
        where T : IServerPacket
    {
        try
        {
            var id = PacketIdHelper.GetPacketId(protocolVersion, T.PacketId);
            if (inPacket.Id != id || !T.IsSupportedVersionStatic(protocolVersion))
            {
                outPacket = default;
                return false;
            }

            var serverPacket = PacketFactory.CreateClientboundPacket(protocolVersion, inPacket.Id, T.PacketId.State);
            var reader = new MinecraftPrimitiveReader(inPacket.Data);
            DeserializeCore(ref reader, serverPacket, protocolVersion);
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
            if (!PacketFactory.TryCreateClientboundPacket(client.ProtocolVersion, p.Id, state, out var packet))
                continue;
            var reader = new MinecraftPrimitiveReader(p.Data);
            DeserializeCore(ref reader, packet, client.ProtocolVersion);
            yield return packet;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ReadLength(this ref MinecraftPrimitiveReader reader, LengthFormat lengthFormat)
    {
        return lengthFormat switch
        {
            LengthFormat.VarInt => reader.ReadVarInt(),
            LengthFormat.Byte => reader.ReadUnsignedByte(),
            LengthFormat.Short => reader.ReadSignedShort(),
            LengthFormat.Int => reader.ReadSignedInt(),
            _ => throw new ArgumentOutOfRangeException(nameof(lengthFormat), lengthFormat, null)
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ReadBuffer(this ref MinecraftPrimitiveReader reader, LengthFormat lengthFormat)
    {
        var len = reader.ReadLength(lengthFormat);
        return reader.ReadBuffer(len);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] ReadArray<T>(this ref MinecraftPrimitiveReader reader, LengthFormat lengthFormat,
        ReadDelegate<T> readDelegate)

    {
        var len = reader.ReadLength(lengthFormat);
        return ReadArray(ref reader, len, readDelegate);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] ReadArray<T, TReader>(this ref MinecraftPrimitiveReader reader, LengthFormat lengthFormat)
        where TReader : IArrayReader<T>
    {
        int length = reader.ReadLength(lengthFormat);
        return ReadArray<T, TReader>(ref reader, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] ReadArray<T, TReader>(this ref MinecraftPrimitiveReader reader, int length)
        where TReader : IArrayReader<T>
    {
        T[] result = new T[length];
        TReader.Read(ref reader, 0, result);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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