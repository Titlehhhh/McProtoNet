using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("HurtAnimation", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class HurtAnimationPacket : IServerPacket
    {
        public int EntityId { get; set; }
        public float Yaw { get; set; }

        [PacketSubInfo(762, 769)]
        public sealed partial class V762_769 : HurtAnimationPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                Yaw = reader.ReadFloat();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}