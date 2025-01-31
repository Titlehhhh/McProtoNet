using System.Diagnostics;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using McProtoNet.Abstractions;
using McProtoNet.Client;
using McProtoNet;
using McProtoNet.Serialization;
using static McProtoNet.Protocol.ReadDelegates;

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

    public static async IAsyncEnumerable<IServerPacket> OnAllPackets(this IMinecraftClient client, PacketState state,
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
            catch (Exception e)
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
        int len = reader.ReadLength(lengthFormat);
        return reader.ReadBuffer(len);
    }

    public static T[] ReadArray<T>(this ref MinecraftPrimitiveReader reader, int len,
        ReadDelegate<T> readDelegate)

    {
        if (len == 0)
            return [];

        if (ReferenceEquals(readDelegate, ReadDelegates.Byte))
        {
            return (T[])(object)reader.ReadBuffer(len);
        }

        if (ReferenceEquals(readDelegate, ReadDelegates.SByte))
        {
            ReadOnlySpan<byte> buff = reader.Read(len);
            ReadOnlySpan<sbyte> casted = MemoryMarshal.Cast<byte, sbyte>(buff);
            return (T[])(object)casted.ToArray();
        }

        if (ReferenceEquals(readDelegate, ReadDelegates.Int16))
        {
            return (T[])(object)reader.ReadArrayInt16BigEndian(len);
        }

        if (ReferenceEquals(readDelegate, ReadDelegates.UInt16))
        {
            return (T[])(object)reader.ReadArrayUnsignedInt16BigEndian(len);
        }

        if (ReferenceEquals(readDelegate, ReadDelegates.Int32))
        {
            return (T[])(object)reader.ReadArrayInt32BigEndian(len);
        }

        if (ReferenceEquals(readDelegate, ReadDelegates.UInt32))
        {
            return (T[])(object)reader.ReadArrayUnsignedInt32BigEndian(len);
        }

        if (ReferenceEquals(readDelegate, ReadDelegates.Int64))
        {
            return (T[])(object)reader.ReadArrayInt64BigEndian(len);
        }

        if (ReferenceEquals(readDelegate, ReadDelegates.UInt64))
        {
            return (T[])(object)reader.ReadArrayUnsignedInt64BigEndian(len);
        }

        if (ReferenceEquals(readDelegate, ReadDelegates.Float))
        {
            return (T[])(object)reader.ReadArrayFloatBigEndian(len);
        }

        if (ReferenceEquals(readDelegate, ReadDelegates.Double))
        {
            return (T[])(object)reader.ReadArrayDoubleBigEndian(len);
        }

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
        int len = reader.ReadLength(lengthFormat);
        return ReadArray<T>(ref reader, len, readDelegate);
    }

    public static T? ReadOptional<T>(this MinecraftPrimitiveReader reader, ReadDelegate<T> readDelegate)
    {
        if (reader.ReadBoolean())
            return readDelegate(ref reader);
        return default;
    }
}