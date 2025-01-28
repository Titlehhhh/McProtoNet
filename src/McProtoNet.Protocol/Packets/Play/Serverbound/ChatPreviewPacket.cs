using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("ChatPreview", PacketState.Play, PacketDirection.Serverbound)]
    public partial class ChatPreviewPacket : IClientPacket
    {
        public int Query { get; set; }
        public string Message { get; set; }

        [PacketSubInfo(759, 760)]
        public sealed partial class V759_760 : ChatPreviewPacket
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
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V759_760.IsSupportedVersionStatic(protocolVersion))
                V759_760.SerializeInternal(ref writer, protocolVersion, Query, Message);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.ChatPreview), protocolVersion);
        }
    }
}