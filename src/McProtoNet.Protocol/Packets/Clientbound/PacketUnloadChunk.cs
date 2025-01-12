using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets
{
    public abstract class UnloadChunkPacket : IServerPacket
    {
        public int ChunkX { get; set; }
        public int ChunkZ { get; set; }

        internal sealed class V340_763 : UnloadChunkPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                ChunkX = reader.ReadSignedInt();
                ChunkZ = reader.ReadSignedInt();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 763;
            }
        }

        internal sealed class V764_769 : UnloadChunkPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                ChunkZ = reader.ReadSignedInt();
                ChunkX = reader.ReadSignedInt();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 764 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_763.SupportedVersion(protocolVersion) || V764_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static ServerPacket PacketId => ServerPacket.UnloadChunk;

        public ServerPacket GetPacketId() => PacketId;
    }
}