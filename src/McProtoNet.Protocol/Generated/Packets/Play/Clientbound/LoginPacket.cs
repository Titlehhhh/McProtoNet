using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.login", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("WorldNames", "string[]")]
[PacketField("MaxPlayers", "int")]
[PacketField("ViewDistance", "int")]
[PacketField("ReducedDebugInfo", "bool")]
[PacketField("EnableRespawnScreen", "bool")]
[PacketField("Gamemode", "int", Group = "VUntil736", To = 736)]
[PacketField("PreviousGamemode", "int", Group = "VUntil736", To = 736)]
[PacketField("DimensionCodec", "NbtTag", Group = "VUntil736", To = 736)]
[PacketField("Dimension", "string", Group = "VUntil736", To = 736)]
[PacketField("WorldName", "string", Group = "VUntil736", To = 736)]
[PacketField("HashedSeed", "long", Group = "VUntil736", To = 736)]
[PacketField("IsDebug", "bool", Group = "VUntil736", To = 736)]
[PacketField("IsFlat", "bool", Group = "VUntil736", To = 736)]
[PacketField("IsHardcore", "bool", Group = "V751_754", From = 751, To = 754)]
[PacketField("Gamemode", "int", Group = "V751_754", From = 751, To = 754)]
[PacketField("PreviousGamemode", "int", Group = "V751_754", From = 751, To = 754)]
[PacketField("DimensionCodec", "NbtTag", Group = "V751_754", From = 751, To = 754)]
[PacketField("DimensionNbt", "NbtTag", Group = "V751_754", From = 751, To = 754)]
[PacketField("WorldName", "string", Group = "V751_754", From = 751, To = 754)]
[PacketField("HashedSeed", "long", Group = "V751_754", From = 751, To = 754)]
[PacketField("IsDebug", "bool", Group = "V751_754", From = 751, To = 754)]
[PacketField("IsFlat", "bool", Group = "V751_754", From = 751, To = 754)]
[PacketField("IsHardcore", "bool", Group = "V755_756", From = 755, To = 756)]
[PacketField("Gamemode", "int", Group = "V755_756", From = 755, To = 756)]
[PacketField("PreviousGamemode", "int", Group = "V755_756", From = 755, To = 756)]
[PacketField("DimensionCodec", "NbtTag", Group = "V755_756", From = 755, To = 756)]
[PacketField("DimensionNbt", "NbtTag", Group = "V755_756", From = 755, To = 756)]
[PacketField("WorldName", "string", Group = "V755_756", From = 755, To = 756)]
[PacketField("HashedSeed", "long", Group = "V755_756", From = 755, To = 756)]
[PacketField("IsDebug", "bool", Group = "V755_756", From = 755, To = 756)]
[PacketField("IsFlat", "bool", Group = "V755_756", From = 755, To = 756)]
[PacketField("IsHardcore", "bool", Group = "V757_758", From = 757, To = 758)]
[PacketField("Gamemode", "int", Group = "V757_758", From = 757, To = 758)]
[PacketField("PreviousGamemode", "int", Group = "V757_758", From = 757, To = 758)]
[PacketField("DimensionCodec", "NbtTag", Group = "V757_758", From = 757, To = 758)]
[PacketField("DimensionNbt", "NbtTag", Group = "V757_758", From = 757, To = 758)]
[PacketField("WorldName", "string", Group = "V757_758", From = 757, To = 758)]
[PacketField("HashedSeed", "long", Group = "V757_758", From = 757, To = 758)]
[PacketField("SimulationDistance", "int", Group = "V757_758", From = 757, To = 758)]
[PacketField("IsDebug", "bool", Group = "V757_758", From = 757, To = 758)]
[PacketField("IsFlat", "bool", Group = "V757_758", From = 757, To = 758)]
[PacketField("IsHardcore", "bool", Group = "V759_762", From = 759, To = 762)]
[PacketField("Gamemode", "int", Group = "V759_762", From = 759, To = 762)]
[PacketField("PreviousGamemode", "int", Group = "V759_762", From = 759, To = 762)]
[PacketField("DimensionCodec", "NbtTag", Group = "V759_762", From = 759, To = 762)]
[PacketField("WorldType", "string", Group = "V759_762", From = 759, To = 762)]
[PacketField("WorldName", "string", Group = "V759_762", From = 759, To = 762)]
[PacketField("HashedSeed", "long", Group = "V759_762", From = 759, To = 762)]
[PacketField("SimulationDistance", "int", Group = "V759_762", From = 759, To = 762)]
[PacketField("IsDebug", "bool", Group = "V759_762", From = 759, To = 762)]
[PacketField("IsFlat", "bool", Group = "V759_762", From = 759, To = 762)]
[PacketField("Death", "DeathLocation?", Group = "V759_762", From = 759, To = 762)]
[PacketField("IsHardcore", "bool", Group = "V763", From = 763, To = 763)]
[PacketField("Gamemode", "int", Group = "V763", From = 763, To = 763)]
[PacketField("PreviousGamemode", "int", Group = "V763", From = 763, To = 763)]
[PacketField("DimensionCodec", "NbtTag", Group = "V763", From = 763, To = 763)]
[PacketField("WorldType", "string", Group = "V763", From = 763, To = 763)]
[PacketField("WorldName", "string", Group = "V763", From = 763, To = 763)]
[PacketField("HashedSeed", "long", Group = "V763", From = 763, To = 763)]
[PacketField("SimulationDistance", "int", Group = "V763", From = 763, To = 763)]
[PacketField("IsDebug", "bool", Group = "V763", From = 763, To = 763)]
[PacketField("IsFlat", "bool", Group = "V763", From = 763, To = 763)]
[PacketField("Death", "DeathLocation?", Group = "V763", From = 763, To = 763)]
[PacketField("PortalCooldown", "int", Group = "V763", From = 763, To = 763)]
[PacketField("IsHardcore", "bool", Group = "V764_765", From = 764, To = 765)]
[PacketField("Gamemode", "int", Group = "V764_765", From = 764, To = 765)]
[PacketField("PreviousGamemode", "int", Group = "V764_765", From = 764, To = 765)]
[PacketField("WorldType", "string", Group = "V764_765", From = 764, To = 765)]
[PacketField("WorldName", "string", Group = "V764_765", From = 764, To = 765)]
[PacketField("HashedSeed", "long", Group = "V764_765", From = 764, To = 765)]
[PacketField("SimulationDistance", "int", Group = "V764_765", From = 764, To = 765)]
[PacketField("IsDebug", "bool", Group = "V764_765", From = 764, To = 765)]
[PacketField("IsFlat", "bool", Group = "V764_765", From = 764, To = 765)]
[PacketField("Death", "DeathLocation?", Group = "V764_765", From = 764, To = 765)]
[PacketField("PortalCooldown", "int", Group = "V764_765", From = 764, To = 765)]
[PacketField("DoLimitedCrafting", "bool", Group = "V764_765", From = 764, To = 765)]
[PacketField("IsHardcore", "bool", Group = "V766_775", From = 766, To = 775)]
[PacketField("SimulationDistance", "int", Group = "V766_775", From = 766, To = 775)]
[PacketField("DoLimitedCrafting", "bool", Group = "V766_775", From = 766, To = 775)]
[PacketField("WorldState", "SpawnInfo", Group = "V766_775", From = 766, To = 775)]
[PacketField("EnforcesSecureChat", "bool", Group = "V766_775", From = 766, To = 775)]
[PacketField("IsHardcore", "bool", Group = "V776_Last", From = 776)]
[PacketField("SimulationDistance", "int", Group = "V776_Last", From = 776)]
[PacketField("DoLimitedCrafting", "bool", Group = "V776_Last", From = 776)]
[PacketField("WorldState", "SpawnInfo", Group = "V776_Last", From = 776)]
[PacketField("OnlineMode", "bool", Group = "V776_Last", From = 776)]
[PacketField("EnforcesSecureChat", "bool", Group = "V776_Last", From = 776)]
public sealed partial record LoginPacket(int EntityId, string[] WorldNames, int MaxPlayers, int ViewDistance, bool ReducedDebugInfo, bool EnableRespawnScreen, LoginPacket.VUntil736Layer? VUntil736 = null, LoginPacket.V751_754Layer? V751_754 = null, LoginPacket.V755_756Layer? V755_756 = null, LoginPacket.V757_758Layer? V757_758 = null, LoginPacket.V759_762Layer? V759_762 = null, LoginPacket.V763Layer? V763 = null, LoginPacket.V764_765Layer? V764_765 = null, LoginPacket.V766_775Layer? V766_775 = null, LoginPacket.V776_LastLayer? V776_Last = null) : IPacket<LoginPacket>, IPacket
{
    public readonly record struct VUntil736Layer(int Gamemode, int PreviousGamemode, NbtTag DimensionCodec, string Dimension, string WorldName, long HashedSeed, bool IsDebug, bool IsFlat);
    public readonly record struct V751_754Layer(bool IsHardcore, int Gamemode, int PreviousGamemode, NbtTag DimensionCodec, NbtTag DimensionNbt, string WorldName, long HashedSeed, bool IsDebug, bool IsFlat);
    public readonly record struct V755_756Layer(bool IsHardcore, int Gamemode, int PreviousGamemode, NbtTag DimensionCodec, NbtTag DimensionNbt, string WorldName, long HashedSeed, bool IsDebug, bool IsFlat);
    public readonly record struct V757_758Layer(bool IsHardcore, int Gamemode, int PreviousGamemode, NbtTag DimensionCodec, NbtTag DimensionNbt, string WorldName, long HashedSeed, int SimulationDistance, bool IsDebug, bool IsFlat);
    public readonly record struct V759_762Layer(bool IsHardcore, int Gamemode, int PreviousGamemode, NbtTag DimensionCodec, string WorldType, string WorldName, long HashedSeed, int SimulationDistance, bool IsDebug, bool IsFlat, DeathLocation? Death);
    public readonly record struct V763Layer(bool IsHardcore, int Gamemode, int PreviousGamemode, NbtTag DimensionCodec, string WorldType, string WorldName, long HashedSeed, int SimulationDistance, bool IsDebug, bool IsFlat, DeathLocation? Death, int PortalCooldown);
    public readonly record struct V764_765Layer(bool IsHardcore, int Gamemode, int PreviousGamemode, string WorldType, string WorldName, long HashedSeed, int SimulationDistance, bool IsDebug, bool IsFlat, DeathLocation? Death, int PortalCooldown, bool DoLimitedCrafting);
    public readonly record struct V766_775Layer(bool IsHardcore, int SimulationDistance, bool DoLimitedCrafting, SpawnInfo WorldState, bool EnforcesSecureChat);
    public readonly record struct V776_LastLayer(bool IsHardcore, int SimulationDistance, bool DoLimitedCrafting, SpawnInfo WorldState, bool OnlineMode, bool EnforcesSecureChat);
    public static LoginPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginPacket>(protocolVersion);
        if (protocolVersion <= 736)
        {
            var entityId = reader.ReadSignedInt();
            var gamemode = reader.ReadUnsignedByte();
            var previousGamemode = reader.ReadUnsignedByte();
            int worldNamesCount = reader.ReadVarInt();
            var worldNames = new string[worldNamesCount];
            for (int i = 0; i < worldNames.Length; i++)
                worldNames[i] = reader.ReadString();
            var dimensionCodec = reader.ReadNbtTag(true)!;
            var dimension = reader.ReadString();
            var worldName = reader.ReadString();
            var hashedSeed = reader.ReadSignedLong();
            var maxPlayers = reader.ReadUnsignedByte();
            var viewDistance = reader.ReadVarInt();
            var reducedDebugInfo = reader.ReadBoolean();
            var enableRespawnScreen = reader.ReadBoolean();
            var isDebug = reader.ReadBoolean();
            var isFlat = reader.ReadBoolean();
            return new LoginPacket(entityId, worldNames, maxPlayers, viewDistance, reducedDebugInfo, enableRespawnScreen, VUntil736: new VUntil736Layer(gamemode, previousGamemode, dimensionCodec, dimension, worldName, hashedSeed, isDebug, isFlat));
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            var entityId = reader.ReadSignedInt();
            var isHardcore = reader.ReadBoolean();
            var gamemode = reader.ReadUnsignedByte();
            var previousGamemode = reader.ReadUnsignedByte();
            int worldNamesCount = reader.ReadVarInt();
            var worldNames = new string[worldNamesCount];
            for (int i = 0; i < worldNames.Length; i++)
                worldNames[i] = reader.ReadString();
            var dimensionCodec = reader.ReadNbtTag(true)!;
            var dimensionNbt = reader.ReadNbtTag(true)!;
            var worldName = reader.ReadString();
            var hashedSeed = reader.ReadSignedLong();
            var maxPlayers = reader.ReadVarInt();
            var viewDistance = reader.ReadVarInt();
            var reducedDebugInfo = reader.ReadBoolean();
            var enableRespawnScreen = reader.ReadBoolean();
            var isDebug = reader.ReadBoolean();
            var isFlat = reader.ReadBoolean();
            return new LoginPacket(entityId, worldNames, maxPlayers, viewDistance, reducedDebugInfo, enableRespawnScreen, V751_754: new V751_754Layer(isHardcore, gamemode, previousGamemode, dimensionCodec, dimensionNbt, worldName, hashedSeed, isDebug, isFlat));
        }

        if (protocolVersion >= 755 && protocolVersion <= 756)
        {
            var entityId = reader.ReadSignedInt();
            var isHardcore = reader.ReadBoolean();
            var gamemode = reader.ReadUnsignedByte();
            var previousGamemode = reader.ReadSignedByte();
            int worldNamesCount = reader.ReadVarInt();
            var worldNames = new string[worldNamesCount];
            for (int i = 0; i < worldNames.Length; i++)
                worldNames[i] = reader.ReadString();
            var dimensionCodec = reader.ReadNbtTag(true)!;
            var dimensionNbt = reader.ReadNbtTag(true)!;
            var worldName = reader.ReadString();
            var hashedSeed = reader.ReadSignedLong();
            var maxPlayers = reader.ReadVarInt();
            var viewDistance = reader.ReadVarInt();
            var reducedDebugInfo = reader.ReadBoolean();
            var enableRespawnScreen = reader.ReadBoolean();
            var isDebug = reader.ReadBoolean();
            var isFlat = reader.ReadBoolean();
            return new LoginPacket(entityId, worldNames, maxPlayers, viewDistance, reducedDebugInfo, enableRespawnScreen, V755_756: new V755_756Layer(isHardcore, gamemode, previousGamemode, dimensionCodec, dimensionNbt, worldName, hashedSeed, isDebug, isFlat));
        }

        if (protocolVersion >= 757 && protocolVersion <= 758)
        {
            var entityId = reader.ReadSignedInt();
            var isHardcore = reader.ReadBoolean();
            var gamemode = reader.ReadUnsignedByte();
            var previousGamemode = reader.ReadSignedByte();
            int worldNamesCount = reader.ReadVarInt();
            var worldNames = new string[worldNamesCount];
            for (int i = 0; i < worldNames.Length; i++)
                worldNames[i] = reader.ReadString();
            var dimensionCodec = reader.ReadNbtTag(true)!;
            var dimensionNbt = reader.ReadNbtTag(true)!;
            var worldName = reader.ReadString();
            var hashedSeed = reader.ReadSignedLong();
            var maxPlayers = reader.ReadVarInt();
            var viewDistance = reader.ReadVarInt();
            var simulationDistance = reader.ReadVarInt();
            var reducedDebugInfo = reader.ReadBoolean();
            var enableRespawnScreen = reader.ReadBoolean();
            var isDebug = reader.ReadBoolean();
            var isFlat = reader.ReadBoolean();
            return new LoginPacket(entityId, worldNames, maxPlayers, viewDistance, reducedDebugInfo, enableRespawnScreen, V757_758: new V757_758Layer(isHardcore, gamemode, previousGamemode, dimensionCodec, dimensionNbt, worldName, hashedSeed, simulationDistance, isDebug, isFlat));
        }

        if (protocolVersion >= 759 && protocolVersion <= 762)
        {
            var entityId = reader.ReadSignedInt();
            var isHardcore = reader.ReadBoolean();
            var gamemode = reader.ReadUnsignedByte();
            var previousGamemode = reader.ReadSignedByte();
            int worldNamesCount = reader.ReadVarInt();
            var worldNames = new string[worldNamesCount];
            for (int i = 0; i < worldNames.Length; i++)
                worldNames[i] = reader.ReadString();
            var dimensionCodec = reader.ReadNbtTag(true)!;
            var worldType = reader.ReadString();
            var worldName = reader.ReadString();
            var hashedSeed = reader.ReadSignedLong();
            var maxPlayers = reader.ReadVarInt();
            var viewDistance = reader.ReadVarInt();
            var simulationDistance = reader.ReadVarInt();
            var reducedDebugInfo = reader.ReadBoolean();
            var enableRespawnScreen = reader.ReadBoolean();
            var isDebug = reader.ReadBoolean();
            var isFlat = reader.ReadBoolean();
            DeathLocation? death = null;
            if (reader.ReadBoolean())
                death = reader.ReadType<DeathLocation>(protocolVersion);
            return new LoginPacket(entityId, worldNames, maxPlayers, viewDistance, reducedDebugInfo, enableRespawnScreen, V759_762: new V759_762Layer(isHardcore, gamemode, previousGamemode, dimensionCodec, worldType, worldName, hashedSeed, simulationDistance, isDebug, isFlat, death));
        }

        if (protocolVersion >= 763 && protocolVersion <= 763)
        {
            var entityId = reader.ReadSignedInt();
            var isHardcore = reader.ReadBoolean();
            var gamemode = reader.ReadUnsignedByte();
            var previousGamemode = reader.ReadSignedByte();
            int worldNamesCount = reader.ReadVarInt();
            var worldNames = new string[worldNamesCount];
            for (int i = 0; i < worldNames.Length; i++)
                worldNames[i] = reader.ReadString();
            var dimensionCodec = reader.ReadNbtTag(true)!;
            var worldType = reader.ReadString();
            var worldName = reader.ReadString();
            var hashedSeed = reader.ReadSignedLong();
            var maxPlayers = reader.ReadVarInt();
            var viewDistance = reader.ReadVarInt();
            var simulationDistance = reader.ReadVarInt();
            var reducedDebugInfo = reader.ReadBoolean();
            var enableRespawnScreen = reader.ReadBoolean();
            var isDebug = reader.ReadBoolean();
            var isFlat = reader.ReadBoolean();
            DeathLocation? death = null;
            if (reader.ReadBoolean())
                death = reader.ReadType<DeathLocation>(protocolVersion);
            var portalCooldown = reader.ReadVarInt();
            return new LoginPacket(entityId, worldNames, maxPlayers, viewDistance, reducedDebugInfo, enableRespawnScreen, V763: new V763Layer(isHardcore, gamemode, previousGamemode, dimensionCodec, worldType, worldName, hashedSeed, simulationDistance, isDebug, isFlat, death, portalCooldown));
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            var entityId = reader.ReadSignedInt();
            var isHardcore = reader.ReadBoolean();
            int worldNamesCount = reader.ReadVarInt();
            var worldNames = new string[worldNamesCount];
            for (int i = 0; i < worldNames.Length; i++)
                worldNames[i] = reader.ReadString();
            var maxPlayers = reader.ReadVarInt();
            var viewDistance = reader.ReadVarInt();
            var simulationDistance = reader.ReadVarInt();
            var reducedDebugInfo = reader.ReadBoolean();
            var enableRespawnScreen = reader.ReadBoolean();
            var doLimitedCrafting = reader.ReadBoolean();
            var worldType = reader.ReadString();
            var worldName = reader.ReadString();
            var hashedSeed = reader.ReadSignedLong();
            var gamemode = reader.ReadUnsignedByte();
            var previousGamemode = reader.ReadSignedByte();
            var isDebug = reader.ReadBoolean();
            var isFlat = reader.ReadBoolean();
            DeathLocation? death = null;
            if (reader.ReadBoolean())
                death = reader.ReadType<DeathLocation>(protocolVersion);
            var portalCooldown = reader.ReadVarInt();
            return new LoginPacket(entityId, worldNames, maxPlayers, viewDistance, reducedDebugInfo, enableRespawnScreen, V764_765: new V764_765Layer(isHardcore, gamemode, previousGamemode, worldType, worldName, hashedSeed, simulationDistance, isDebug, isFlat, death, portalCooldown, doLimitedCrafting));
        }

        if (protocolVersion >= 766 && protocolVersion <= 775)
        {
            var entityId = reader.ReadSignedInt();
            var isHardcore = reader.ReadBoolean();
            int worldNamesCount = reader.ReadVarInt();
            var worldNames = new string[worldNamesCount];
            for (int i = 0; i < worldNames.Length; i++)
                worldNames[i] = reader.ReadString();
            var maxPlayers = reader.ReadVarInt();
            var viewDistance = reader.ReadVarInt();
            var simulationDistance = reader.ReadVarInt();
            var reducedDebugInfo = reader.ReadBoolean();
            var enableRespawnScreen = reader.ReadBoolean();
            var doLimitedCrafting = reader.ReadBoolean();
            var worldState = reader.ReadType<SpawnInfo>(protocolVersion);
            var enforcesSecureChat = reader.ReadBoolean();
            return new LoginPacket(entityId, worldNames, maxPlayers, viewDistance, reducedDebugInfo, enableRespawnScreen, V766_775: new V766_775Layer(isHardcore, simulationDistance, doLimitedCrafting, worldState, enforcesSecureChat));
        }

        if (protocolVersion >= 776)
        {
            var entityId = reader.ReadSignedInt();
            var isHardcore = reader.ReadBoolean();
            int worldNamesCount = reader.ReadVarInt();
            var worldNames = new string[worldNamesCount];
            for (int i = 0; i < worldNames.Length; i++)
                worldNames[i] = reader.ReadString();
            var maxPlayers = reader.ReadVarInt();
            var viewDistance = reader.ReadVarInt();
            var simulationDistance = reader.ReadVarInt();
            var reducedDebugInfo = reader.ReadBoolean();
            var enableRespawnScreen = reader.ReadBoolean();
            var doLimitedCrafting = reader.ReadBoolean();
            var worldState = reader.ReadType<SpawnInfo>(protocolVersion);
            var onlineMode = reader.ReadBoolean();
            var enforcesSecureChat = reader.ReadBoolean();
            return new LoginPacket(entityId, worldNames, maxPlayers, viewDistance, reducedDebugInfo, enableRespawnScreen, V776_Last: new V776_LastLayer(isHardcore, simulationDistance, doLimitedCrafting, worldState, onlineMode, enforcesSecureChat));
        }

        throw new System.NotSupportedException($"LoginPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginPacket>(protocolVersion);
        if (protocolVersion <= 736)
        {
            var layer = VUntil736 ?? throw new WrongLayerException("LoginPacket", protocolVersion, "VUntil736");
            int Gamemode = layer.Gamemode;
            int PreviousGamemode = layer.PreviousGamemode;
            NbtTag DimensionCodec = layer.DimensionCodec;
            string Dimension = layer.Dimension;
            string WorldName = layer.WorldName;
            long HashedSeed = layer.HashedSeed;
            bool IsDebug = layer.IsDebug;
            bool IsFlat = layer.IsFlat;
            writer.WriteSignedInt(EntityId);
            writer.WriteUnsignedByte((byte)Gamemode);
            writer.WriteUnsignedByte((byte)PreviousGamemode);
            writer.WriteVarInt(WorldNames.Length);
            foreach (var worldNamesItem in WorldNames)
                writer.WriteString(worldNamesItem);
            writer.WriteNbt(DimensionCodec, true);
            writer.WriteString(Dimension);
            writer.WriteString(WorldName);
            writer.WriteSignedLong(HashedSeed);
            writer.WriteUnsignedByte((byte)MaxPlayers);
            writer.WriteVarInt(ViewDistance);
            writer.WriteBoolean(ReducedDebugInfo);
            writer.WriteBoolean(EnableRespawnScreen);
            writer.WriteBoolean(IsDebug);
            writer.WriteBoolean(IsFlat);
            return;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            var layer = V751_754 ?? throw new WrongLayerException("LoginPacket", protocolVersion, "V751_754");
            bool IsHardcore = layer.IsHardcore;
            int Gamemode = layer.Gamemode;
            int PreviousGamemode = layer.PreviousGamemode;
            NbtTag DimensionCodec = layer.DimensionCodec;
            NbtTag DimensionNbt = layer.DimensionNbt;
            string WorldName = layer.WorldName;
            long HashedSeed = layer.HashedSeed;
            bool IsDebug = layer.IsDebug;
            bool IsFlat = layer.IsFlat;
            writer.WriteSignedInt(EntityId);
            writer.WriteBoolean(IsHardcore);
            writer.WriteUnsignedByte((byte)Gamemode);
            writer.WriteUnsignedByte((byte)PreviousGamemode);
            writer.WriteVarInt(WorldNames.Length);
            foreach (var worldNamesItem in WorldNames)
                writer.WriteString(worldNamesItem);
            writer.WriteNbt(DimensionCodec, true);
            writer.WriteNbt(DimensionNbt, true);
            writer.WriteString(WorldName);
            writer.WriteSignedLong(HashedSeed);
            writer.WriteVarInt(MaxPlayers);
            writer.WriteVarInt(ViewDistance);
            writer.WriteBoolean(ReducedDebugInfo);
            writer.WriteBoolean(EnableRespawnScreen);
            writer.WriteBoolean(IsDebug);
            writer.WriteBoolean(IsFlat);
            return;
        }

        if (protocolVersion >= 755 && protocolVersion <= 756)
        {
            var layer = V755_756 ?? throw new WrongLayerException("LoginPacket", protocolVersion, "V755_756");
            bool IsHardcore = layer.IsHardcore;
            int Gamemode = layer.Gamemode;
            int PreviousGamemode = layer.PreviousGamemode;
            NbtTag DimensionCodec = layer.DimensionCodec;
            NbtTag DimensionNbt = layer.DimensionNbt;
            string WorldName = layer.WorldName;
            long HashedSeed = layer.HashedSeed;
            bool IsDebug = layer.IsDebug;
            bool IsFlat = layer.IsFlat;
            writer.WriteSignedInt(EntityId);
            writer.WriteBoolean(IsHardcore);
            writer.WriteUnsignedByte((byte)Gamemode);
            writer.WriteSignedByte((sbyte)PreviousGamemode);
            writer.WriteVarInt(WorldNames.Length);
            foreach (var worldNamesItem in WorldNames)
                writer.WriteString(worldNamesItem);
            writer.WriteNbt(DimensionCodec, true);
            writer.WriteNbt(DimensionNbt, true);
            writer.WriteString(WorldName);
            writer.WriteSignedLong(HashedSeed);
            writer.WriteVarInt(MaxPlayers);
            writer.WriteVarInt(ViewDistance);
            writer.WriteBoolean(ReducedDebugInfo);
            writer.WriteBoolean(EnableRespawnScreen);
            writer.WriteBoolean(IsDebug);
            writer.WriteBoolean(IsFlat);
            return;
        }

        if (protocolVersion >= 757 && protocolVersion <= 758)
        {
            var layer = V757_758 ?? throw new WrongLayerException("LoginPacket", protocolVersion, "V757_758");
            bool IsHardcore = layer.IsHardcore;
            int Gamemode = layer.Gamemode;
            int PreviousGamemode = layer.PreviousGamemode;
            NbtTag DimensionCodec = layer.DimensionCodec;
            NbtTag DimensionNbt = layer.DimensionNbt;
            string WorldName = layer.WorldName;
            long HashedSeed = layer.HashedSeed;
            int SimulationDistance = layer.SimulationDistance;
            bool IsDebug = layer.IsDebug;
            bool IsFlat = layer.IsFlat;
            writer.WriteSignedInt(EntityId);
            writer.WriteBoolean(IsHardcore);
            writer.WriteUnsignedByte((byte)Gamemode);
            writer.WriteSignedByte((sbyte)PreviousGamemode);
            writer.WriteVarInt(WorldNames.Length);
            foreach (var worldNamesItem in WorldNames)
                writer.WriteString(worldNamesItem);
            writer.WriteNbt(DimensionCodec, true);
            writer.WriteNbt(DimensionNbt, true);
            writer.WriteString(WorldName);
            writer.WriteSignedLong(HashedSeed);
            writer.WriteVarInt(MaxPlayers);
            writer.WriteVarInt(ViewDistance);
            writer.WriteVarInt(SimulationDistance);
            writer.WriteBoolean(ReducedDebugInfo);
            writer.WriteBoolean(EnableRespawnScreen);
            writer.WriteBoolean(IsDebug);
            writer.WriteBoolean(IsFlat);
            return;
        }

        if (protocolVersion >= 759 && protocolVersion <= 762)
        {
            var layer = V759_762 ?? throw new WrongLayerException("LoginPacket", protocolVersion, "V759_762");
            bool IsHardcore = layer.IsHardcore;
            int Gamemode = layer.Gamemode;
            int PreviousGamemode = layer.PreviousGamemode;
            NbtTag DimensionCodec = layer.DimensionCodec;
            string WorldType = layer.WorldType;
            string WorldName = layer.WorldName;
            long HashedSeed = layer.HashedSeed;
            int SimulationDistance = layer.SimulationDistance;
            bool IsDebug = layer.IsDebug;
            bool IsFlat = layer.IsFlat;
            DeathLocation? Death = layer.Death;
            writer.WriteSignedInt(EntityId);
            writer.WriteBoolean(IsHardcore);
            writer.WriteUnsignedByte((byte)Gamemode);
            writer.WriteSignedByte((sbyte)PreviousGamemode);
            writer.WriteVarInt(WorldNames.Length);
            foreach (var worldNamesItem in WorldNames)
                writer.WriteString(worldNamesItem);
            writer.WriteNbt(DimensionCodec, true);
            writer.WriteString(WorldType);
            writer.WriteString(WorldName);
            writer.WriteSignedLong(HashedSeed);
            writer.WriteVarInt(MaxPlayers);
            writer.WriteVarInt(ViewDistance);
            writer.WriteVarInt(SimulationDistance);
            writer.WriteBoolean(ReducedDebugInfo);
            writer.WriteBoolean(EnableRespawnScreen);
            writer.WriteBoolean(IsDebug);
            writer.WriteBoolean(IsFlat);
            writer.WriteBoolean(Death is not null);
            if (Death is { } deathValue)
                writer.WriteType<DeathLocation>(deathValue, protocolVersion);
            return;
        }

        if (protocolVersion >= 763 && protocolVersion <= 763)
        {
            var layer = V763 ?? throw new WrongLayerException("LoginPacket", protocolVersion, "V763");
            bool IsHardcore = layer.IsHardcore;
            int Gamemode = layer.Gamemode;
            int PreviousGamemode = layer.PreviousGamemode;
            NbtTag DimensionCodec = layer.DimensionCodec;
            string WorldType = layer.WorldType;
            string WorldName = layer.WorldName;
            long HashedSeed = layer.HashedSeed;
            int SimulationDistance = layer.SimulationDistance;
            bool IsDebug = layer.IsDebug;
            bool IsFlat = layer.IsFlat;
            DeathLocation? Death = layer.Death;
            int PortalCooldown = layer.PortalCooldown;
            writer.WriteSignedInt(EntityId);
            writer.WriteBoolean(IsHardcore);
            writer.WriteUnsignedByte((byte)Gamemode);
            writer.WriteSignedByte((sbyte)PreviousGamemode);
            writer.WriteVarInt(WorldNames.Length);
            foreach (var worldNamesItem in WorldNames)
                writer.WriteString(worldNamesItem);
            writer.WriteNbt(DimensionCodec, true);
            writer.WriteString(WorldType);
            writer.WriteString(WorldName);
            writer.WriteSignedLong(HashedSeed);
            writer.WriteVarInt(MaxPlayers);
            writer.WriteVarInt(ViewDistance);
            writer.WriteVarInt(SimulationDistance);
            writer.WriteBoolean(ReducedDebugInfo);
            writer.WriteBoolean(EnableRespawnScreen);
            writer.WriteBoolean(IsDebug);
            writer.WriteBoolean(IsFlat);
            writer.WriteBoolean(Death is not null);
            if (Death is { } deathValue)
                writer.WriteType<DeathLocation>(deathValue, protocolVersion);
            writer.WriteVarInt(PortalCooldown);
            return;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            var layer = V764_765 ?? throw new WrongLayerException("LoginPacket", protocolVersion, "V764_765");
            bool IsHardcore = layer.IsHardcore;
            int Gamemode = layer.Gamemode;
            int PreviousGamemode = layer.PreviousGamemode;
            string WorldType = layer.WorldType;
            string WorldName = layer.WorldName;
            long HashedSeed = layer.HashedSeed;
            int SimulationDistance = layer.SimulationDistance;
            bool IsDebug = layer.IsDebug;
            bool IsFlat = layer.IsFlat;
            DeathLocation? Death = layer.Death;
            int PortalCooldown = layer.PortalCooldown;
            bool DoLimitedCrafting = layer.DoLimitedCrafting;
            writer.WriteSignedInt(EntityId);
            writer.WriteBoolean(IsHardcore);
            writer.WriteVarInt(WorldNames.Length);
            foreach (var worldNamesItem in WorldNames)
                writer.WriteString(worldNamesItem);
            writer.WriteVarInt(MaxPlayers);
            writer.WriteVarInt(ViewDistance);
            writer.WriteVarInt(SimulationDistance);
            writer.WriteBoolean(ReducedDebugInfo);
            writer.WriteBoolean(EnableRespawnScreen);
            writer.WriteBoolean(DoLimitedCrafting);
            writer.WriteString(WorldType);
            writer.WriteString(WorldName);
            writer.WriteSignedLong(HashedSeed);
            writer.WriteUnsignedByte((byte)Gamemode);
            writer.WriteSignedByte((sbyte)PreviousGamemode);
            writer.WriteBoolean(IsDebug);
            writer.WriteBoolean(IsFlat);
            writer.WriteBoolean(Death is not null);
            if (Death is { } deathValue)
                writer.WriteType<DeathLocation>(deathValue, protocolVersion);
            writer.WriteVarInt(PortalCooldown);
            return;
        }

        if (protocolVersion >= 766 && protocolVersion <= 775)
        {
            var layer = V766_775 ?? throw new WrongLayerException("LoginPacket", protocolVersion, "V766_775");
            bool IsHardcore = layer.IsHardcore;
            int SimulationDistance = layer.SimulationDistance;
            bool DoLimitedCrafting = layer.DoLimitedCrafting;
            SpawnInfo WorldState = layer.WorldState;
            bool EnforcesSecureChat = layer.EnforcesSecureChat;
            writer.WriteSignedInt(EntityId);
            writer.WriteBoolean(IsHardcore);
            writer.WriteVarInt(WorldNames.Length);
            foreach (var worldNamesItem in WorldNames)
                writer.WriteString(worldNamesItem);
            writer.WriteVarInt(MaxPlayers);
            writer.WriteVarInt(ViewDistance);
            writer.WriteVarInt(SimulationDistance);
            writer.WriteBoolean(ReducedDebugInfo);
            writer.WriteBoolean(EnableRespawnScreen);
            writer.WriteBoolean(DoLimitedCrafting);
            writer.WriteType<SpawnInfo>(WorldState, protocolVersion);
            writer.WriteBoolean(EnforcesSecureChat);
            return;
        }

        if (protocolVersion >= 776)
        {
            var layer = V776_Last ?? throw new WrongLayerException("LoginPacket", protocolVersion, "V776_Last");
            bool IsHardcore = layer.IsHardcore;
            int SimulationDistance = layer.SimulationDistance;
            bool DoLimitedCrafting = layer.DoLimitedCrafting;
            SpawnInfo WorldState = layer.WorldState;
            bool OnlineMode = layer.OnlineMode;
            bool EnforcesSecureChat = layer.EnforcesSecureChat;
            writer.WriteSignedInt(EntityId);
            writer.WriteBoolean(IsHardcore);
            writer.WriteVarInt(WorldNames.Length);
            foreach (var worldNamesItem in WorldNames)
                writer.WriteString(worldNamesItem);
            writer.WriteVarInt(MaxPlayers);
            writer.WriteVarInt(ViewDistance);
            writer.WriteVarInt(SimulationDistance);
            writer.WriteBoolean(ReducedDebugInfo);
            writer.WriteBoolean(EnableRespawnScreen);
            writer.WriteBoolean(DoLimitedCrafting);
            writer.WriteType<SpawnInfo>(WorldState, protocolVersion);
            writer.WriteBoolean(OnlineMode);
            writer.WriteBoolean(EnforcesSecureChat);
            return;
        }

        throw new System.NotSupportedException($"LoginPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("EntityId");
        writer.WriteNumberValue(EntityId);
        writer.WritePropertyName("WorldNames");
        writer.WriteStartArray();
        foreach (var item0 in WorldNames)
        {
            writer.WriteStringValue(item0);
        }

        writer.WriteEndArray();
        writer.WritePropertyName("MaxPlayers");
        writer.WriteNumberValue(MaxPlayers);
        writer.WritePropertyName("ViewDistance");
        writer.WriteNumberValue(ViewDistance);
        writer.WritePropertyName("ReducedDebugInfo");
        writer.WriteBooleanValue(ReducedDebugInfo);
        writer.WritePropertyName("EnableRespawnScreen");
        writer.WriteBooleanValue(EnableRespawnScreen);
        if (VUntil736 is { } vUntil736)
        {
            writer.WritePropertyName("Gamemode");
            writer.WriteNumberValue(vUntil736.Gamemode);
            writer.WritePropertyName("PreviousGamemode");
            writer.WriteNumberValue(vUntil736.PreviousGamemode);
            writer.WritePropertyName("DimensionCodec");
            vUntil736.DimensionCodec.WriteJson(writer);
            writer.WritePropertyName("Dimension");
            writer.WriteStringValue(vUntil736.Dimension);
            writer.WritePropertyName("WorldName");
            writer.WriteStringValue(vUntil736.WorldName);
            writer.WritePropertyName("HashedSeed");
            writer.WriteNumberValue(vUntil736.HashedSeed);
            writer.WritePropertyName("IsDebug");
            writer.WriteBooleanValue(vUntil736.IsDebug);
            writer.WritePropertyName("IsFlat");
            writer.WriteBooleanValue(vUntil736.IsFlat);
        }
        else if (V751_754 is { } v751_754)
        {
            writer.WritePropertyName("IsHardcore");
            writer.WriteBooleanValue(v751_754.IsHardcore);
            writer.WritePropertyName("Gamemode");
            writer.WriteNumberValue(v751_754.Gamemode);
            writer.WritePropertyName("PreviousGamemode");
            writer.WriteNumberValue(v751_754.PreviousGamemode);
            writer.WritePropertyName("DimensionCodec");
            v751_754.DimensionCodec.WriteJson(writer);
            writer.WritePropertyName("DimensionNbt");
            v751_754.DimensionNbt.WriteJson(writer);
            writer.WritePropertyName("WorldName");
            writer.WriteStringValue(v751_754.WorldName);
            writer.WritePropertyName("HashedSeed");
            writer.WriteNumberValue(v751_754.HashedSeed);
            writer.WritePropertyName("IsDebug");
            writer.WriteBooleanValue(v751_754.IsDebug);
            writer.WritePropertyName("IsFlat");
            writer.WriteBooleanValue(v751_754.IsFlat);
        }
        else if (V755_756 is { } v755_756)
        {
            writer.WritePropertyName("IsHardcore");
            writer.WriteBooleanValue(v755_756.IsHardcore);
            writer.WritePropertyName("Gamemode");
            writer.WriteNumberValue(v755_756.Gamemode);
            writer.WritePropertyName("PreviousGamemode");
            writer.WriteNumberValue(v755_756.PreviousGamemode);
            writer.WritePropertyName("DimensionCodec");
            v755_756.DimensionCodec.WriteJson(writer);
            writer.WritePropertyName("DimensionNbt");
            v755_756.DimensionNbt.WriteJson(writer);
            writer.WritePropertyName("WorldName");
            writer.WriteStringValue(v755_756.WorldName);
            writer.WritePropertyName("HashedSeed");
            writer.WriteNumberValue(v755_756.HashedSeed);
            writer.WritePropertyName("IsDebug");
            writer.WriteBooleanValue(v755_756.IsDebug);
            writer.WritePropertyName("IsFlat");
            writer.WriteBooleanValue(v755_756.IsFlat);
        }
        else if (V757_758 is { } v757_758)
        {
            writer.WritePropertyName("IsHardcore");
            writer.WriteBooleanValue(v757_758.IsHardcore);
            writer.WritePropertyName("Gamemode");
            writer.WriteNumberValue(v757_758.Gamemode);
            writer.WritePropertyName("PreviousGamemode");
            writer.WriteNumberValue(v757_758.PreviousGamemode);
            writer.WritePropertyName("DimensionCodec");
            v757_758.DimensionCodec.WriteJson(writer);
            writer.WritePropertyName("DimensionNbt");
            v757_758.DimensionNbt.WriteJson(writer);
            writer.WritePropertyName("WorldName");
            writer.WriteStringValue(v757_758.WorldName);
            writer.WritePropertyName("HashedSeed");
            writer.WriteNumberValue(v757_758.HashedSeed);
            writer.WritePropertyName("SimulationDistance");
            writer.WriteNumberValue(v757_758.SimulationDistance);
            writer.WritePropertyName("IsDebug");
            writer.WriteBooleanValue(v757_758.IsDebug);
            writer.WritePropertyName("IsFlat");
            writer.WriteBooleanValue(v757_758.IsFlat);
        }
        else if (V759_762 is { } v759_762)
        {
            writer.WritePropertyName("IsHardcore");
            writer.WriteBooleanValue(v759_762.IsHardcore);
            writer.WritePropertyName("Gamemode");
            writer.WriteNumberValue(v759_762.Gamemode);
            writer.WritePropertyName("PreviousGamemode");
            writer.WriteNumberValue(v759_762.PreviousGamemode);
            writer.WritePropertyName("DimensionCodec");
            v759_762.DimensionCodec.WriteJson(writer);
            writer.WritePropertyName("WorldType");
            writer.WriteStringValue(v759_762.WorldType);
            writer.WritePropertyName("WorldName");
            writer.WriteStringValue(v759_762.WorldName);
            writer.WritePropertyName("HashedSeed");
            writer.WriteNumberValue(v759_762.HashedSeed);
            writer.WritePropertyName("SimulationDistance");
            writer.WriteNumberValue(v759_762.SimulationDistance);
            writer.WritePropertyName("IsDebug");
            writer.WriteBooleanValue(v759_762.IsDebug);
            writer.WritePropertyName("IsFlat");
            writer.WriteBooleanValue(v759_762.IsFlat);
            if (v759_762.Death is { } deathValue)
            {
                writer.WritePropertyName("Death");
                deathValue.WriteJson(writer);
            }
        }
        else if (V763 is { } v763)
        {
            writer.WritePropertyName("IsHardcore");
            writer.WriteBooleanValue(v763.IsHardcore);
            writer.WritePropertyName("Gamemode");
            writer.WriteNumberValue(v763.Gamemode);
            writer.WritePropertyName("PreviousGamemode");
            writer.WriteNumberValue(v763.PreviousGamemode);
            writer.WritePropertyName("DimensionCodec");
            v763.DimensionCodec.WriteJson(writer);
            writer.WritePropertyName("WorldType");
            writer.WriteStringValue(v763.WorldType);
            writer.WritePropertyName("WorldName");
            writer.WriteStringValue(v763.WorldName);
            writer.WritePropertyName("HashedSeed");
            writer.WriteNumberValue(v763.HashedSeed);
            writer.WritePropertyName("SimulationDistance");
            writer.WriteNumberValue(v763.SimulationDistance);
            writer.WritePropertyName("IsDebug");
            writer.WriteBooleanValue(v763.IsDebug);
            writer.WritePropertyName("IsFlat");
            writer.WriteBooleanValue(v763.IsFlat);
            if (v763.Death is { } deathValue)
            {
                writer.WritePropertyName("Death");
                deathValue.WriteJson(writer);
            }

            writer.WritePropertyName("PortalCooldown");
            writer.WriteNumberValue(v763.PortalCooldown);
        }
        else if (V764_765 is { } v764_765)
        {
            writer.WritePropertyName("IsHardcore");
            writer.WriteBooleanValue(v764_765.IsHardcore);
            writer.WritePropertyName("Gamemode");
            writer.WriteNumberValue(v764_765.Gamemode);
            writer.WritePropertyName("PreviousGamemode");
            writer.WriteNumberValue(v764_765.PreviousGamemode);
            writer.WritePropertyName("WorldType");
            writer.WriteStringValue(v764_765.WorldType);
            writer.WritePropertyName("WorldName");
            writer.WriteStringValue(v764_765.WorldName);
            writer.WritePropertyName("HashedSeed");
            writer.WriteNumberValue(v764_765.HashedSeed);
            writer.WritePropertyName("SimulationDistance");
            writer.WriteNumberValue(v764_765.SimulationDistance);
            writer.WritePropertyName("IsDebug");
            writer.WriteBooleanValue(v764_765.IsDebug);
            writer.WritePropertyName("IsFlat");
            writer.WriteBooleanValue(v764_765.IsFlat);
            if (v764_765.Death is { } deathValue)
            {
                writer.WritePropertyName("Death");
                deathValue.WriteJson(writer);
            }

            writer.WritePropertyName("PortalCooldown");
            writer.WriteNumberValue(v764_765.PortalCooldown);
            writer.WritePropertyName("DoLimitedCrafting");
            writer.WriteBooleanValue(v764_765.DoLimitedCrafting);
        }
        else if (V766_775 is { } v766_775)
        {
            writer.WritePropertyName("IsHardcore");
            writer.WriteBooleanValue(v766_775.IsHardcore);
            writer.WritePropertyName("SimulationDistance");
            writer.WriteNumberValue(v766_775.SimulationDistance);
            writer.WritePropertyName("DoLimitedCrafting");
            writer.WriteBooleanValue(v766_775.DoLimitedCrafting);
            writer.WritePropertyName("WorldState");
            v766_775.WorldState.WriteJson(writer);
            writer.WritePropertyName("EnforcesSecureChat");
            writer.WriteBooleanValue(v766_775.EnforcesSecureChat);
        }
        else if (V776_Last is { } v776_Last)
        {
            writer.WritePropertyName("IsHardcore");
            writer.WriteBooleanValue(v776_Last.IsHardcore);
            writer.WritePropertyName("SimulationDistance");
            writer.WriteNumberValue(v776_Last.SimulationDistance);
            writer.WritePropertyName("DoLimitedCrafting");
            writer.WriteBooleanValue(v776_Last.DoLimitedCrafting);
            writer.WritePropertyName("WorldState");
            v776_Last.WorldState.WriteJson(writer);
            writer.WritePropertyName("OnlineMode");
            writer.WriteBooleanValue(v776_Last.OnlineMode);
            writer.WritePropertyName("EnforcesSecureChat");
            writer.WriteBooleanValue(v776_Last.EnforcesSecureChat);
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.login", "Login", PacketPhase.Play, PacketDirection.Clientbound, 56);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
