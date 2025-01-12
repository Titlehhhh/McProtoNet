using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets
{
    public class ChatPacket : IClientPacket
    {
        public string Message { get; set; }

        internal sealed class V340_758 : ChatPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Message);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, string message)
            {
                writer.WriteString(message);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 758;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_758.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_758.SupportedVersion(protocolVersion))
                V340_758.SerializeInternal(ref writer, protocolVersion, Message);
            else
                throw new ProtocolNotSupportException(nameof(ClientPacket.Chat), protocolVersion);
        }

        public static ClientPacket PacketId => ClientPacket.Chat;

        public ClientPacket GetPacketId() => PacketId;
    }
}