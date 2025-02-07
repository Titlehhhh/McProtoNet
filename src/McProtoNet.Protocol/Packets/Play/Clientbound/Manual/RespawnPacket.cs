using McProtoNet.Protocol;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("Respawn", PacketState.Play, PacketDirection.Clientbound)]
public abstract partial class RespawnPacket : IServerPacket
{
    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);

    [PacketSubInfo(340, 404)]
    public sealed partial class V340_404 : RespawnPacket
    {
        public int Dimension { get; set; }
        public byte Difficulty { get; set; }
        public byte GameMode { get; set; }
        public string LevelType { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Dimension = reader.ReadSignedInt();
            Difficulty = reader.ReadUnsignedByte();
            GameMode = reader.ReadUnsignedByte();
            LevelType = reader.ReadString();
        }
    }

    [PacketSubInfo(477, 498)]
    public sealed partial class V477_498 : RespawnPacket
    {
        public int Dimension { get; set; }
        public byte GameMode { get; set; }
        public string LevelType { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Dimension = reader.ReadSignedInt();
            GameMode = reader.ReadUnsignedByte();
            LevelType = reader.ReadString();
        }
    }

    [PacketSubInfo(573, 710)]
    public sealed partial class V573_710 : RespawnPacket
    {
        public int Dimension { get; set; }
        public long HashedSeed { get; set; }
        public byte GameMode { get; set; }
        public string LevelType { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Dimension = reader.ReadSignedInt();
            HashedSeed = reader.ReadSignedLong();
            GameMode = reader.ReadUnsignedByte();
            LevelType = reader.ReadString();
        }
    }

    [PacketSubInfo(734, 736)]
    public sealed partial class V734_736 : RespawnPacket
    {
        public string Dimension { get; set; }
        public string WorldName { get; set; }
        public long HashedSeed { get; set; }
        public byte GameMode { get; set; }
        public byte PreviousGameMode { get; set; }
        public bool IsDebug { get; set; }
        public bool IsFlat { get; set; }
        public bool CopyMetadata { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Dimension = reader.ReadString();
            WorldName = reader.ReadString();
            HashedSeed = reader.ReadSignedLong();
            GameMode = reader.ReadUnsignedByte();
            PreviousGameMode = reader.ReadUnsignedByte();
            IsDebug = reader.ReadBoolean();
            IsFlat = reader.ReadBoolean();
            CopyMetadata = reader.ReadBoolean();
        }
    }

    [PacketSubInfo(751, 758)]
    public sealed partial class V751_758 : RespawnPacket
    {
        public NbtTag? Dimension { get; set; }
        public string WorldName { get; set; }
        public long HashedSeed { get; set; }
        public byte GameMode { get; set; }
        public byte PreviousGameMode { get; set; }
        public bool IsDebug { get; set; }
        public bool IsFlat { get; set; }
        public bool CopyMetadata { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Dimension = reader.ReadNbtTag(protocolVersion);
            WorldName = reader.ReadString();
            HashedSeed = reader.ReadSignedLong();
            GameMode = reader.ReadUnsignedByte();
            PreviousGameMode = reader.ReadUnsignedByte();
            IsDebug = reader.ReadBoolean();
            IsFlat = reader.ReadBoolean();
            CopyMetadata = reader.ReadBoolean();
        }
    }

    [PacketSubInfo(759, 759)]
    public sealed partial class V759 : RespawnPacket
    {
        public string Dimension { get; set; }
        public string WorldName { get; set; }
        public long HashedSeed { get; set; }
        public byte GameMode { get; set; }
        public byte PreviousGameMode { get; set; }
        public bool IsDebug { get; set; }
        public bool IsFlat { get; set; }
        public bool CopyMetadata { get; set; }
        public DeathLocation? Death { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Dimension = reader.ReadString();
            WorldName = reader.ReadString();
            HashedSeed = reader.ReadSignedLong();
            GameMode = reader.ReadUnsignedByte();
            PreviousGameMode = reader.ReadUnsignedByte();
            IsDebug = reader.ReadBoolean();
            IsFlat = reader.ReadBoolean();
            CopyMetadata = reader.ReadBoolean();
            if (reader.ReadBoolean())
            {
                Death = reader.ReadDeathLocation(protocolVersion);
            }
        }
    }

    [PacketSubInfo(760, 762)]
    public sealed partial class V760_762 : RespawnPacket
    {
        public string Dimension { get; set; }
        public string WorldName { get; set; }
        public long HashedSeed { get; set; }
        public sbyte GameMode { get; set; }
        public byte PreviousGameMode { get; set; }
        public bool IsDebug { get; set; }
        public bool IsFlat { get; set; }
        public bool CopyMetadata { get; set; }
        public DeathLocation? Death { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Dimension = reader.ReadString();
            WorldName = reader.ReadString();
            HashedSeed = reader.ReadSignedLong();
            GameMode = reader.ReadSignedByte();
            PreviousGameMode = reader.ReadUnsignedByte();
            IsDebug = reader.ReadBoolean();
            IsFlat = reader.ReadBoolean();
            CopyMetadata = reader.ReadBoolean();
            if (reader.ReadBoolean())
            {
                Death = reader.ReadDeathLocation(protocolVersion);
            }
        }
    }

    [PacketSubInfo(763, 763)]
    public sealed partial class V763 : RespawnPacket
    {
        public string Dimension { get; set; }
        public string WorldName { get; set; }
        public long HashedSeed { get; set; }
        public sbyte GameMode { get; set; }
        public byte PreviousGameMode { get; set; }
        public bool IsDebug { get; set; }
        public bool IsFlat { get; set; }
        public bool CopyMetadata { get; set; }
        public DeathLocation? Death { get; set; }
        public int PortalCooldown { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Dimension = reader.ReadString();
            WorldName = reader.ReadString();
            HashedSeed = reader.ReadSignedLong();
            GameMode = reader.ReadSignedByte();
            PreviousGameMode = reader.ReadUnsignedByte();
            IsDebug = reader.ReadBoolean();
            IsFlat = reader.ReadBoolean();
            CopyMetadata = reader.ReadBoolean();
            if (reader.ReadBoolean())
            {
                Death = reader.ReadDeathLocation(protocolVersion);
            }

            PortalCooldown = reader.ReadVarInt();
        }
    }

    [PacketSubInfo(764, 765)]
    public sealed partial class V764_765 : RespawnPacket
    {
        public string Dimension { get; set; }
        public string WorldName { get; set; }
        public long HashedSeed { get; set; }
        public sbyte GameMode { get; set; }
        public byte PreviousGameMode { get; set; }
        public bool IsDebug { get; set; }
        public bool IsFlat { get; set; }
        public DeathLocation? Death { get; set; }
        public int PortalCooldown { get; set; }
        public bool CopyMetadata { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Dimension = reader.ReadString();
            WorldName = reader.ReadString();
            HashedSeed = reader.ReadSignedLong();
            GameMode = reader.ReadSignedByte();
            PreviousGameMode = reader.ReadUnsignedByte();
            IsDebug = reader.ReadBoolean();
            IsFlat = reader.ReadBoolean();
            if (reader.ReadBoolean())
            {
                Death = reader.ReadDeathLocation(protocolVersion);
            }

            PortalCooldown = reader.ReadVarInt();
            CopyMetadata = reader.ReadBoolean();
        }
    }

    [PacketSubInfo(766, 766)]
    public sealed partial class V766 : RespawnPacket
    {
        public SpawnInfo WorldState { get; set; }
        public bool CopyMetadata { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            WorldState = reader.ReadSpawnInfo(protocolVersion);
            CopyMetadata = reader.ReadBoolean();
        }
    }

    [PacketSubInfo(767, 769)]
    public sealed partial class V767_769 : RespawnPacket
    {
        public SpawnInfo WorldState { get; set; }
        public byte CopyMetadata { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            WorldState = reader.ReadSpawnInfo(protocolVersion);
            CopyMetadata = reader.ReadUnsignedByte();
        }
    }
}