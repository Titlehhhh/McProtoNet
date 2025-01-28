using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("Camera", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class CameraPacket : IServerPacket
    {
        public int CameraId { get; set; }

        [PacketSubInfo(340, 769)]
        internal sealed partial class V340_769 : CameraPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                CameraId = reader.ReadVarInt();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}