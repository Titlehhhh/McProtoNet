using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("UnloadChunk", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class UnloadChunkPacket : IServerPacket
    {
        public int ChunkX { get; set; }
        public int ChunkZ { get; set; }

        [PacketSubInfo(340, 763)]
        internal sealed partial class V340_763 : UnloadChunkPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                ChunkX = reader.ReadSignedInt();
                ChunkZ = reader.ReadSignedInt();
            }
        }

        [PacketSubInfo(764, 769)]
        internal sealed partial class V764_769 : UnloadChunkPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                ChunkZ = reader.ReadSignedInt();
                ChunkX = reader.ReadSignedInt();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}