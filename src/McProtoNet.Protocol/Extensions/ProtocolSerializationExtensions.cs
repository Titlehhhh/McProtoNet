using System;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Extensions;

public static partial class ProtocolSerializationExtensions
{
    private static T ReadTypeWithoutProtocolVersion<T>(ref MinecraftPrimitiveReader reader)
    {
        if (typeof(T) == typeof(bool)) return (T)(object)reader.ReadBoolean();
        if (typeof(T) == typeof(byte)) return (T)(object)reader.ReadUnsignedByte();
        if (typeof(T) == typeof(sbyte)) return (T)(object)reader.ReadSignedByte();
        if (typeof(T) == typeof(short)) return (T)(object)reader.ReadSignedShort();
        if (typeof(T) == typeof(ushort)) return (T)(object)reader.ReadUnsignedShort();
        if (typeof(T) == typeof(int)) return (T)(object)reader.ReadSignedInt();
        if (typeof(T) == typeof(uint)) return (T)(object)reader.ReadUnsignedInt();
        if (typeof(T) == typeof(long)) return (T)(object)reader.ReadSignedLong();
        if (typeof(T) == typeof(ulong)) return (T)(object)reader.ReadUnsignedLong();
        if (typeof(T) == typeof(float)) return (T)(object)reader.ReadFloat();
        if (typeof(T) == typeof(double)) return (T)(object)reader.ReadDouble();
        if (typeof(T) == typeof(string)) return (T)(object)reader.ReadString();
        if (typeof(T) == typeof(Guid)) return (T)(object)reader.ReadUUID();

        throw new NotSupportedException(
            $"ReadArray<{typeof(T).Name}> without protocolVersion is not supported. Use ReadArray<T>(..., int).");
    }

    private static void WriteTypeWithoutProtocolVersion<T>(MinecraftPrimitiveWriter writer, T value)
    {
        if (typeof(T) == typeof(bool)) { writer.WriteBoolean((bool)(object)value!); return; }
        if (typeof(T) == typeof(byte)) { writer.WriteUnsignedByte((byte)(object)value!); return; }
        if (typeof(T) == typeof(sbyte)) { writer.WriteSignedByte((sbyte)(object)value!); return; }
        if (typeof(T) == typeof(short)) { writer.WriteSignedShort((short)(object)value!); return; }
        if (typeof(T) == typeof(ushort)) { writer.WriteUnsignedShort((ushort)(object)value!); return; }
        if (typeof(T) == typeof(int)) { writer.WriteSignedInt((int)(object)value!); return; }
        if (typeof(T) == typeof(uint)) { writer.WriteUnsignedInt((uint)(object)value!); return; }
        if (typeof(T) == typeof(long)) { writer.WriteSignedLong((long)(object)value!); return; }
        if (typeof(T) == typeof(ulong)) { writer.WriteUnsignedLong((ulong)(object)value!); return; }
        if (typeof(T) == typeof(float)) { writer.WriteFloat((float)(object)value!); return; }
        if (typeof(T) == typeof(double)) { writer.WriteDouble((double)(object)value!); return; }
        if (typeof(T) == typeof(string)) { writer.WriteString((string)(object)value!); return; }
        if (typeof(T) == typeof(Guid)) { writer.WriteUUID((Guid)(object)value!); return; }

        throw new NotSupportedException(
            $"WriteArray<{typeof(T).Name}> without protocolVersion is not supported. Use WriteArray<T>(..., int).");
    }
}
