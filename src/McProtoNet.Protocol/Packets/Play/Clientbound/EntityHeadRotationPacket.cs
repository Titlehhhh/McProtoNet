using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("EntityHeadRotation", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class EntityHeadRotationPacket : IServerPacket
    {
        public int EntityId { get; set; }
        public sbyte HeadYaw { get; set; }

        [PacketSubInfo(340, 769)]
        internal sealed partial class V340_769 : EntityHeadRotationPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                HeadYaw = reader.ReadSignedByte();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}