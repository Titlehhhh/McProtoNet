using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class AttachEntityPacket : IServerPacket
    {
        public int EntityId { get; set; }
        public int VehicleId { get; set; }

        internal sealed class V340_769 : AttachEntityPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadSignedInt();
                VehicleId = reader.ReadSignedInt();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static PacketIdentifier PacketId => ServerPlayPacket.AttachEntity;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}