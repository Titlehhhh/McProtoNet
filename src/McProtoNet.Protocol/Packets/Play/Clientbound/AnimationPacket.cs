using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("Animation", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class AnimationPacket : IServerPacket
    {
        public int EntityId { get; set; }
        public byte Animation { get; set; }

        [PacketSubInfo(340, 769)]
        internal sealed partial class V340_769 : AnimationPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                Animation = reader.ReadUnsignedByte();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}