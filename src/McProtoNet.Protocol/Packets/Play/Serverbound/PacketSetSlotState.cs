using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets.Play
{
    public class SetSlotStatePacket : IClientPacket
    {
        public int SlotId { get; set; }
        public int WindowId { get; set; }
        public bool State { get; set; }

        public sealed class V765_767 : SetSlotStatePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, SlotId, WindowId, State);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int slotId, int windowId, bool state)
            {
                writer.WriteVarInt(slotId);
                writer.WriteVarInt(windowId);
                writer.WriteBoolean(state);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 765 and <= 767;
            }
        }

        public sealed class V768_769 : SetSlotStatePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, SlotId, WindowId, State);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int slotId, int windowId, bool state)
            {
                writer.WriteVarInt(slotId);
                writer.WriteVarInt(windowId);
                writer.WriteBoolean(state);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 768 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V765_767.SupportedVersion(protocolVersion) || V768_769.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V765_767.SupportedVersion(protocolVersion))
                V765_767.SerializeInternal(ref writer, protocolVersion, SlotId, WindowId, State);
            else if (V768_769.SupportedVersion(protocolVersion))
                V768_769.SerializeInternal(ref writer, protocolVersion, SlotId, WindowId, State);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.SetSlotState), protocolVersion);
        }

        public static PacketIdentifier PacketId => ClientPlayPacket.SetSlotState;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}