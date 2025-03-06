using McProtoNet.NBT;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public static class Extensions
{
    public static NbtTag? ReadNbtTag(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        return reader.ReadNbtTag(protocolVersion < 764);
    }

    public static NbtTag? ReadOptionalNbtTag(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        return reader.ReadOptionalNbtTag(protocolVersion < 764);
    }

    public static Position ReadPosition(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        var locEncoded = reader.ReadSignedLong();
        int x, y, z;

        if (protocolVersion is < 340 or > 769)
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

        return new Position(x, y, z);
    }

    public static ChunkCoordinate ReadChunkCoordinate(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ulong bitfield = reader.ReadUnsignedLong();
        int x = (int)((bitfield >> 42) & 0x3FFFFF);
        int z = (int)((bitfield >> 20) & 0x3FFFFF);
        int y = (int)(bitfield & 0xFFFFF);
        // Sign extend if necessary
        if ((x & 0x200000) != 0) x |= ~0x3FFFFF;
        if ((z & 0x200000) != 0) z |= ~0x3FFFFF;
        if ((y & 0x80000) != 0) y |= ~0xFFFFF;

        return new ChunkCoordinate(x, z, y);
    }

    public static DeathLocation ReadDeathLocation(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        return new DeathLocation(reader.ReadString(), reader.ReadPosition(protocolVersion));
    }

    public static SpawnInfo ReadSpawnInfo(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        if (protocolVersion < 766)
        {
            throw new InvalidOperationException($"Protocol {protocolVersion} not supported.");
        }


        var dimension = reader.ReadVarInt();
        var name = reader.ReadString();
        var hashedSeed = reader.ReadSignedLong();
        var gamemode = reader.ReadUnsignedByte();
        var previousGamemode = reader.ReadUnsignedByte();
        var isDebug = reader.ReadBoolean();
        var isFlat = reader.ReadBoolean();
        var death = reader.ReadDeathLocation(protocolVersion);
        var portalCooldown = reader.ReadVarInt();
        int? seaLevel = null;

        if (protocolVersion >= 768)
        {
            seaLevel = reader.ReadVarInt();
        }

        return new SpawnInfo(dimension, name, hashedSeed, gamemode, previousGamemode, isDebug, isFlat, death,
            portalCooldown, seaLevel);
    }


    public static ChunkBlockEntity ReadChunkBlockEntity(this ref MinecraftPrimitiveReader reader, int protocolVersion)


    {
        if (protocolVersion is < 757 or > 769)
            throw new InvalidOperationException($"Protocol {protocolVersion} not supported.");

        byte packed = reader.ReadUnsignedByte();
        int x = packed >> 4;
        int z = packed & 0xF;
        short y = reader.ReadSignedShort();
        int type = reader.ReadVarInt();
        var nbtData = reader.ReadNbtTag(protocolVersion);

        return new ChunkBlockEntity((byte)x, (byte)z, y, type, nbtData);
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
            writer.WriteNbt(slot.Nbt);
        }
    }

    public static Slot? ReadSlot(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        if (reader.ReadBoolean())
            return new Slot(reader.ReadVarInt(), reader.ReadSignedByte(),
                reader.ReadNbtTag(protocolVersion));

        return null;
    }
}