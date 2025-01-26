using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class WorldBorderSizePacket : IServerPacket
    {
        public double Diameter { get; set; }

        public sealed class V755_769 : WorldBorderSizePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Diameter = reader.ReadDouble();
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
        public static PacketIdentifier PacketId => ServerPlayPacket.WorldBorderSize;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}