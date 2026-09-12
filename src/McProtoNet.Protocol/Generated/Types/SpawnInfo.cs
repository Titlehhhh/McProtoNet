using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class SpawnInfo : IProtocolType<SpawnInfo>
{
    public int Dimension { get; }
    public string Name { get; }
    public long HashedSeed { get; }
    public Gamemode Gamemode { get; }
    public int PreviousGamemode { get; }
    public bool IsDebug { get; }
    public bool IsFlat { get; }
    public DeathLocation? Death { get; }
    public int PortalCooldown { get; }
    public int SeaLevel { get; }

    public SpawnInfo(int dimension, string name, long hashedSeed, Gamemode gamemode, int previousGamemode, bool isDebug, bool isFlat, DeathLocation? death, int portalCooldown, int seaLevel)
    {
        Dimension = dimension;
        Name = name;
        HashedSeed = hashedSeed;
        Gamemode = gamemode;
        PreviousGamemode = previousGamemode;
        IsDebug = isDebug;
        IsFlat = isFlat;
        Death = death;
        PortalCooldown = portalCooldown;
        SeaLevel = seaLevel;
    }

    public static SpawnInfo Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpawnInfo>(protocolVersion);
        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            var dimension = reader.ReadVarInt();
            var name = reader.ReadString();
            var hashedSeed = reader.ReadSignedLong();
            var gamemode = new Gamemode((int)reader.ReadSignedByte());
            var previousGamemode = reader.ReadUnsignedByte();
            var isDebug = reader.ReadBoolean();
            var isFlat = reader.ReadBoolean();
            DeathLocation? death = null;
            if (reader.ReadBoolean())
                death = reader.ReadType<DeathLocation>(protocolVersion);
            var portalCooldown = reader.ReadVarInt();
            return new SpawnInfo(dimension, name, hashedSeed, gamemode, previousGamemode, isDebug, isFlat, death, portalCooldown, default!);
        }

        if (protocolVersion >= 768)
        {
            var dimension = reader.ReadVarInt();
            var name = reader.ReadString();
            var hashedSeed = reader.ReadSignedLong();
            var gamemode = new Gamemode((int)reader.ReadSignedByte());
            var previousGamemode = reader.ReadUnsignedByte();
            var isDebug = reader.ReadBoolean();
            var isFlat = reader.ReadBoolean();
            DeathLocation? death = null;
            if (reader.ReadBoolean())
                death = reader.ReadType<DeathLocation>(protocolVersion);
            var portalCooldown = reader.ReadVarInt();
            var seaLevel = reader.ReadVarInt();
            return new SpawnInfo(dimension, name, hashedSeed, gamemode, previousGamemode, isDebug, isFlat, death, portalCooldown, seaLevel);
        }

        throw new System.NotSupportedException($"SpawnInfo has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpawnInfo>(protocolVersion);
        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            writer.WriteVarInt(Dimension);
            writer.WriteString(Name);
            writer.WriteSignedLong(HashedSeed);
            writer.WriteSignedByte((sbyte)Gamemode.Value);
            writer.WriteUnsignedByte((byte)PreviousGamemode);
            writer.WriteBoolean(IsDebug);
            writer.WriteBoolean(IsFlat);
            writer.WriteBoolean(Death is not null);
            if (Death is { } deathValue)
                writer.WriteType<DeathLocation>(deathValue, protocolVersion);
            writer.WriteVarInt(PortalCooldown);
            return;
        }

        if (protocolVersion >= 768)
        {
            writer.WriteVarInt(Dimension);
            writer.WriteString(Name);
            writer.WriteSignedLong(HashedSeed);
            writer.WriteSignedByte((sbyte)Gamemode.Value);
            writer.WriteUnsignedByte((byte)PreviousGamemode);
            writer.WriteBoolean(IsDebug);
            writer.WriteBoolean(IsFlat);
            writer.WriteBoolean(Death is not null);
            if (Death is { } deathValue)
                writer.WriteType<DeathLocation>(deathValue, protocolVersion);
            writer.WriteVarInt(PortalCooldown);
            writer.WriteVarInt(SeaLevel);
            return;
        }

        throw new System.NotSupportedException($"SpawnInfo has no wire layout for protocol version {protocolVersion}.");
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Dimension");
        writer.WriteNumberValue(Dimension);
        writer.WritePropertyName("Name");
        writer.WriteStringValue(Name);
        writer.WritePropertyName("HashedSeed");
        writer.WriteNumberValue(HashedSeed);
        writer.WritePropertyName("Gamemode");
        writer.WriteStringValue(Gamemode.ToString());
        writer.WritePropertyName("PreviousGamemode");
        writer.WriteNumberValue(PreviousGamemode);
        writer.WritePropertyName("IsDebug");
        writer.WriteBooleanValue(IsDebug);
        writer.WritePropertyName("IsFlat");
        writer.WriteBooleanValue(IsFlat);
        if (Death is { } deathValue)
        {
            writer.WritePropertyName("Death");
            deathValue.WriteJson(writer);
        }

        writer.WritePropertyName("PortalCooldown");
        writer.WriteNumberValue(PortalCooldown);
        writer.WritePropertyName("SeaLevel");
        writer.WriteNumberValue(SeaLevel);
        writer.WriteEndObject();
    }
}
