using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Extensions;

public static partial class ProtocolSerializationExtensions
{
    public static SpawnInfo ReadSpawnInfo(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpawnInfo>(protocolVersion);
        int dimension = reader.ReadVarInt();
        string name = reader.ReadString();
        long hashedSeed = reader.ReadSignedLong();
        byte gamemode = reader.ReadUnsignedByte();
        byte previousGamemode = reader.ReadUnsignedByte();
        bool isDebug = reader.ReadBoolean();
        bool isFlat = reader.ReadBoolean();
        DeathLocation? death = null;
        if (reader.ReadBoolean())
        {
            death = reader.ReadDeathLocation(protocolVersion);
        }

        int portalCooldown = reader.ReadVarInt();
        int? seaLevel = null;
        if (protocolVersion >= 768)
        {
            seaLevel = reader.ReadVarInt();
        }

        return new SpawnInfo(dimension, name, hashedSeed, gamemode, previousGamemode, isDebug, isFlat, death,
            portalCooldown, seaLevel);
    }

    public static void WriteSpawnInfo(this ref MinecraftPrimitiveWriter writer, SpawnInfo value, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpawnInfo>(protocolVersion);
        writer.WriteVarInt(value.Dimension);
        writer.WriteString(value.Name);
        writer.WriteSignedLong(value.HashedSeed);
        writer.WriteUnsignedByte(value.Gamemode);
        writer.WriteUnsignedByte(value.PreviousGamemode);
        writer.WriteBoolean(value.IsDebug);
        writer.WriteBoolean(value.IsFlat);
        if (value.Death is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            writer.WriteDeathLocation(value.Death.Value, protocolVersion);
        }

        writer.WriteVarInt(value.PortalCooldown);
        if (protocolVersion >= 768)
        {
            writer.WriteVarInt(value.SeaLevel ?? 0);
        }
    }

    public static DeathLocation ReadDeathLocation(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DeathLocation>(protocolVersion);
        string dimensionName = reader.ReadString();
        Position location = reader.ReadPosition(protocolVersion);
        return new DeathLocation(dimensionName, location);
    }

    public static void WriteDeathLocation(this ref MinecraftPrimitiveWriter writer, DeathLocation value, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DeathLocation>(protocolVersion);
        writer.WriteString(value.DimensionName);
        writer.WritePosition(value.Location, protocolVersion);
    }
}
