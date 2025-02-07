using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("CustomPayload", PacketState.Play, PacketDirection.Serverbound)]
    public partial class CustomPayloadPacket : IClientPacket
    {
        public string Channel { get; set; }
        public byte[] Data { get; set; }

        [PacketSubInfo(340, 769)]
        internal sealed partial class V340_769 : CustomPayloadPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Channel, Data);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                string channel, byte[] data)
            {
                writer.WriteString(channel);
                writer.WriteBuffer(data);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_769.IsSupportedVersionStatic(protocolVersion))
                V340_769.SerializeInternal(ref writer, protocolVersion, Channel, Data);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.CustomPayload), protocolVersion);
        }
    }
}