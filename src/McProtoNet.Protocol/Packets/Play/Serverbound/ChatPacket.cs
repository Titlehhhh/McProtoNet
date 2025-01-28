using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("Chat", PacketState.Play, PacketDirection.Serverbound)]
    public partial class ChatPacket : IClientPacket
    {
        public string Message { get; set; }

        [PacketSubInfo(340, 758)]
        public sealed partial class V340_758 : ChatPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Message);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, string message)
            {
                writer.WriteString(message);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_758.IsSupportedVersionStatic(protocolVersion))
                V340_758.SerializeInternal(ref writer, protocolVersion, Message);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.Chat), protocolVersion);
        }
    }
}