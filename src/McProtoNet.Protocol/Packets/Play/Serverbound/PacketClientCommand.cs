using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets.Play
{
    public class ClientCommandPacket : IClientPacket
    {
        public int ActionId { get; set; }

        internal sealed class V340_769 : ClientCommandPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, ActionId);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int actionId)
            {
                writer.WriteVarInt(actionId);
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
                V340_769.SerializeInternal(ref writer, protocolVersion, ActionId);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.ClientCommand), protocolVersion);
        }

        public static PacketIdentifier PacketId => ClientPlayPacket.ClientCommand;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}