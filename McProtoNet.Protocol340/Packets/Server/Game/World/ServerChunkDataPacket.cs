using McProtoNet.Protocol340.Data.World.Chunk;

namespace McProtoNet.Protocol340.Packets.Server
{


    public sealed class ServerChunkDataPacket : Packet
    {
        public int CurrentDimension { get; set; }

        public int X { get; private set; }
        public int Z { get; private set; }
        public ChunkColumn Column { get; private set; }

        public override void Read(IMinecraftPrimitiveReader stream)
        {
            Column = new ChunkColumn(16);
            X = stream.ReadInt();
            Z = stream.ReadInt();


            bool full = stream.ReadBoolean();
            int mask = stream.ReadVarInt();
            int a = stream.ReadVarInt();
            //  byte[] blockData = stream.ReadByteArray();



            for (int chunkY = 0; chunkY < 16; chunkY++)
            {
                if ((mask & (1 << chunkY)) == 0)
                    continue;

                byte bitsPerBlock = stream.ReadUnsignedByte();
                bool usePalette = bitsPerBlock <= 8;

                if (bitsPerBlock < 4)
                    bitsPerBlock = 4;

                int paletteLength = stream.ReadVarInt();

                int[] palette = new int[paletteLength];

                for (int i = 0; i < paletteLength; i++)
                {
                    palette[i] = stream.ReadVarInt();

                }
                uint valueMask = (uint)((1 << bitsPerBlock) - 1);

                ulong[] dataArray = stream.ReadULongArray();

                Chunk chunk = new Chunk(16);

                if (dataArray.Length > 0)
                {
                    int longIndex = 0;
                    int startOffset = -bitsPerBlock;

                    for (int y = 0; y < 16; y++)
                    {
                        for (int z = 0; z < 16; z++)
                        {
                            for (int x = 0; x < 16; x++)
                            {
                                int num3 = (y * 16 + z) * 16 + x;
                                int num4 = num3 * bitsPerBlock / 64;
                                int num5 = num3 * bitsPerBlock % 64;
                                int num6 = ((num3 + 1) * bitsPerBlock - 1) / 64;
                                ushort num7;
                                if (num4 == num6)
                                {
                                    num7 = (ushort)((dataArray[num4] >> num5) & valueMask);
                                }
                                else
                                {
                                    int num8 = 64 - num5;
                                    num7 = (ushort)(((dataArray[num4] >> num5) | (dataArray[num6] << num8)) & valueMask);
                                }
                                if (usePalette)
                                {
                                    num7 = (ushort)palette[num7];
                                }
                                // blockId = (ushort)palette[num7];


                                chunk[x, y, z] = new Block(num7);

                            }
                        }
                    }
                }

                Column[chunkY] = chunk;

                stream.ReadByteArray(16 * 16 * 16 / 2);
                if (CurrentDimension == 0)
                    stream.ReadByteArray(16 * 16 * 16 / 2);

            }
        }



        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }



        public ServerChunkDataPacket() { }
    }

}
