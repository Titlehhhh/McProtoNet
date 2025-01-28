using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("ChunkBatchStart", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class ChunkBatchStartPacket : IServerPacket
    {
        [PacketSubInfo(764, 769)]
        public sealed partial class V764_769 : ChunkBatchStartPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}