using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class SetPlayerInventoryPacket : IServerPacket
    {
        public int SlotId { get; set; }
        public Slot? Contents { get; set; }

        public sealed class V768_769 : SetPlayerInventoryPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                SlotId = reader.ReadVarInt();
                Contents = reader.ReadOptional((ref MinecraftPrimitiveReader r_0) => r_0.ReadSlot(protocolVersion));
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 768 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V768_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static PacketIdentifier PacketId => ServerPlayPacket.SetPlayerInventory;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}