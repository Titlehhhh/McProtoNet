using System.Diagnostics;
using System.Runtime.CompilerServices;
using McProtoNet.Abstractions;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public static class ReadExtensions
{
    public static async IAsyncEnumerable<TPacket> OnPacket<TPacket>(this IMinecraftClient client,
        CancellationToken cancellationToken) where TPacket : IServerPacket
    {
        await foreach (var packet in client.ReceivePackets(cancellationToken))
        {
            var id = PacketIdHelper.GetPacketId(client.ProtocolVersion, TPacket.PacketId);
            if (packet.Id == id && TPacket.IsSupportedVersionStatic(client.ProtocolVersion))
            {
                TPacket serverPacket =
                    (TPacket)PacketFactory.CreateClientboundPacket(client.ProtocolVersion, packet.Id, TPacket.PacketId.State);
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
                //if (PacketIdHelper.TryGetPacketIdentifier(p.Id, client.ProtocolVersion,
                //        state,
                //        PacketDirection.Clientbound, out var identifier))
                {
                    //Debug.WriteLine($"Not found packet");
                }
            }
            catch (Exception ex)
            {
                throw;
                if (PacketIdHelper.TryGetPacketIdentifier(p.Id, client.ProtocolVersion, state,
                        PacketDirection.Clientbound, out var identifier))
                {
                    Console.WriteLine($"Error in: {identifier}. Error: {ex}");
                    Debug.WriteLine($"Error in: {identifier}. Error: {ex}");
                }
            }

            if (packet is not null)
            {
                yield return packet;
            }
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
