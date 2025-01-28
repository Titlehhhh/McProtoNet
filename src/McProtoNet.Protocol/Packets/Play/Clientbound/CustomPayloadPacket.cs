using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("CustomPayload", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class CustomPayloadPacket : IServerPacket
    {
        public string Channel { get; set; }
        public byte[] Data { get; set; }

        [PacketSubInfo(340, 769)]
        internal sealed partial class V340_769 : CustomPayloadPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Channel = reader.ReadString();
                Data = reader.ReadRestBuffer();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}