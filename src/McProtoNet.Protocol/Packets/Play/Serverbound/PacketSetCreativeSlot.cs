using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets.Play
{
    public class SetCreativeSlotPacket : IClientPacket
    {
        public short Slot { get; set; }
        public Slot Item { get; set; }

        internal sealed class V340_765 : SetCreativeSlotPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Slot, Item);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, short slot,
                Slot item)
            {
                writer.WriteSignedShort(slot);
                writer.WriteSlot(item, protocolVersion);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 765;
            }
        }

        internal sealed class V766_769 : SetCreativeSlotPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Slot, Item);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, short slot,
                Slot item)
            {
                writer.WriteSignedShort(slot);
                writer.WriteSlot(item, protocolVersion);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 766 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_765.SupportedVersion(protocolVersion) || V766_769.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_765.SupportedVersion(protocolVersion))
                V340_765.SerializeInternal(ref writer, protocolVersion, Slot, Item);
            else if (V766_769.SupportedVersion(protocolVersion))
                V766_769.SerializeInternal(ref writer, protocolVersion, Slot, Item);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.SetCreativeSlot), protocolVersion);
        }

        public static PacketIdentifier PacketId => ClientPlayPacket.SetCreativeSlot;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}