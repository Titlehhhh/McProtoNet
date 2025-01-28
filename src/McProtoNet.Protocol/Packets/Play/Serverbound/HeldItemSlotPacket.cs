using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("HeldItemSlot", PacketState.Play, PacketDirection.Serverbound)]
    public partial class HeldItemSlotPacket : IClientPacket
    {
        public short SlotId { get; set; }

        [PacketSubInfo(340, 769)]
        internal sealed partial class V340_769 : HeldItemSlotPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, SlotId);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, short slotId)
            {
                writer.WriteSignedShort(slotId);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_769.IsSupportedVersionStatic(protocolVersion))
                V340_769.SerializeInternal(ref writer, protocolVersion, SlotId);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.HeldItemSlot), protocolVersion);
        }
    }
}