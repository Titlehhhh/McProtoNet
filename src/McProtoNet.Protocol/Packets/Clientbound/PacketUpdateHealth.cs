using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets
{
    public abstract class UpdateHealthPacket : IServerPacket
    {
        public float Health { get; set; }
        public int Food { get; set; }
        public float FoodSaturation { get; set; }

        internal sealed class V340_769 : UpdateHealthPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Health = reader.ReadFloat();
                Food = reader.ReadVarInt();
                FoodSaturation = reader.ReadFloat();
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
        public static ServerPacket PacketId => ServerPacket.UpdateHealth;

        public ServerPacket GetPacketId() => PacketId;
    }
}