using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("Entity", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class EntityPacket : IServerPacket
    {
        public int EntityId { get; set; }

        [PacketSubInfo(340, 754)]
        public sealed partial class V340_754 : EntityPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}