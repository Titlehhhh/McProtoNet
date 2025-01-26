using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets.Play
{
    public class HeldItemSlotPacket : IClientPacket
    {
        public short SlotId { get; set; }

        internal sealed class V340_769 : HeldItemSlotPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, SlotId);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                short slotId)
            {
                writer.WriteSignedShort(slotId);
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

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_769.SupportedVersion(protocolVersion))
                V340_769.SerializeInternal(ref writer, protocolVersion, SlotId);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.HeldItemSlot), protocolVersion);
        }

        public static PacketIdentifier PacketId => ClientPlayPacket.HeldItemSlot;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}