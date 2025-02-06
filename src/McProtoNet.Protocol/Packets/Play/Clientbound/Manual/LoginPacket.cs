using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("Login", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class LoginPacket : IServerPacket
    {
        public int EntityId { get; set; }

        [PacketSubInfo(340, 404)]
        public sealed partial class V340_404 : LoginPacket
        {
            public byte GameMode { get; set; }
            public int Dimension { get; set; }
            public byte Difficulty { get; set; }
            public byte MaxPlayers { get; set; }
            public string LevelType { get; set; }
            public bool ReducedDebugInfo { get; set; }

            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadSignedInt();
                GameMode = reader.ReadUnsignedByte();
                Dimension = reader.ReadSignedInt();
                Difficulty = reader.ReadUnsignedByte();
                MaxPlayers = reader.ReadUnsignedByte();
                LevelType = reader.ReadString();
                ReducedDebugInfo = reader.ReadBoolean();
            }
        }

        [PacketSubInfo(477, 498)]
        public sealed partial class V477_498 : LoginPacket
        {
            public byte GameMode { get; set; }
            public int Dimension { get; set; }
            public byte MaxPlayers { get; set; }
            public string LevelType { get; set; }
            public int ViewDistance { get; set; }
            public bool ReducedDebugInfo { get; set; }

            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadSignedInt();
                GameMode = reader.ReadUnsignedByte();
                Dimension = reader.ReadSignedInt();
                MaxPlayers = reader.ReadUnsignedByte();
                LevelType = reader.ReadString();
                ViewDistance = reader.ReadVarInt();
                ReducedDebugInfo = reader.ReadBoolean();
            }
        }

        [PacketSubInfo(573, 710)]
        public sealed partial class V573_710 : LoginPacket
        {
            public byte GameMode { get; set; }
            public int Dimension { get; set; }
            public long HashedSeed { get; set; }
            public byte MaxPlayers { get; set; }
            public string LevelType { get; set; }
            public int ViewDistance { get; set; }
            public bool ReducedDebugInfo { get; set; }
            public bool EnableRespawnScreen { get; set; }

            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadSignedInt();
                GameMode = reader.ReadUnsignedByte();
                Dimension = reader.ReadSignedInt();
                HashedSeed = reader.ReadSignedLong();
                MaxPlayers = reader.ReadUnsignedByte();
                LevelType = reader.ReadString();
                ViewDistance = reader.ReadVarInt();
                ReducedDebugInfo = reader.ReadBoolean();
                EnableRespawnScreen = reader.ReadBoolean();
            }
        }

        [PacketSubInfo(734, 736)]
        public sealed partial class V734_736 : LoginPacket
        {
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

            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadSignedInt();
                GameMode = reader.ReadUnsignedByte();
                PreviousGameMode = reader.ReadUnsignedByte();
                WorldNames = reader.ReadArray(reader.ReadVarInt(),
                    (ref MinecraftPrimitiveReader reader) => reader.ReadString());
                DimensionCodec = reader.ReadNbtTag(protocolVersion);
                Dimension = reader.ReadString();
                WorldName = reader.ReadString();
                HashedSeed = reader.ReadSignedLong();
                MaxPlayers = reader.ReadUnsignedByte();
                ViewDistance = reader.ReadVarInt();

                ReducedDebugInfo = reader.ReadBoolean();
                EnableRespawnScreen = reader.ReadBoolean();
                IsDebug = reader.ReadBoolean();
                IsFlat = reader.ReadBoolean();
            }
        }

        [PacketSubInfo(751, 754)]
        public sealed partial class V751_754 : LoginPacket
        {
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

            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadSignedInt();
                IsHardcore = reader.ReadBoolean();
                GameMode = reader.ReadUnsignedByte();
                PreviousGameMode = reader.ReadUnsignedByte();
                WorldNames = reader.ReadArray(reader.ReadVarInt(),
                    (ref MinecraftPrimitiveReader reader) => reader.ReadString());
                DimensionCodec = reader.ReadNbtTag(protocolVersion);
                Dimension = reader.ReadNbtTag(protocolVersion);
                WorldName = reader.ReadString();
                HashedSeed = reader.ReadSignedLong();
                MaxPlayers = reader.ReadVarInt();
                ViewDistance = reader.ReadVarInt();

                ReducedDebugInfo = reader.ReadBoolean();
                EnableRespawnScreen = reader.ReadBoolean();
                IsDebug = reader.ReadBoolean();
                IsFlat = reader.ReadBoolean();
            }
        }

        [PacketSubInfo(755, 756)]
        public sealed partial class V755_756 : LoginPacket
        {
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

            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadSignedInt();
                IsHardcore = reader.ReadBoolean();
                GameMode = reader.ReadUnsignedByte();
                PreviousGameMode = reader.ReadSignedByte();
                WorldNames = reader.ReadArray(reader.ReadVarInt(),
                    (ref MinecraftPrimitiveReader reader) => reader.ReadString());
                DimensionCodec = reader.ReadNbtTag(protocolVersion);
                Dimension = reader.ReadNbtTag(protocolVersion);
                WorldName = reader.ReadString();
                HashedSeed = reader.ReadSignedLong();
                MaxPlayers = reader.ReadVarInt();
                ViewDistance = reader.ReadVarInt();
                ReducedDebugInfo = reader.ReadBoolean();
                EnableRespawnScreen = reader.ReadBoolean();
                IsDebug = reader.ReadBoolean();
                IsFlat = reader.ReadBoolean();
            }
        }

        [PacketSubInfo(757, 758)]
        public sealed partial class V757_758 : LoginPacket
        {
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

            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadSignedInt();
                IsHardcore = reader.ReadBoolean();
                GameMode = reader.ReadUnsignedByte();
                PreviousGameMode = reader.ReadSignedByte();
                WorldNames = reader.ReadArray(reader.ReadVarInt(),
                    (ref MinecraftPrimitiveReader reader) => reader.ReadString());
                DimensionCodec = reader.ReadNbtTag(protocolVersion);
                Dimension = reader.ReadNbtTag(protocolVersion);
                WorldName = reader.ReadString();
                HashedSeed = reader.ReadSignedLong();

                MaxPlayers = reader.ReadVarInt();
                ViewDistance = reader.ReadVarInt();
                SimulationDistance = reader.ReadVarInt();
                ReducedDebugInfo = reader.ReadBoolean();
                EnableRespawnScreen = reader.ReadBoolean();
                IsDebug = reader.ReadBoolean();
                IsFlat = reader.ReadBoolean();
            }
        }

        [PacketSubInfo(759, 762)]
        public sealed partial class V759_762 : LoginPacket
        {
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

            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadSignedInt();
                IsHardcore = reader.ReadBoolean();
                GameMode = reader.ReadUnsignedByte();
                PreviousGameMode = reader.ReadSignedByte();
                WorldNames = reader.ReadArray(reader.ReadVarInt(),
                    (ref MinecraftPrimitiveReader reader) => reader.ReadString());
                DimensionCodec = reader.ReadNbtTag(protocolVersion);
                WorldType = reader.ReadString();
                WorldName = reader.ReadString();
                HashedSeed = reader.ReadSignedLong();
                MaxPlayers = reader.ReadVarInt();
                ViewDistance = reader.ReadVarInt();

                SimulationDistance = reader.ReadVarInt();
                ReducedDebugInfo = reader.ReadBoolean();
                EnableRespawnScreen = reader.ReadBoolean();
                IsDebug = reader.ReadBoolean();
                IsFlat = reader.ReadBoolean();
                if (reader.ReadBoolean())
                {
                    Death = new DeathLocation
                    {
                        DimensionName = reader.ReadString(),
                        Location = reader.ReadPosition(protocolVersion)
                    };
                }
            }
        }

        [PacketSubInfo(763, 763)]
        public sealed partial class V763 : LoginPacket
        {
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

            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadSignedInt();
                IsHardcore = reader.ReadBoolean();
                GameMode = reader.ReadUnsignedByte();
                PreviousGameMode = reader.ReadSignedByte();
                WorldNames = reader.ReadArray(reader.ReadVarInt(),
                    (ref MinecraftPrimitiveReader reader) => reader.ReadString());
                DimensionCodec = reader.ReadNbtTag(protocolVersion);
                WorldType = reader.ReadString();
                WorldName = reader.ReadString();
                HashedSeed = reader.ReadSignedLong();
                MaxPlayers = reader.ReadVarInt();

                ViewDistance = reader.ReadVarInt();
                SimulationDistance = reader.ReadVarInt();
                ReducedDebugInfo = reader.ReadBoolean();
                EnableRespawnScreen = reader.ReadBoolean();
                IsDebug = reader.ReadBoolean();
                IsFlat = reader.ReadBoolean();
                if (reader.ReadBoolean())
                {
                    Death = new DeathLocation
                    {
                        DimensionName = reader.ReadString(),
                        Location = reader.ReadPosition(protocolVersion)
                    };
                }

                PortalCooldown = reader.ReadVarInt();
            }
        }

        [PacketSubInfo(764, 765)]
        public sealed partial class V764_765 : LoginPacket
        {
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

            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadSignedInt();
                IsHardcore = reader.ReadBoolean();
                WorldNames = reader.ReadArray(reader.ReadVarInt(),
                    (ref MinecraftPrimitiveReader reader) => reader.ReadString());
                MaxPlayers = reader.ReadVarInt();
                ViewDistance = reader.ReadVarInt();
                SimulationDistance = reader.ReadVarInt();
                ReducedDebugInfo = reader.ReadBoolean();
                EnableRespawnScreen = reader.ReadBoolean();
                DoLimitedCrafting = reader.ReadBoolean();
                WorldType = reader.ReadString();
                WorldName = reader.ReadString();
                HashedSeed = reader.ReadSignedLong();
                GameMode = reader.ReadUnsignedByte();
                PreviousGameMode = reader.ReadSignedByte();
                IsDebug = reader.ReadBoolean();
                IsFlat = reader.ReadBoolean();
                if (reader.ReadBoolean())
                {
                    Death = new DeathLocation
                    {
                        DimensionName = reader.ReadString(),
                        Location = reader.ReadPosition(protocolVersion)
                    };
                }

                PortalCooldown = reader.ReadVarInt();
            }
        }

        [PacketSubInfo(766, 769)]
        public sealed partial class V766_769 : LoginPacket
        {
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

            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadSignedInt();
                IsHardcore = reader.ReadBoolean();
                WorldNames = reader.ReadArray(reader.ReadVarInt(),
                    (ref MinecraftPrimitiveReader reader) => reader.ReadString());
                MaxPlayers = reader.ReadVarInt();
                ViewDistance = reader.ReadVarInt();
                SimulationDistance = reader.ReadVarInt();
                ReducedDebugInfo = reader.ReadBoolean();
                EnableRespawnScreen = reader.ReadBoolean();

                DoLimitedCrafting = reader.ReadBoolean();
                WorldState = new SpawnInfo();
                WorldState.Deserialize(ref reader, protocolVersion);
                EnforcesSecureChat = reader.ReadBoolean();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }

    public sealed class DeathLocation
    {
        public string DimensionName { get; set; }
        public Position Location { get; set; }
    }

    public sealed class SpawnInfo
    {
        public int Dimension { get; set; }
        public string Name { get; set; }
        public long HashedSeed { get; set; }
        public byte Gamemode { get; set; }
        public byte PreviousGamemode { get; set; }
        public bool IsDebug { get; set; }
        public bool IsFlat { get; set; }
        public DeathLocation Death { get; set; }
        public int PortalCooldown { get; set; }
        public int? SeaLevel { get; set; }

        public void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Dimension = reader.ReadVarInt();
            Name = reader.ReadString();
            HashedSeed = reader.ReadSignedLong();
            Gamemode = reader.ReadUnsignedByte();
            PreviousGamemode = reader.ReadUnsignedByte();
            IsDebug = reader.ReadBoolean();
            IsFlat = reader.ReadBoolean();


            if (reader.ReadBoolean())
            {
                Death = new DeathLocation
                {
                    DimensionName = reader.ReadString(),
                    Location = reader.ReadPosition(protocolVersion)
                };
            }

            PortalCooldown = reader.ReadVarInt();

            if (protocolVersion >= 768)
            {
                SeaLevel = reader.ReadVarInt();
            }
        }
    }
}