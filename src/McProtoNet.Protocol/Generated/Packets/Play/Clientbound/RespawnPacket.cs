using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.respawn", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Dimension", "string", Group = "VUntil736", To = 736)]
[PacketField("WorldName", "string", Group = "VUntil736", To = 736)]
[PacketField("HashedSeed", "long", Group = "VUntil736", To = 736)]
[PacketField("Gamemode", "int", Group = "VUntil736", To = 736)]
[PacketField("PreviousGamemode", "int", Group = "VUntil736", To = 736)]
[PacketField("IsDebug", "bool", Group = "VUntil736", To = 736)]
[PacketField("IsFlat", "bool", Group = "VUntil736", To = 736)]
[PacketField("CopyMetadata", "bool", Group = "VUntil736", To = 736)]
[PacketField("DimensionNbt", "NbtTag", Group = "V751_758", From = 751, To = 758)]
[PacketField("WorldName", "string", Group = "V751_758", From = 751, To = 758)]
[PacketField("HashedSeed", "long", Group = "V751_758", From = 751, To = 758)]
[PacketField("Gamemode", "int", Group = "V751_758", From = 751, To = 758)]
[PacketField("PreviousGamemode", "int", Group = "V751_758", From = 751, To = 758)]
[PacketField("IsDebug", "bool", Group = "V751_758", From = 751, To = 758)]
[PacketField("IsFlat", "bool", Group = "V751_758", From = 751, To = 758)]
[PacketField("CopyMetadata", "bool", Group = "V751_758", From = 751, To = 758)]
[PacketField("DimensionName", "string", Group = "V759", From = 759, To = 759)]
[PacketField("WorldName", "string", Group = "V759", From = 759, To = 759)]
[PacketField("HashedSeed", "long", Group = "V759", From = 759, To = 759)]
[PacketField("Gamemode", "int", Group = "V759", From = 759, To = 759)]
[PacketField("PreviousGamemode", "int", Group = "V759", From = 759, To = 759)]
[PacketField("IsDebug", "bool", Group = "V759", From = 759, To = 759)]
[PacketField("IsFlat", "bool", Group = "V759", From = 759, To = 759)]
[PacketField("Death", "DeathLocation?", Group = "V759", From = 759, To = 759)]
[PacketField("CopyMetadata", "bool", Group = "V759", From = 759, To = 759)]
[PacketField("DimensionName", "string", Group = "V760_762", From = 760, To = 762)]
[PacketField("WorldName", "string", Group = "V760_762", From = 760, To = 762)]
[PacketField("HashedSeed", "long", Group = "V760_762", From = 760, To = 762)]
[PacketField("Gamemode", "int", Group = "V760_762", From = 760, To = 762)]
[PacketField("PreviousGamemode", "int", Group = "V760_762", From = 760, To = 762)]
[PacketField("IsDebug", "bool", Group = "V760_762", From = 760, To = 762)]
[PacketField("IsFlat", "bool", Group = "V760_762", From = 760, To = 762)]
[PacketField("Death", "DeathLocation?", Group = "V760_762", From = 760, To = 762)]
[PacketField("CopyMetadata", "bool", Group = "V760_762", From = 760, To = 762)]
[PacketField("DimensionName", "string", Group = "V763", From = 763, To = 763)]
[PacketField("WorldName", "string", Group = "V763", From = 763, To = 763)]
[PacketField("HashedSeed", "long", Group = "V763", From = 763, To = 763)]
[PacketField("Gamemode", "int", Group = "V763", From = 763, To = 763)]
[PacketField("PreviousGamemode", "int", Group = "V763", From = 763, To = 763)]
[PacketField("IsDebug", "bool", Group = "V763", From = 763, To = 763)]
[PacketField("IsFlat", "bool", Group = "V763", From = 763, To = 763)]
[PacketField("Death", "DeathLocation?", Group = "V763", From = 763, To = 763)]
[PacketField("PortalCooldown", "int", Group = "V763", From = 763, To = 763)]
[PacketField("CopyMetadata", "bool", Group = "V763", From = 763, To = 763)]
[PacketField("DimensionName", "string", Group = "V764_765", From = 764, To = 765)]
[PacketField("WorldName", "string", Group = "V764_765", From = 764, To = 765)]
[PacketField("HashedSeed", "long", Group = "V764_765", From = 764, To = 765)]
[PacketField("Gamemode", "int", Group = "V764_765", From = 764, To = 765)]
[PacketField("PreviousGamemode", "int", Group = "V764_765", From = 764, To = 765)]
[PacketField("IsDebug", "bool", Group = "V764_765", From = 764, To = 765)]
[PacketField("IsFlat", "bool", Group = "V764_765", From = 764, To = 765)]
[PacketField("Death", "DeathLocation?", Group = "V764_765", From = 764, To = 765)]
[PacketField("PortalCooldown", "int", Group = "V764_765", From = 764, To = 765)]
[PacketField("CopyMetadata", "bool", Group = "V764_765", From = 764, To = 765)]
[PacketField("CopyMetadataByte", "int", Group = "V766_Last", From = 766)]
[PacketField("WorldState", "SpawnInfo", Group = "V766_Last", From = 766)]
public sealed partial record RespawnPacket(RespawnPacket.VUntil736Layer? VUntil736 = null, RespawnPacket.V751_758Layer? V751_758 = null, RespawnPacket.V759Layer? V759 = null, RespawnPacket.V760_762Layer? V760_762 = null, RespawnPacket.V763Layer? V763 = null, RespawnPacket.V764_765Layer? V764_765 = null, RespawnPacket.V766_LastLayer? V766_Last = null) : IPacket<RespawnPacket>, IPacket
{
    public readonly record struct VUntil736Layer(string Dimension, string WorldName, long HashedSeed, int Gamemode, int PreviousGamemode, bool IsDebug, bool IsFlat, bool CopyMetadata);
    public readonly record struct V751_758Layer(NbtTag DimensionNbt, string WorldName, long HashedSeed, int Gamemode, int PreviousGamemode, bool IsDebug, bool IsFlat, bool CopyMetadata);
    public readonly record struct V759Layer(string DimensionName, string WorldName, long HashedSeed, int Gamemode, int PreviousGamemode, bool IsDebug, bool IsFlat, DeathLocation? Death, bool CopyMetadata);
    public readonly record struct V760_762Layer(string DimensionName, string WorldName, long HashedSeed, int Gamemode, int PreviousGamemode, bool IsDebug, bool IsFlat, DeathLocation? Death, bool CopyMetadata);
    public readonly record struct V763Layer(string DimensionName, string WorldName, long HashedSeed, int Gamemode, int PreviousGamemode, bool IsDebug, bool IsFlat, DeathLocation? Death, int PortalCooldown, bool CopyMetadata);
    public readonly record struct V764_765Layer(string DimensionName, string WorldName, long HashedSeed, int Gamemode, int PreviousGamemode, bool IsDebug, bool IsFlat, DeathLocation? Death, int PortalCooldown, bool CopyMetadata);
    public readonly record struct V766_LastLayer(int CopyMetadataByte, SpawnInfo WorldState);
    public static RespawnPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RespawnPacket>(protocolVersion);
        if (protocolVersion <= 736)
        {
            var dimension = reader.ReadString();
            var worldName = reader.ReadString();
            var hashedSeed = reader.ReadSignedLong();
            var gamemode = reader.ReadUnsignedByte();
            var previousGamemode = reader.ReadUnsignedByte();
            var isDebug = reader.ReadBoolean();
            var isFlat = reader.ReadBoolean();
            var copyMetadata = reader.ReadBoolean();
            return new RespawnPacket(VUntil736: new VUntil736Layer(dimension, worldName, hashedSeed, gamemode, previousGamemode, isDebug, isFlat, copyMetadata));
        }

        if (protocolVersion >= 751 && protocolVersion <= 758)
        {
            var dimensionNbt = reader.ReadNbtTag(true)!;
            var worldName = reader.ReadString();
            var hashedSeed = reader.ReadSignedLong();
            var gamemode = reader.ReadUnsignedByte();
            var previousGamemode = reader.ReadUnsignedByte();
            var isDebug = reader.ReadBoolean();
            var isFlat = reader.ReadBoolean();
            var copyMetadata = reader.ReadBoolean();
            return new RespawnPacket(V751_758: new V751_758Layer(dimensionNbt, worldName, hashedSeed, gamemode, previousGamemode, isDebug, isFlat, copyMetadata));
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            var dimensionName = reader.ReadString();
            var worldName = reader.ReadString();
            var hashedSeed = reader.ReadSignedLong();
            var gamemode = reader.ReadUnsignedByte();
            var previousGamemode = reader.ReadUnsignedByte();
            var isDebug = reader.ReadBoolean();
            var isFlat = reader.ReadBoolean();
            var copyMetadata = reader.ReadBoolean();
            DeathLocation? death = null;
            if (reader.ReadBoolean())
                death = reader.ReadType<DeathLocation>(protocolVersion);
            return new RespawnPacket(V759: new V759Layer(dimensionName, worldName, hashedSeed, gamemode, previousGamemode, isDebug, isFlat, death, copyMetadata));
        }

        if (protocolVersion >= 760 && protocolVersion <= 762)
        {
            var dimensionName = reader.ReadString();
            var worldName = reader.ReadString();
            var hashedSeed = reader.ReadSignedLong();
            var gamemode = reader.ReadSignedByte();
            var previousGamemode = reader.ReadUnsignedByte();
            var isDebug = reader.ReadBoolean();
            var isFlat = reader.ReadBoolean();
            var copyMetadata = reader.ReadBoolean();
            DeathLocation? death = null;
            if (reader.ReadBoolean())
                death = reader.ReadType<DeathLocation>(protocolVersion);
            return new RespawnPacket(V760_762: new V760_762Layer(dimensionName, worldName, hashedSeed, gamemode, previousGamemode, isDebug, isFlat, death, copyMetadata));
        }

        if (protocolVersion >= 763 && protocolVersion <= 763)
        {
            var dimensionName = reader.ReadString();
            var worldName = reader.ReadString();
            var hashedSeed = reader.ReadSignedLong();
            var gamemode = reader.ReadSignedByte();
            var previousGamemode = reader.ReadUnsignedByte();
            var isDebug = reader.ReadBoolean();
            var isFlat = reader.ReadBoolean();
            var copyMetadata = reader.ReadBoolean();
            DeathLocation? death = null;
            if (reader.ReadBoolean())
                death = reader.ReadType<DeathLocation>(protocolVersion);
            var portalCooldown = reader.ReadVarInt();
            return new RespawnPacket(V763: new V763Layer(dimensionName, worldName, hashedSeed, gamemode, previousGamemode, isDebug, isFlat, death, portalCooldown, copyMetadata));
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            var dimensionName = reader.ReadString();
            var worldName = reader.ReadString();
            var hashedSeed = reader.ReadSignedLong();
            var gamemode = reader.ReadSignedByte();
            var previousGamemode = reader.ReadUnsignedByte();
            var isDebug = reader.ReadBoolean();
            var isFlat = reader.ReadBoolean();
            DeathLocation? death = null;
            if (reader.ReadBoolean())
                death = reader.ReadType<DeathLocation>(protocolVersion);
            var portalCooldown = reader.ReadVarInt();
            var copyMetadata = reader.ReadBoolean();
            return new RespawnPacket(V764_765: new V764_765Layer(dimensionName, worldName, hashedSeed, gamemode, previousGamemode, isDebug, isFlat, death, portalCooldown, copyMetadata));
        }

        if (protocolVersion >= 766)
        {
            var worldState = reader.ReadType<SpawnInfo>(protocolVersion);
            var copyMetadataByte = reader.ReadUnsignedByte();
            return new RespawnPacket(V766_Last: new V766_LastLayer(copyMetadataByte, worldState));
        }

        throw new System.NotSupportedException($"RespawnPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RespawnPacket>(protocolVersion);
        if (protocolVersion <= 736)
        {
            var layer = VUntil736 ?? throw new WrongLayerException("RespawnPacket", protocolVersion, "VUntil736");
            string Dimension = layer.Dimension;
            string WorldName = layer.WorldName;
            long HashedSeed = layer.HashedSeed;
            int Gamemode = layer.Gamemode;
            int PreviousGamemode = layer.PreviousGamemode;
            bool IsDebug = layer.IsDebug;
            bool IsFlat = layer.IsFlat;
            bool CopyMetadata = layer.CopyMetadata;
            writer.WriteString(Dimension);
            writer.WriteString(WorldName);
            writer.WriteSignedLong(HashedSeed);
            writer.WriteUnsignedByte((byte)Gamemode);
            writer.WriteUnsignedByte((byte)PreviousGamemode);
            writer.WriteBoolean(IsDebug);
            writer.WriteBoolean(IsFlat);
            writer.WriteBoolean(CopyMetadata);
            return;
        }

        if (protocolVersion >= 751 && protocolVersion <= 758)
        {
            var layer = V751_758 ?? throw new WrongLayerException("RespawnPacket", protocolVersion, "V751_758");
            NbtTag DimensionNbt = layer.DimensionNbt;
            string WorldName = layer.WorldName;
            long HashedSeed = layer.HashedSeed;
            int Gamemode = layer.Gamemode;
            int PreviousGamemode = layer.PreviousGamemode;
            bool IsDebug = layer.IsDebug;
            bool IsFlat = layer.IsFlat;
            bool CopyMetadata = layer.CopyMetadata;
            writer.WriteNbt(DimensionNbt, true);
            writer.WriteString(WorldName);
            writer.WriteSignedLong(HashedSeed);
            writer.WriteUnsignedByte((byte)Gamemode);
            writer.WriteUnsignedByte((byte)PreviousGamemode);
            writer.WriteBoolean(IsDebug);
            writer.WriteBoolean(IsFlat);
            writer.WriteBoolean(CopyMetadata);
            return;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            var layer = V759 ?? throw new WrongLayerException("RespawnPacket", protocolVersion, "V759");
            string DimensionName = layer.DimensionName;
            string WorldName = layer.WorldName;
            long HashedSeed = layer.HashedSeed;
            int Gamemode = layer.Gamemode;
            int PreviousGamemode = layer.PreviousGamemode;
            bool IsDebug = layer.IsDebug;
            bool IsFlat = layer.IsFlat;
            DeathLocation? Death = layer.Death;
            bool CopyMetadata = layer.CopyMetadata;
            writer.WriteString(DimensionName);
            writer.WriteString(WorldName);
            writer.WriteSignedLong(HashedSeed);
            writer.WriteUnsignedByte((byte)Gamemode);
            writer.WriteUnsignedByte((byte)PreviousGamemode);
            writer.WriteBoolean(IsDebug);
            writer.WriteBoolean(IsFlat);
            writer.WriteBoolean(CopyMetadata);
            writer.WriteBoolean(Death is not null);
            if (Death is { } deathValue)
                writer.WriteType<DeathLocation>(deathValue, protocolVersion);
            return;
        }

        if (protocolVersion >= 760 && protocolVersion <= 762)
        {
            var layer = V760_762 ?? throw new WrongLayerException("RespawnPacket", protocolVersion, "V760_762");
            string DimensionName = layer.DimensionName;
            string WorldName = layer.WorldName;
            long HashedSeed = layer.HashedSeed;
            int Gamemode = layer.Gamemode;
            int PreviousGamemode = layer.PreviousGamemode;
            bool IsDebug = layer.IsDebug;
            bool IsFlat = layer.IsFlat;
            DeathLocation? Death = layer.Death;
            bool CopyMetadata = layer.CopyMetadata;
            writer.WriteString(DimensionName);
            writer.WriteString(WorldName);
            writer.WriteSignedLong(HashedSeed);
            writer.WriteSignedByte((sbyte)Gamemode);
            writer.WriteUnsignedByte((byte)PreviousGamemode);
            writer.WriteBoolean(IsDebug);
            writer.WriteBoolean(IsFlat);
            writer.WriteBoolean(CopyMetadata);
            writer.WriteBoolean(Death is not null);
            if (Death is { } deathValue)
                writer.WriteType<DeathLocation>(deathValue, protocolVersion);
            return;
        }

        if (protocolVersion >= 763 && protocolVersion <= 763)
        {
            var layer = V763 ?? throw new WrongLayerException("RespawnPacket", protocolVersion, "V763");
            string DimensionName = layer.DimensionName;
            string WorldName = layer.WorldName;
            long HashedSeed = layer.HashedSeed;
            int Gamemode = layer.Gamemode;
            int PreviousGamemode = layer.PreviousGamemode;
            bool IsDebug = layer.IsDebug;
            bool IsFlat = layer.IsFlat;
            DeathLocation? Death = layer.Death;
            int PortalCooldown = layer.PortalCooldown;
            bool CopyMetadata = layer.CopyMetadata;
            writer.WriteString(DimensionName);
            writer.WriteString(WorldName);
            writer.WriteSignedLong(HashedSeed);
            writer.WriteSignedByte((sbyte)Gamemode);
            writer.WriteUnsignedByte((byte)PreviousGamemode);
            writer.WriteBoolean(IsDebug);
            writer.WriteBoolean(IsFlat);
            writer.WriteBoolean(CopyMetadata);
            writer.WriteBoolean(Death is not null);
            if (Death is { } deathValue)
                writer.WriteType<DeathLocation>(deathValue, protocolVersion);
            writer.WriteVarInt(PortalCooldown);
            return;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            var layer = V764_765 ?? throw new WrongLayerException("RespawnPacket", protocolVersion, "V764_765");
            string DimensionName = layer.DimensionName;
            string WorldName = layer.WorldName;
            long HashedSeed = layer.HashedSeed;
            int Gamemode = layer.Gamemode;
            int PreviousGamemode = layer.PreviousGamemode;
            bool IsDebug = layer.IsDebug;
            bool IsFlat = layer.IsFlat;
            DeathLocation? Death = layer.Death;
            int PortalCooldown = layer.PortalCooldown;
            bool CopyMetadata = layer.CopyMetadata;
            writer.WriteString(DimensionName);
            writer.WriteString(WorldName);
            writer.WriteSignedLong(HashedSeed);
            writer.WriteSignedByte((sbyte)Gamemode);
            writer.WriteUnsignedByte((byte)PreviousGamemode);
            writer.WriteBoolean(IsDebug);
            writer.WriteBoolean(IsFlat);
            writer.WriteBoolean(Death is not null);
            if (Death is { } deathValue)
                writer.WriteType<DeathLocation>(deathValue, protocolVersion);
            writer.WriteVarInt(PortalCooldown);
            writer.WriteBoolean(CopyMetadata);
            return;
        }

        if (protocolVersion >= 766)
        {
            var layer = V766_Last ?? throw new WrongLayerException("RespawnPacket", protocolVersion, "V766_Last");
            int CopyMetadataByte = layer.CopyMetadataByte;
            SpawnInfo WorldState = layer.WorldState;
            writer.WriteType<SpawnInfo>(WorldState, protocolVersion);
            writer.WriteUnsignedByte((byte)CopyMetadataByte);
            return;
        }

        throw new System.NotSupportedException($"RespawnPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.respawn", "Respawn", PacketPhase.Play, PacketDirection.Clientbound, 76);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x3A;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x39;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x3D;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x3B;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x3E;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x3D;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x41;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x43;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x45;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x47;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x4C;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x4B;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x50;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x52;
            return true;
        }

        id = 0;
        return false;
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (TryGetPacketId(protocolVersion, out var id))
            return id;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
