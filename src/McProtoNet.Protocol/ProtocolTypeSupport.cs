using System;
using McProtoNet.NBT;
using McProtoNet.Primitives;
namespace McProtoNet.Protocol;

public interface IProtocolType<TSelf> where TSelf : IProtocolType<TSelf>
{
    static abstract TSelf Read(ref MinecraftPrimitiveReader reader, int protocolVersion);
    void Write(MinecraftPrimitiveWriter writer, int protocolVersion);
}

public static class ProtocolTypeExtensions
{
    public static T ReadType<T>(this ref MinecraftPrimitiveReader reader, int protocolVersion)
        where T : IProtocolType<T>
        => T.Read(ref reader, protocolVersion);

    public static void WriteType<T>(this MinecraftPrimitiveWriter writer, T value, int protocolVersion)
        where T : IProtocolType<T>
        => value.Write(writer, protocolVersion);

    public static byte[] ReadByteArray(this ref MinecraftPrimitiveReader reader)
        => reader.ReadBuffer(reader.ReadVarInt());

    public static void WriteByteArray(this MinecraftPrimitiveWriter writer, byte[] value)
    {
        writer.WriteVarInt(value.Length);
        writer.WriteBuffer(value);
    }

    public static byte[] ReadFixedBytes(this ref MinecraftPrimitiveReader reader, int length)
        => reader.ReadBuffer(length);

    public static void WriteFixedBytes(this MinecraftPrimitiveWriter writer, byte[] value, int length)
    {
        if (value.Length != length)
        {
            throw new ArgumentException(
                $"Expected exactly {length} bytes, got {value.Length}.", nameof(value));
        }

        writer.WriteBuffer(value);
    }

    public static byte[] ReadRestBytes(this ref MinecraftPrimitiveReader reader)
        => reader.ReadRestBuffer();

    public static void WriteRestBytes(this MinecraftPrimitiveWriter writer, byte[] value)
        => writer.WriteBuffer(value);
}
