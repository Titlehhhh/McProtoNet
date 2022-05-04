namespace ProtoLib.API.IO
{
    public interface IMinecraftStreamReader
    {
        long Length { get; }
        int Read(byte[] buffer, int offset, int count);
        bool ReadBoolean();        
        double ReadDouble();
        float ReadFloat();        
        Guid ReadGuid();
        int ReadInt();        
        long ReadLong();        
        short ReadShort();        
        sbyte ReadSignedByte();
        string ReadString(int maxLength = 32767);        
        byte[] ReadUInt8Array(int length = 0);       
        ulong[] ReadULongArray();    
        byte ReadUnsignedByte();        
        ulong ReadUnsignedLong();        
        ushort ReadUnsignedShort();        
        int ReadVarInt();        
        long ReadVarLong();        
    }
}
