using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public static class Extensions
{
    public static Position ReadPosition(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        var locEncoded = reader.ReadSignedLong();
        int x, y, z;

        if (protocolVersion is <= 340 or >= 769)
        {
            throw new InvalidOperationException($"Protocol {protocolVersion} not supported.");
        }

        if (protocolVersion >= 477)
        {
            // Protocol 477-769: x(26) z(26) y(12)
            x = (int)(locEncoded >> 38);
            z = (int)((locEncoded >> 12) & 0x3FFFFFF);
            y = (int)(locEncoded & 0xFFF);
        }
        else
        {
            // Protocol 340-404: x(26) y(12) z(26)
            x = (int)(locEncoded >> 38);
            y = (int)((locEncoded >> 26) & 0xFFF);
            z = (int)(locEncoded & 0x3FFFFFF);
        }

        // Sign extend the values
        if (x >= 0x02000000) x -= 0x04000000;
        if (y >= 0x00000800) y -= 0x00001000;
        if (z >= 0x02000000) z -= 0x04000000;

        return new Position(x, z, y);
    }

    public static Vector2 ReadVector2(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        if (protocolVersion is >= 767 and <= 769)
        {
            float x = reader.ReadFloat();
            float y = reader.ReadFloat();
            return new Vector2(x, y);
        }

        throw new InvalidOperationException("Protocol version not supported");
    }

    public static Vector3F64 ReadVector3F64(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        if (protocolVersion is >= 762 and <= 769)
        {
            double x = reader.ReadDouble();
            double y = reader.ReadDouble();
            double z = reader.ReadDouble();
            return new Vector3F64(x, y, z);
        }

        throw new InvalidOperationException("Protocol version not supported");
    }

    // TODO ProtocolVersion
    public static void WritePosition(this scoped ref MinecraftPrimitiveWriter writer, Position position,
        int protocolVersion)
    {
        if (protocolVersion >= 477)
        {
            var a = (((ulong)position.X & 0x3FFFFFF) << 38) |
                    (((ulong)position.Z & 0x3FFFFFF) << 12) |
                    ((ulong)position.Y & 0xFFF);

            writer.WriteUnsignedLong(a);
        }
        else if (protocolVersion >= 340)
        {
            var a = (((ulong)position.X & 0x3FFFFFF) << 38) |
                    (((ulong)position.Y & 0xFFF) << 26) |
                    ((ulong)position.Z & 0x3FFFFFF);

            writer.WriteUnsignedLong(a);
        }
        else
        {
            throw new InvalidOperationException("Protocol version not supported");
        }
    }

    public static void WriteVector2(this scoped ref MinecraftPrimitiveWriter writer, Vector2 rotation,
        int protocolVersion)
    {
        if (protocolVersion is >= 767 and <= 769)
        {
            writer.WriteFloat(rotation.X);
            writer.WriteFloat(rotation.Y);
        }

        throw new InvalidOperationException("Protocol version not supported");
    }

    public static void WriteVector3F64(this scoped ref MinecraftPrimitiveWriter writer, Vector3F64 rotation,
        int protocolVersion)
    {
        if (protocolVersion is >= 762 and <= 769)
        {
            writer.WriteDouble(rotation.X);
            writer.WriteDouble(rotation.Y);
            writer.WriteDouble(rotation.Z);
        }
        else
        {
            throw new InvalidOperationException("Protocol version not supported");
        }
    }


    public static void WriteSlot(this ref MinecraftPrimitiveWriter writer, Slot? slot, int protocolVersion)
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