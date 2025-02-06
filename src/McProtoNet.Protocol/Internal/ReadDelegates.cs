using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public delegate T ReadDelegate<out T>(ref MinecraftPrimitiveReader reader);

public static class ReadDelegates
{
    public static readonly ReadDelegate<byte> Byte = (ref MinecraftPrimitiveReader reader) => reader.ReadUnsignedByte();
    public static readonly ReadDelegate<sbyte> SByte = (ref MinecraftPrimitiveReader reader) => reader.ReadSignedByte();

    public static readonly ReadDelegate<int> VarInt = (ref MinecraftPrimitiveReader reader) => reader.ReadVarInt();
    public static readonly ReadDelegate<long> VarLong = (ref MinecraftPrimitiveReader reader) => reader.ReadVarLong();

    public static readonly ReadDelegate<int> Int32 = (ref MinecraftPrimitiveReader reader) => reader.ReadSignedInt();

    public static readonly ReadDelegate<uint>
        UInt32 = (ref MinecraftPrimitiveReader reader) => reader.ReadUnsignedInt();

    public static readonly ReadDelegate<long> Int64 = (ref MinecraftPrimitiveReader reader) => reader.ReadSignedLong();

    public static readonly ReadDelegate<ulong> UInt64 = (ref MinecraftPrimitiveReader reader) =>
        reader.ReadUnsignedLong();

    public static readonly ReadDelegate<short>
        Int16 = (ref MinecraftPrimitiveReader reader) => reader.ReadSignedShort();

    public static readonly ReadDelegate<ushort> UInt16 = (ref MinecraftPrimitiveReader reader) =>
        reader.ReadUnsignedShort();

    public static readonly ReadDelegate<float> Float = (ref MinecraftPrimitiveReader reader) => reader.ReadFloat();
    public static readonly ReadDelegate<double> Double = (ref MinecraftPrimitiveReader reader) => reader.ReadDouble();

    public static readonly ReadDelegate<string> String = (ref MinecraftPrimitiveReader reader) => reader.ReadString();
}