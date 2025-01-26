using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class WorldBorderCenterPacket : IServerPacket
    {
        public double X { get; set; }
        public double Z { get; set; }

        public sealed class V755_769 : WorldBorderCenterPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                X = reader.ReadDouble();
                Z = reader.ReadDouble();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 755 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V755_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static PacketIdentifier PacketId => ServerPlayPacket.WorldBorderCenter;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}