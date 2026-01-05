using System;
using McProtoNet.NBT;
using McProtoNet.Protocol.Extensions;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("Respawn", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class RespawnPacket : IServerPacket
{
    public string Dimension { get; set; } = string.Empty;
    public NbtTag? DimensionTag { get; set; }
    public string WorldName { get; set; } = string.Empty;
    public long HashedSeed { get; set; }
    public sbyte GameMode { get; set; }
    public byte PreviousGameMode { get; set; }
    public bool IsDebug { get; set; }
    public bool IsFlat { get; set; }
    public bool CopyMetadata { get; set; }

    public V759Fields? V759 { get; set; }
    public V760_762Fields? V760_762 { get; set; }
    public V763Fields? V763 { get; set; }
    public V764_765Fields? V764_765 { get; set; }
    public V766_LastFields? V766_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 736:
                writer.WriteString(Dimension);
                writer.WriteString(WorldName);
                writer.WriteSignedLong(HashedSeed);
                writer.WriteUnsignedByte((byte)GameMode);
                writer.WriteUnsignedByte(PreviousGameMode);
                writer.WriteBoolean(IsDebug);
                writer.WriteBoolean(IsFlat);
                writer.WriteBoolean(CopyMetadata);
                return;
            case >= 751 and <= 758:
                writer.WriteNbtTag(DimensionTag ?? throw new InvalidOperationException("Respawn.dimension missing."),
                    protocolVersion);
                writer.WriteString(WorldName);
                writer.WriteSignedLong(HashedSeed);
                writer.WriteUnsignedByte((byte)GameMode);
                writer.WriteUnsignedByte(PreviousGameMode);
                writer.WriteBoolean(IsDebug);
                writer.WriteBoolean(IsFlat);
                writer.WriteBoolean(CopyMetadata);
                return;
            case 759:
            {
                var fields = V759 ?? throw new InvalidOperationException("Respawn V759 missing.");
                writer.WriteString(Dimension);
                writer.WriteString(WorldName);
                writer.WriteSignedLong(HashedSeed);
                writer.WriteUnsignedByte((byte)GameMode);
                writer.WriteUnsignedByte(PreviousGameMode);
                writer.WriteBoolean(IsDebug);
                writer.WriteBoolean(IsFlat);
                writer.WriteBoolean(CopyMetadata);
                if (fields.Death is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteString(fields.Death.Value.DimensionName);
                    writer.WritePosition(fields.Death.Value.Location, protocolVersion);
                }
                return;
            }
            case >= 760 and <= 762:
            {
                var fields = V760_762 ?? throw new InvalidOperationException("Respawn V760_762 missing.");
                writer.WriteString(Dimension);
                writer.WriteString(WorldName);
                writer.WriteSignedLong(HashedSeed);
                writer.WriteSignedByte(GameMode);
                writer.WriteUnsignedByte(PreviousGameMode);
                writer.WriteBoolean(IsDebug);
                writer.WriteBoolean(IsFlat);
                writer.WriteBoolean(CopyMetadata);
                if (fields.Death is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteString(fields.Death.Value.DimensionName);
                    writer.WritePosition(fields.Death.Value.Location, protocolVersion);
                }
                return;
            }
            case 763:
            {
                var fields = V763 ?? throw new InvalidOperationException("Respawn V763 missing.");
                writer.WriteString(Dimension);
                writer.WriteString(WorldName);
                writer.WriteSignedLong(HashedSeed);
                writer.WriteSignedByte(GameMode);
                writer.WriteUnsignedByte(PreviousGameMode);
                writer.WriteBoolean(IsDebug);
                writer.WriteBoolean(IsFlat);
                writer.WriteBoolean(CopyMetadata);
                if (fields.Death is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteString(fields.Death.Value.DimensionName);
                    writer.WritePosition(fields.Death.Value.Location, protocolVersion);
                }
                writer.WriteVarInt(fields.PortalCooldown);
                return;
            }
            case >= 764 and <= 765:
            {
                var fields = V764_765 ?? throw new InvalidOperationException("Respawn V764_765 missing.");
                writer.WriteString(Dimension);
                writer.WriteString(WorldName);
                writer.WriteSignedLong(HashedSeed);
                writer.WriteSignedByte(GameMode);
                writer.WriteUnsignedByte(PreviousGameMode);
                writer.WriteBoolean(IsDebug);
                writer.WriteBoolean(IsFlat);
                if (fields.Death is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteString(fields.Death.Value.DimensionName);
                    writer.WritePosition(fields.Death.Value.Location, protocolVersion);
                }
                writer.WriteVarInt(fields.PortalCooldown);
                writer.WriteBoolean(CopyMetadata);
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V766_Last ?? throw new InvalidOperationException("Respawn V766_Last missing.");
                writer.WriteSpawnInfo(fields.WorldState, protocolVersion);
                writer.WriteUnsignedByte(fields.CopyMetadataFlag);
                return;
            }
            default:
                throw new ProtocolNotSupportException(nameof(ServerPlayPacket.Respawn), protocolVersion);
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 736:
                Dimension = reader.ReadString();
                WorldName = reader.ReadString();
                HashedSeed = reader.ReadSignedLong();
                GameMode = (sbyte)reader.ReadUnsignedByte();
                PreviousGameMode = reader.ReadUnsignedByte();
                IsDebug = reader.ReadBoolean();
                IsFlat = reader.ReadBoolean();
                CopyMetadata = reader.ReadBoolean();
                return;
            case >= 751 and <= 758:
                DimensionTag = reader.ReadNbtTag(protocolVersion)
                    ?? throw new InvalidOperationException("Respawn.dimension missing.");
                WorldName = reader.ReadString();
                HashedSeed = reader.ReadSignedLong();
                GameMode = (sbyte)reader.ReadUnsignedByte();
                PreviousGameMode = reader.ReadUnsignedByte();
                IsDebug = reader.ReadBoolean();
                IsFlat = reader.ReadBoolean();
                CopyMetadata = reader.ReadBoolean();
                return;
            case 759:
            {
                Dimension = reader.ReadString();
                WorldName = reader.ReadString();
                HashedSeed = reader.ReadSignedLong();
                GameMode = (sbyte)reader.ReadUnsignedByte();
                PreviousGameMode = reader.ReadUnsignedByte();
                IsDebug = reader.ReadBoolean();
                IsFlat = reader.ReadBoolean();
                CopyMetadata = reader.ReadBoolean();
                bool hasDeath = reader.ReadBoolean();
                V759 = new V759Fields
                {
                    Death = hasDeath
                        ? new DeathFields
                        {
                            DimensionName = reader.ReadString(),
                            Location = reader.ReadPosition(protocolVersion)
                        }
                        : null
                };
                return;
            }
            case >= 760 and <= 762:
            {
                Dimension = reader.ReadString();
                WorldName = reader.ReadString();
                HashedSeed = reader.ReadSignedLong();
                GameMode = reader.ReadSignedByte();
                PreviousGameMode = reader.ReadUnsignedByte();
                IsDebug = reader.ReadBoolean();
                IsFlat = reader.ReadBoolean();
                CopyMetadata = reader.ReadBoolean();
                bool hasDeath = reader.ReadBoolean();
                V760_762 = new V760_762Fields
                {
                    Death = hasDeath
                        ? new DeathFields
                        {
                            DimensionName = reader.ReadString(),
                            Location = reader.ReadPosition(protocolVersion)
                        }
                        : null
                };
                return;
            }
            case 763:
            {
                Dimension = reader.ReadString();
                WorldName = reader.ReadString();
                HashedSeed = reader.ReadSignedLong();
                GameMode = reader.ReadSignedByte();
                PreviousGameMode = reader.ReadUnsignedByte();
                IsDebug = reader.ReadBoolean();
                IsFlat = reader.ReadBoolean();
                CopyMetadata = reader.ReadBoolean();
                bool hasDeath = reader.ReadBoolean();
                V763 = new V763Fields
                {
                    Death = hasDeath
                        ? new DeathFields
                        {
                            DimensionName = reader.ReadString(),
                            Location = reader.ReadPosition(protocolVersion)
                        }
                        : null,
                    PortalCooldown = reader.ReadVarInt()
                };
                return;
            }
            case >= 764 and <= 765:
            {
                Dimension = reader.ReadString();
                WorldName = reader.ReadString();
                HashedSeed = reader.ReadSignedLong();
                GameMode = reader.ReadSignedByte();
                PreviousGameMode = reader.ReadUnsignedByte();
                IsDebug = reader.ReadBoolean();
                IsFlat = reader.ReadBoolean();
                bool hasDeath = reader.ReadBoolean();
                V764_765 = new V764_765Fields
                {
                    Death = hasDeath
                        ? new DeathFields
                        {
                            DimensionName = reader.ReadString(),
                            Location = reader.ReadPosition(protocolVersion)
                        }
                        : null,
                    PortalCooldown = reader.ReadVarInt()
                };
                CopyMetadata = reader.ReadBoolean();
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                V766_Last = new V766_LastFields
                {
                    WorldState = reader.ReadSpawnInfo(protocolVersion),
                    CopyMetadataFlag = reader.ReadUnsignedByte()
                };
                return;
            default:
                throw new ProtocolNotSupportException(nameof(ServerPlayPacket.Respawn), protocolVersion);
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V759Fields
    {
        public DeathFields? Death { get; set; }
    }

    public struct V760_762Fields
    {
        public DeathFields? Death { get; set; }
    }

    public struct V763Fields
    {
        public DeathFields? Death { get; set; }
        public int PortalCooldown { get; set; }
    }

    public struct V764_765Fields
    {
        public DeathFields? Death { get; set; }
        public int PortalCooldown { get; set; }
    }

    public struct V766_LastFields
    {
        public SpawnInfo WorldState { get; set; }
        public byte CopyMetadataFlag { get; set; }
    }

    public struct DeathFields
    {
        public string DimensionName { get; set; }
        public Position Location { get; set; }
    }
}
