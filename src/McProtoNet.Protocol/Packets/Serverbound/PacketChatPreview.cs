using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets
{
    public class ChatPreviewPacket : IClientPacket
    {
        public int Query { get; set; }
        public string Message { get; set; }

        public sealed class V759_760 : ChatPreviewPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Query, Message);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int query, string message)
            {
                writer.WriteSignedInt(query);
                writer.WriteString(message);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 759 and <= 760;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V759_760.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V759_760.SupportedVersion(protocolVersion))
                V759_760.SerializeInternal(ref writer, protocolVersion, Query, Message);
            else
                throw new ProtocolNotSupportException(nameof(ClientPacket.ChatPreview), protocolVersion);
        }

        public static ClientPacket PacketId => ClientPacket.ChatPreview;

        public ClientPacket GetPacketId() => PacketId;
    }
}