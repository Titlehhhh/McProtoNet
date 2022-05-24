namespace McProtoNet.API.IO
{
    public interface IMinecraftPrimitiveReader
    {
        byte[] ReadToEnd();
        bool ReadBoolean();
        double ReadDouble();
        float ReadFloat();
        Guid ReadGuid();
        int ReadInt();
        long ReadLong();
        short ReadShort();
        sbyte ReadSignedByte();
        string ReadString(int maxLength = 32767);
        byte[] ReadByteArray();
        ulong[] ReadULongArray();
        byte ReadUnsignedByte();
        ulong ReadUnsignedLong();
        ushort ReadUnsignedShort();
        int ReadVarInt();
        long ReadVarLong();
    }
}
