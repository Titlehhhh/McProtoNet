using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class InitializeWorldBorderPacket : IServerPacket
    {
        public double X { get; set; }
        public double Z { get; set; }
        public double OldDiameter { get; set; }
        public double NewDiameter { get; set; }
        public int PortalTeleportBoundary { get; set; }
        public int WarningBlocks { get; set; }
        public int WarningTime { get; set; }

        public sealed class V755_758 : InitializeWorldBorderPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                X = reader.ReadDouble();
                Z = reader.ReadDouble();
                OldDiameter = reader.ReadDouble();
                NewDiameter = reader.ReadDouble();
                Speed = reader.ReadVarLong();
                PortalTeleportBoundary = reader.ReadVarInt();
                WarningBlocks = reader.ReadVarInt();
                WarningTime = reader.ReadVarInt();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 755 and <= 758;
            }

            public long Speed { get; set; }
        }

        public sealed class V759_769 : InitializeWorldBorderPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                X = reader.ReadDouble();
                Z = reader.ReadDouble();
                OldDiameter = reader.ReadDouble();
                NewDiameter = reader.ReadDouble();
                Speed = reader.ReadVarInt();
                PortalTeleportBoundary = reader.ReadVarInt();
                WarningBlocks = reader.ReadVarInt();
                WarningTime = reader.ReadVarInt();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 759 and <= 769;
            }

            public int Speed { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V755_758.SupportedVersion(protocolVersion) || V759_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static PacketIdentifier PacketId => ServerPlayPacket.InitializeWorldBorder;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}