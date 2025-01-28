using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("EntityStatus", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class EntityStatusPacket : IServerPacket
    {
        public int EntityId { get; set; }
        public sbyte EntityStatus { get; set; }

        [PacketSubInfo(340, 769)]
        internal sealed partial class V340_769 : EntityStatusPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadSignedInt();
                EntityStatus = reader.ReadSignedByte();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}