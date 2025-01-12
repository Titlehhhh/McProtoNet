using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets
{
    public class PickItemPacket : IClientPacket
    {
        public int Slot { get; set; }

        internal sealed class V393_768 : PickItemPacket
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
                return protocolVersion is >= 393 and <= 768;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V393_768.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V393_768.SupportedVersion(protocolVersion))
                V393_768.SerializeInternal(ref writer, protocolVersion, Slot);
            else
                throw new ProtocolNotSupportException(nameof(ClientPacket.PickItem), protocolVersion);
        }

        public static ClientPacket PacketId => ClientPacket.PickItem;

        public ClientPacket GetPacketId() => PacketId;
    }
}