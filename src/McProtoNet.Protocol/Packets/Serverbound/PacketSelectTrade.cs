using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets
{
    public class SelectTradePacket : IClientPacket
    {
        public int Slot { get; set; }

        internal sealed class V393_769 : SelectTradePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Slot);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int slot)
            {
                writer.WriteVarInt(slot);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 393 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V393_769.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V393_769.SupportedVersion(protocolVersion))
                V393_769.SerializeInternal(ref writer, protocolVersion, Slot);
            else
                throw new ProtocolNotSupportException(nameof(ClientPacket.SelectTrade), protocolVersion);
        }

        public static ClientPacket PacketId => ClientPacket.SelectTrade;

        public ClientPacket GetPacketId() => PacketId;
    }
}