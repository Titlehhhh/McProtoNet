using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class UpdateViewDistancePacket : IServerPacket
    {
        public int ViewDistance { get; set; }

        public sealed class V477_769 : UpdateViewDistancePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                ViewDistance = reader.ReadVarInt();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 477 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V477_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static PacketIdentifier PacketId => ServerPlayPacket.UpdateViewDistance;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}