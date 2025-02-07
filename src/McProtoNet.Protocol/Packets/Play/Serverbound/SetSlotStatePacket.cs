using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("SetSlotState", PacketState.Play, PacketDirection.Serverbound)]
    public partial class SetSlotStatePacket : IClientPacket
    {
        public int SlotId { get; set; }
        public int WindowId { get; set; }
        public bool State { get; set; }

        [PacketSubInfo(765, 767)]
        public sealed partial class V765_767 : SetSlotStatePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, SlotId, WindowId, State);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int slotId,
                int windowId, bool state)
            {
                writer.WriteVarInt(slotId);
                writer.WriteVarInt(windowId);
                writer.WriteBoolean(state);
            }
        }

        [PacketSubInfo(768, 769)]
        public sealed partial class V768_769 : SetSlotStatePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, SlotId, WindowId, State);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int slotId,
                int windowId, bool state)
            {
                writer.WriteVarInt(slotId);
                writer.WriteVarInt(windowId);
                writer.WriteBoolean(state);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V765_767.IsSupportedVersionStatic(protocolVersion))
                V765_767.SerializeInternal(ref writer, protocolVersion, SlotId, WindowId, State);
            else if (V768_769.IsSupportedVersionStatic(protocolVersion))
                V768_769.SerializeInternal(ref writer, protocolVersion, SlotId, WindowId, State);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.SetSlotState), protocolVersion);
        }
    }
}