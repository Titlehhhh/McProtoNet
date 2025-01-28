using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("PlayerRotation", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class PlayerRotationPacket : IServerPacket
    {
        public float Yaw { get; set; }
        public float Pitch { get; set; }

        [PacketSubInfo(768, 769)]
        public sealed partial class V768_769 : PlayerRotationPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Yaw = reader.ReadFloat();
                Pitch = reader.ReadFloat();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}