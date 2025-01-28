using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("SetCreativeSlot", PacketState.Play, PacketDirection.Serverbound)]
    public partial class SetCreativeSlotPacket : IClientPacket
    {
        public short Slot { get; set; }
        public Slot Item { get; set; }

        [PacketSubInfo(340, 765)]
        internal sealed partial class V340_765 : SetCreativeSlotPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Slot, Item);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, short slot, Slot item)
            {
                writer.WriteSignedShort(slot);
                writer.WriteSlot(item, protocolVersion);
            }
        }

        [PacketSubInfo(766, 769)]
        internal sealed partial class V766_769 : SetCreativeSlotPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Slot, Item);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, short slot, Slot item)
            {
                writer.WriteSignedShort(slot);
                writer.WriteSlot(item, protocolVersion);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_765.IsSupportedVersionStatic(protocolVersion))
                V340_765.SerializeInternal(ref writer, protocolVersion, Slot, Item);
            else if (V766_769.IsSupportedVersionStatic(protocolVersion))
                V766_769.SerializeInternal(ref writer, protocolVersion, Slot, Item);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.SetCreativeSlot), protocolVersion);
        }
    }
}