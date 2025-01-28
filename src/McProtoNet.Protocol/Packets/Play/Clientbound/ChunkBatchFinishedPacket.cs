using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("ChunkBatchFinished", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class ChunkBatchFinishedPacket : IServerPacket
    {
        public int BatchSize { get; set; }

        [PacketSubInfo(764, 769)]
        public sealed partial class V764_769 : ChunkBatchFinishedPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                BatchSize = reader.ReadVarInt();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}