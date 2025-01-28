using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("SelectBundleItem", PacketState.Play, PacketDirection.Serverbound)]
    public partial class SelectBundleItemPacket : IClientPacket
    {
        public int SlotId { get; set; }
        public int SelectedItemIndex { get; set; }

        [PacketSubInfo(768, 769)]
        public sealed partial class V768_769 : SelectBundleItemPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, SlotId, SelectedItemIndex);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int slotId, int selectedItemIndex)
            {
                writer.WriteVarInt(slotId);
                writer.WriteVarInt(selectedItemIndex);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V768_769.IsSupportedVersionStatic(protocolVersion))
                V768_769.SerializeInternal(ref writer, protocolVersion, SlotId, SelectedItemIndex);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.SelectBundleItem), protocolVersion);
        }
    }
}