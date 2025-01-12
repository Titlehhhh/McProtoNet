using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public static class Extensions
{
    // TODO ProtocolVersion
    public static Position ReadPosition(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        
        var locEncoded = reader.ReadSignedLong();


        int x, y, z;
        //if (protocolversion >= Protocol18Handler.MC_1_14_Version)
        //{
        x = (int)(locEncoded >> 38);
        y = (int)(locEncoded & 4095);
        z = (int)((locEncoded << 26) >> 38);
        //}
        //else
        //{
        //	x = (int)(locEncoded >> 38);
        //	y = (int)((locEncoded >> 26) & 0xFFF);
        //	z = (int)(locEncoded << 38 >> 38);
        //}

        if (x >= 0x02000000) // 33,554,432
            x -= 0x04000000; // 67,108,864
        if (y >= 0x00000800) //      2,048
            y -= 0x00001000; //      4,096
        if (z >= 0x02000000) // 33,554,432
            z -= 0x04000000; // 67,108,864


        return new Position(x, z, y);
    }

    public static Vector2 ReadVector2(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        throw new NotImplementedException();
    }

    public static Vector3F64 ReadVector3F64(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        throw new NotImplementedException();
    }

    // TODO ProtocolVersion
    public static void WritePosition(this scoped ref MinecraftPrimitiveWriter writer, Position position, int protocolVersion)
    {
        var a = (((ulong)position.X & 0x3FFFFFF) << 38) |
                (((ulong)position.Z & 0x3FFFFFF) << 12) |
                ((ulong)position.Y & 0xFFF);
        // var g = BitConverter.GetBytes(a);

        // Array.Reverse(g);
        writer.WriteUnsignedLong(a);
        //writer.WriteBuffer(g);
    }
    
    public static void WriteVector2(this scoped ref MinecraftPrimitiveWriter writer, Vector2 rotation, int protocolVersion)
    {
        throw new NotImplementedException();
    }
    
    public static void WriteVector3F64(this scoped ref MinecraftPrimitiveWriter writer, Vector3F64 rotation, int protocolVersion)
    {
        throw new NotImplementedException();
    }


    public static void WriteSlot(this ref MinecraftPrimitiveWriter writer, Slot? slot,int protocolVersion)
    {
        if (slot is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            writer.WriteVarInt(slot.ItemId);
            writer.WriteSignedByte(slot.ItemCount);
            writer.WriteOptionalNbt(slot.Nbt);
        }
    }

    public static Slot? ReadSlot(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        if (reader.ReadBoolean())
            return new Slot(reader.ReadVarInt(), reader.ReadSignedByte(),
                reader.ReadOptionalNbtTag(protocolVersion < 764));

        return null;
    }
}