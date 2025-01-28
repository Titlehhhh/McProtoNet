using System.Reactive.Linq;
using System.Runtime.InteropServices;
using McProtoNet.Abstractions;
using McProtoNet.Client;
using McProtoNet;
using McProtoNet.Serialization;
using static McProtoNet.Protocol.ReadDelegates;

namespace McProtoNet.Protocol;

public static class ReadExtensions
{
    public static IObservable<T> OnPacket<T>(this IMinecraftClient protocol) where T : IServerPacket
    {
        return protocol.OnPacket
            .Where(x =>
            {
                var id = PacketIdHelper.GetPacketId(protocol.ProtocolVersion, T.PacketId);
                return x.Id == id && T.IsSupportedVersionStatic(protocol.ProtocolVersion);
            })
            .Select(x =>
            {
                T packet = (T)PacketFactory.CreateClientboundPacket(protocol.ProtocolVersion, x.Id, T.PacketId.State);

                MinecraftPrimitiveReader reader = new MinecraftPrimitiveReader(x.Data);
                packet.Deserialize(ref reader, protocol.ProtocolVersion);
                return packet;
            });
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