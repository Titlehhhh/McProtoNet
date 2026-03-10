using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("Login", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class LoginPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 736),
        new(751, 754),
        new(755, 756),
        new(757, 758),
        new(759, 762),
        new(763, 763),
        new(764, 765),
        new(766, MinecraftVersion.LatestProtocol),
    };

    public VFirst_736Fields? VFirst_736 { get; set; }
    public V751_754Fields? V751_754 { get; set; }
    public V755_756Fields? V755_756 { get; set; }
    public V757_758Fields? V757_758 { get; set; }
    public V759_762Fields? V759_762 { get; set; }
    public V763Fields? V763 { get; set; }
    public V764_765Fields? V764_765 { get; set; }
    public V766_LastFields? V766_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 736:
            {
                var fields = VFirst_736 ?? throw new InvalidOperationException("Login VFirst_736 fields missing.");
                writer.WriteSignedInt(fields.EntityId);
                writer.WriteUnsignedByte(fields.GameMode);
                writer.WriteUnsignedByte(fields.PreviousGameMode);
                writer.WriteArray(fields.WorldNames, LengthFormat.VarInt);
                writer.WriteNbtTag(fields.DimensionCodec ?? throw new InvalidOperationException("Login dimensionCodec missing."), protocolVersion);
                writer.WriteString(fields.Dimension);
                writer.WriteString(fields.WorldName);
                writer.WriteSignedLong(fields.HashedSeed);
                writer.WriteUnsignedByte(fields.MaxPlayers);
                writer.WriteVarInt(fields.ViewDistance);
                writer.WriteBoolean(fields.ReducedDebugInfo);
                writer.WriteBoolean(fields.EnableRespawnScreen);
                writer.WriteBoolean(fields.IsDebug);
                writer.WriteBoolean(fields.IsFlat);
                return;
            }
            case >= 751 and <= 754:
            {
                var fields = V751_754 ?? throw new InvalidOperationException("Login V751_754 fields missing.");
                writer.WriteSignedInt(fields.EntityId);
                writer.WriteBoolean(fields.IsHardcore);
                writer.WriteArray(fields.WorldNames, LengthFormat.VarInt);
                writer.WriteNbtTag(fields.DimensionCodec ?? throw new InvalidOperationException("Login dimensionCodec missing."), protocolVersion);
                writer.WriteNbtTag(fields.Dimension ?? throw new InvalidOperationException("Login dimension missing."), protocolVersion);
                writer.WriteString(fields.WorldName);
                writer.WriteSignedLong(fields.HashedSeed);
                writer.WriteVarInt(fields.MaxPlayers);
                writer.WriteVarInt(fields.ViewDistance);
                writer.WriteBoolean(fields.ReducedDebugInfo);
                writer.WriteBoolean(fields.EnableRespawnScreen);
                writer.WriteBoolean(fields.IsDebug);
                writer.WriteBoolean(fields.IsFlat);
                return;
            }
            case >= 759 and <= 762:
            {
                var fields = V759_762 ?? throw new InvalidOperationException("Login V759_762 fields missing.");
                writer.WriteSignedInt(fields.EntityId);
                writer.WriteBoolean(fields.IsHardcore);
                writer.WriteUnsignedByte(fields.GameMode);
                writer.WriteSignedByte(fields.PreviousGameMode);
                writer.WriteArray(fields.WorldNames, LengthFormat.VarInt);
                writer.WriteNbtTag(fields.DimensionCodec ?? throw new InvalidOperationException("Login dimensionCodec missing."), protocolVersion);
                writer.WriteString(fields.WorldType);
                writer.WriteString(fields.WorldName);
                writer.WriteSignedLong(fields.HashedSeed);
                writer.WriteVarInt(fields.MaxPlayers);
                writer.WriteVarInt(fields.ViewDistance);
                writer.WriteVarInt(fields.SimulationDistance);
                writer.WriteBoolean(fields.ReducedDebugInfo);
                writer.WriteBoolean(fields.EnableRespawnScreen);
                writer.WriteBoolean(fields.IsDebug);
                writer.WriteBoolean(fields.IsFlat);
                if (fields.Death is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteDeathLocation(fields.Death.Value, protocolVersion);
                }
                return;
            }
            case 763:
            {
                var fields = V763 ?? throw new InvalidOperationException("Login V763 fields missing.");
                writer.WriteSignedInt(fields.EntityId);
                writer.WriteBoolean(fields.IsHardcore);
                writer.WriteUnsignedByte(fields.GameMode);
                writer.WriteSignedByte(fields.PreviousGameMode);
                writer.WriteArray(fields.WorldNames, LengthFormat.VarInt);
                writer.WriteNbtTag(fields.DimensionCodec ?? throw new InvalidOperationException("Login dimensionCodec missing."), protocolVersion);
                writer.WriteString(fields.WorldType);
                writer.WriteString(fields.WorldName);
                writer.WriteSignedLong(fields.HashedSeed);
                writer.WriteVarInt(fields.MaxPlayers);
                writer.WriteVarInt(fields.ViewDistance);
                writer.WriteVarInt(fields.SimulationDistance);
                writer.WriteBoolean(fields.ReducedDebugInfo);
                writer.WriteBoolean(fields.EnableRespawnScreen);
                writer.WriteBoolean(fields.IsDebug);
                writer.WriteBoolean(fields.IsFlat);
                if (fields.Death is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteDeathLocation(fields.Death.Value, protocolVersion);
                }
                writer.WriteVarInt(fields.PortalCooldown);
                return;
            }
            case >= 764 and <= 765:
            {
                var fields = V764_765 ?? throw new InvalidOperationException("Login V764_765 fields missing.");
                writer.WriteSignedInt(fields.EntityId);
                writer.WriteBoolean(fields.IsHardcore);
                writer.WriteArray(fields.WorldNames, LengthFormat.VarInt);
                writer.WriteVarInt(fields.MaxPlayers);
                writer.WriteVarInt(fields.ViewDistance);
                writer.WriteVarInt(fields.SimulationDistance);
                writer.WriteBoolean(fields.ReducedDebugInfo);
                writer.WriteBoolean(fields.EnableRespawnScreen);
                writer.WriteBoolean(fields.DoLimitedCrafting);
                writer.WriteString(fields.WorldType);
                writer.WriteString(fields.WorldName);
                writer.WriteSignedLong(fields.HashedSeed);
                writer.WriteUnsignedByte(fields.GameMode);
                writer.WriteSignedByte(fields.PreviousGameMode);
                writer.WriteBoolean(fields.IsDebug);
                writer.WriteBoolean(fields.IsFlat);
                if (fields.Death is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteDeathLocation(fields.Death.Value, protocolVersion);
                }
                writer.WriteVarInt(fields.PortalCooldown);
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V766_Last ?? throw new InvalidOperationException("Login V766_Last fields missing.");
                writer.WriteSignedInt(fields.EntityId);
                writer.WriteBoolean(fields.IsHardcore);
                writer.WriteArray(fields.WorldNames, LengthFormat.VarInt);
                writer.WriteVarInt(fields.MaxPlayers);
                writer.WriteVarInt(fields.ViewDistance);
                writer.WriteVarInt(fields.SimulationDistance);
                writer.WriteBoolean(fields.ReducedDebugInfo);
                writer.WriteBoolean(fields.EnableRespawnScreen);
                writer.WriteBoolean(fields.DoLimitedCrafting);
                writer.WriteSpawnInfo(fields.WorldState, protocolVersion);
                writer.WriteBoolean(fields.EnforcesSecureChat);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.Login), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 736:
                VFirst_736 = new VFirst_736Fields
                {
                    EntityId = reader.ReadSignedInt(),
                    GameMode = reader.ReadUnsignedByte(),
                    PreviousGameMode = reader.ReadUnsignedByte(),
                    WorldNames = reader.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader r) => r.ReadString()),
                    DimensionCodec = reader.ReadNbtTag(protocolVersion),
                    Dimension = reader.ReadString(),
                    WorldName = reader.ReadString(),
                    HashedSeed = reader.ReadSignedLong(),
                    MaxPlayers = reader.ReadUnsignedByte(),
                    ViewDistance = reader.ReadVarInt(),
                    ReducedDebugInfo = reader.ReadBoolean(),
                    EnableRespawnScreen = reader.ReadBoolean(),
                    IsDebug = reader.ReadBoolean(),
                    IsFlat = reader.ReadBoolean()
                };
                return;
            case >= 751 and <= 754:
                V751_754 = new V751_754Fields
                {
                    EntityId = reader.ReadSignedInt(),
                    IsHardcore = reader.ReadBoolean(),
                    GameMode = reader.ReadUnsignedByte(),
                    PreviousGameMode = reader.ReadUnsignedByte(),
                    WorldNames = reader.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader r) => r.ReadString()),
                    DimensionCodec = reader.ReadNbtTag(protocolVersion),
                    Dimension = reader.ReadNbtTag(protocolVersion),
                    WorldName = reader.ReadString(),
                    HashedSeed = reader.ReadSignedLong(),
                    MaxPlayers = reader.ReadVarInt(),
                    ViewDistance = reader.ReadVarInt(),
                    ReducedDebugInfo = reader.ReadBoolean(),
                    EnableRespawnScreen = reader.ReadBoolean(),
                    IsDebug = reader.ReadBoolean(),
                    IsFlat = reader.ReadBoolean()
                };
                return;
            case >= 755 and <= 756:
                V755_756 = new V755_756Fields
                {
                    EntityId = reader.ReadSignedInt(),
                    IsHardcore = reader.ReadBoolean(),
                    GameMode = reader.ReadUnsignedByte(),
                    PreviousGameMode = reader.ReadSignedByte(),
                    WorldNames = reader.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader r) => r.ReadString()),
                    DimensionCodec = reader.ReadNbtTag(protocolVersion),
                    Dimension = reader.ReadNbtTag(protocolVersion),
                    WorldName = reader.ReadString(),
                    HashedSeed = reader.ReadSignedLong(),
                    MaxPlayers = reader.ReadVarInt(),
                    ViewDistance = reader.ReadVarInt(),
                    ReducedDebugInfo = reader.ReadBoolean(),
                    EnableRespawnScreen = reader.ReadBoolean(),
                    IsDebug = reader.ReadBoolean(),
                    IsFlat = reader.ReadBoolean()
                };
                return;
            case >= 757 and <= 758:
                V757_758 = new V757_758Fields
                {
                    EntityId = reader.ReadSignedInt(),
                    IsHardcore = reader.ReadBoolean(),
                    GameMode = reader.ReadUnsignedByte(),
                    PreviousGameMode = reader.ReadSignedByte(),
                    WorldNames = reader.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader r) => r.ReadString()),
                    DimensionCodec = reader.ReadNbtTag(protocolVersion),
                    Dimension = reader.ReadNbtTag(protocolVersion),
                    WorldName = reader.ReadString(),
                    HashedSeed = reader.ReadSignedLong(),
                    MaxPlayers = reader.ReadVarInt(),
                    ViewDistance = reader.ReadVarInt(),
                    SimulationDistance = reader.ReadVarInt(),
                    ReducedDebugInfo = reader.ReadBoolean(),
                    EnableRespawnScreen = reader.ReadBoolean(),
                    IsDebug = reader.ReadBoolean(),
                    IsFlat = reader.ReadBoolean()
                };
                return;
            case >= 759 and <= 762:
                V759_762 = new V759_762Fields
                {
                    EntityId = reader.ReadSignedInt(),
                    IsHardcore = reader.ReadBoolean(),
                    GameMode = reader.ReadUnsignedByte(),
                    PreviousGameMode = reader.ReadSignedByte(),
                    WorldNames = reader.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader r) => r.ReadString()),
                    DimensionCodec = reader.ReadNbtTag(protocolVersion),
                    WorldType = reader.ReadString(),
                    WorldName = reader.ReadString(),
                    HashedSeed = reader.ReadSignedLong(),
                    MaxPlayers = reader.ReadVarInt(),
                    ViewDistance = reader.ReadVarInt(),
                    SimulationDistance = reader.ReadVarInt(),
                    ReducedDebugInfo = reader.ReadBoolean(),
                    EnableRespawnScreen = reader.ReadBoolean(),
                    IsDebug = reader.ReadBoolean(),
                    IsFlat = reader.ReadBoolean(),
                    Death = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadDeathLocation(protocolVersion))
                };
                return;
            case 763:
                V763 = new V763Fields
                {
                    EntityId = reader.ReadSignedInt(),
                    IsHardcore = reader.ReadBoolean(),
                    GameMode = reader.ReadUnsignedByte(),
                    PreviousGameMode = reader.ReadSignedByte(),
                    WorldNames = reader.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader r) => r.ReadString()),
                    DimensionCodec = reader.ReadNbtTag(protocolVersion),
                    WorldType = reader.ReadString(),
                    WorldName = reader.ReadString(),
                    HashedSeed = reader.ReadSignedLong(),
                    MaxPlayers = reader.ReadVarInt(),
                    ViewDistance = reader.ReadVarInt(),
                    SimulationDistance = reader.ReadVarInt(),
                    ReducedDebugInfo = reader.ReadBoolean(),
                    EnableRespawnScreen = reader.ReadBoolean(),
                    IsDebug = reader.ReadBoolean(),
                    IsFlat = reader.ReadBoolean(),
                    Death = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadDeathLocation(protocolVersion)),
                    PortalCooldown = reader.ReadVarInt()
                };
                return;
            case >= 764 and <= 765:
                V764_765 = new V764_765Fields
                {
                    EntityId = reader.ReadSignedInt(),
                    IsHardcore = reader.ReadBoolean(),
                    WorldNames = reader.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader r) => r.ReadString()),
                    MaxPlayers = reader.ReadVarInt(),
                    ViewDistance = reader.ReadVarInt(),
                    SimulationDistance = reader.ReadVarInt(),
                    ReducedDebugInfo = reader.ReadBoolean(),
                    EnableRespawnScreen = reader.ReadBoolean(),
                    DoLimitedCrafting = reader.ReadBoolean(),
                    WorldType = reader.ReadString(),
                    WorldName = reader.ReadString(),
                    HashedSeed = reader.ReadSignedLong(),
                    GameMode = reader.ReadUnsignedByte(),
                    PreviousGameMode = reader.ReadSignedByte(),
                    IsDebug = reader.ReadBoolean(),
                    IsFlat = reader.ReadBoolean(),
                    Death = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadDeathLocation(protocolVersion)),
                    PortalCooldown = reader.ReadVarInt()
                };
                return;
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                V766_Last = new V766_LastFields
                {
                    EntityId = reader.ReadSignedInt(),
                    IsHardcore = reader.ReadBoolean(),
                    WorldNames = reader.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader r) => r.ReadString()),
                    MaxPlayers = reader.ReadVarInt(),
                    ViewDistance = reader.ReadVarInt(),
                    SimulationDistance = reader.ReadVarInt(),
                    ReducedDebugInfo = reader.ReadBoolean(),
                    EnableRespawnScreen = reader.ReadBoolean(),
                    DoLimitedCrafting = reader.ReadBoolean(),
                    WorldState = reader.ReadSpawnInfo(protocolVersion),
                    EnforcesSecureChat = reader.ReadBoolean()
                };
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.Login), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_736Fields
    {
        public int EntityId { get; set; }
        public byte GameMode { get; set; }
        public byte PreviousGameMode { get; set; }
        public string[] WorldNames { get; set; }
        public NbtTag? DimensionCodec { get; set; }
        public string Dimension { get; set; }
        public string WorldName { get; set; }
        public long HashedSeed { get; set; }
        public byte MaxPlayers { get; set; }
        public int ViewDistance { get; set; }
        public bool ReducedDebugInfo { get; set; }
        public bool EnableRespawnScreen { get; set; }
        public bool IsDebug { get; set; }
        public bool IsFlat { get; set; }
    }

    public struct V751_754Fields
    {
        public int EntityId { get; set; }
        public bool IsHardcore { get; set; }
        public byte GameMode { get; set; }
        public byte PreviousGameMode { get; set; }
        public string[] WorldNames { get; set; }
        public NbtTag? DimensionCodec { get; set; }
        public NbtTag? Dimension { get; set; }
        public string WorldName { get; set; }
        public long HashedSeed { get; set; }
        public int MaxPlayers { get; set; }
        public int ViewDistance { get; set; }
        public bool ReducedDebugInfo { get; set; }
        public bool EnableRespawnScreen { get; set; }
        public bool IsDebug { get; set; }
        public bool IsFlat { get; set; }
    }

    public struct V755_756Fields
    {
        public int EntityId { get; set; }
        public bool IsHardcore { get; set; }
        public byte GameMode { get; set; }
        public sbyte PreviousGameMode { get; set; }
        public string[] WorldNames { get; set; }
        public NbtTag? DimensionCodec { get; set; }
        public NbtTag? Dimension { get; set; }
        public string WorldName { get; set; }
        public long HashedSeed { get; set; }
        public int MaxPlayers { get; set; }
        public int ViewDistance { get; set; }
        public bool ReducedDebugInfo { get; set; }
        public bool EnableRespawnScreen { get; set; }
        public bool IsDebug { get; set; }
        public bool IsFlat { get; set; }
    }

    public struct V757_758Fields
    {
        public int EntityId { get; set; }
        public bool IsHardcore { get; set; }
        public byte GameMode { get; set; }
        public sbyte PreviousGameMode { get; set; }
        public string[] WorldNames { get; set; }
        public NbtTag? DimensionCodec { get; set; }
        public NbtTag? Dimension { get; set; }
        public string WorldName { get; set; }
        public long HashedSeed { get; set; }
        public int MaxPlayers { get; set; }
        public int ViewDistance { get; set; }
        public int SimulationDistance { get; set; }
        public bool ReducedDebugInfo { get; set; }
        public bool EnableRespawnScreen { get; set; }
        public bool IsDebug { get; set; }
        public bool IsFlat { get; set; }
    }

    public struct V759_762Fields
    {
        public int EntityId { get; set; }
        public bool IsHardcore { get; set; }
        public byte GameMode { get; set; }
        public sbyte PreviousGameMode { get; set; }
        public string[] WorldNames { get; set; }
        public NbtTag? DimensionCodec { get; set; }
        public string WorldType { get; set; }
        public string WorldName { get; set; }
        public long HashedSeed { get; set; }
        public int MaxPlayers { get; set; }
        public int ViewDistance { get; set; }
        public int SimulationDistance { get; set; }
        public bool ReducedDebugInfo { get; set; }
        public bool EnableRespawnScreen { get; set; }
        public bool IsDebug { get; set; }
        public bool IsFlat { get; set; }
        public DeathLocation? Death { get; set; }
    }

    public struct V763Fields
    {
        public int EntityId { get; set; }
        public bool IsHardcore { get; set; }
        public byte GameMode { get; set; }
        public sbyte PreviousGameMode { get; set; }
        public string[] WorldNames { get; set; }
        public NbtTag? DimensionCodec { get; set; }
        public string WorldType { get; set; }
        public string WorldName { get; set; }
        public long HashedSeed { get; set; }
        public int MaxPlayers { get; set; }
        public int ViewDistance { get; set; }
        public int SimulationDistance { get; set; }
        public bool ReducedDebugInfo { get; set; }
        public bool EnableRespawnScreen { get; set; }
        public bool IsDebug { get; set; }
        public bool IsFlat { get; set; }
        public DeathLocation? Death { get; set; }
        public int PortalCooldown { get; set; }
    }

    public struct V764_765Fields
    {
        public int EntityId { get; set; }
        public bool IsHardcore { get; set; }
        public string[] WorldNames { get; set; }
        public int MaxPlayers { get; set; }
        public int ViewDistance { get; set; }
        public int SimulationDistance { get; set; }
        public bool ReducedDebugInfo { get; set; }
        public bool EnableRespawnScreen { get; set; }
        public bool DoLimitedCrafting { get; set; }
        public string WorldType { get; set; }
        public string WorldName { get; set; }
        public long HashedSeed { get; set; }
        public byte GameMode { get; set; }
        public sbyte PreviousGameMode { get; set; }
        public bool IsDebug { get; set; }
        public bool IsFlat { get; set; }
        public DeathLocation? Death { get; set; }
        public int PortalCooldown { get; set; }
    }

    public struct V766_LastFields
    {
        public int EntityId { get; set; }
        public bool IsHardcore { get; set; }
        public string[] WorldNames { get; set; }
        public int MaxPlayers { get; set; }
        public int ViewDistance { get; set; }
        public int SimulationDistance { get; set; }
        public bool ReducedDebugInfo { get; set; }
        public bool EnableRespawnScreen { get; set; }
        public bool DoLimitedCrafting { get; set; }
        public SpawnInfo WorldState { get; set; }
        public bool EnforcesSecureChat { get; set; }
    }
}
