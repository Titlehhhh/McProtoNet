using McProtoNet.NBT;

namespace McProtoNet.Protocol754.Packets.Server
{


    public sealed class ServerChunkDataPacket : MinecraftPacket
    {
        public int ChunkX { get; set; }
        public int ChunkZ { get; set; }
        public bool FullChunk { get; set; }
        public int PrimaryBitMask { get; set; }
        public NbtCompound Heightmaps { get; set; }
        public int[]? Biomes { get; set; }
        public byte[] Data { get; set; }

        public NbtList BlockEntities { get; set; }
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {
            ChunkX = stream.ReadInt();
            ChunkZ = stream.ReadInt();
            FullChunk = stream.ReadBoolean();
            PrimaryBitMask = stream.ReadVarInt();
            Heightmaps = stream.ReadNbt();
            if (stream.ReadBoolean())
            {
                Biomes = new int[stream.ReadVarInt()];
                for (int i = 0; i < Biomes.Length; i++)
                {
                    Biomes[i] = stream.ReadVarInt();
                }
            }
            Data = stream.ReadByteArray();
            // BlockEntities = new NbtList(,);
        }

        public ServerChunkDataPacket() { }
    }
}

