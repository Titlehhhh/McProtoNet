using McProtoNet.NBT;

namespace McProtoNet.API.IO
{
    public interface IMinecraftPrimitiveWriter
    {
        void Write(byte[] buffer);
        void WriteBoolean(bool value);
        void WriteByte(sbyte value);
        void WriteByteArray(byte[] values);
        void WriteDouble(double value);
        void WriteFloat(float value);
        void WriteInt(int value);
        void WriteLong(long value);
        void WriteLongArray(long[] values);
        void WriteNbt(NbtTag nbt);
        void WriteNbtCompound(NbtCompound compound);
        void WriteShort(short value);
        void WriteString(string value);
        void WriteUnsignedByte(byte value);
        void WriteUnsignedLong(ulong value);
        void WriteUnsignedShort(ushort value);
        void WriteULongArray(ulong[] value);
        void WriteUuid(Guid value);
        void WriteVarInt(Enum value);
        void WriteVarInt(int value);
        void WriteVarLong(long value);

    }
}
