using McProtoNet.NBT;
using McProtoNet.Protocol340.Data.World.Chunk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McProtoNet.Protocol340.Util
{
    public static class MinecraftStreamExt
    {

        private static  int POSITION_X_SIZE = 38;
        private static  int POSITION_Y_SIZE = 26;
        private static  int POSITION_Z_SIZE = 38;
        private static  int POSITION_Y_SHIFT = 0xFFF;
        private static  int POSITION_WRITE_SHIFT = 0x3FFFFFF;
        public static Point3_Int ReadPoint3_Int(this IMinecraftPrimitiveReader reader)
        {
            long val = reader.ReadLong();

            int x = (int)(val >> POSITION_X_SIZE);
            int y = (int)((val >> POSITION_Y_SIZE) & POSITION_Y_SHIFT);
            int z = (int)((val << POSITION_Z_SIZE) >> POSITION_Z_SIZE);
            return new Point3_Int(x, y, z);
        }
        public static void WritePoint3_int(this IMinecraftPrimitiveWriter writer, Point3_Int point)
        {
            long x = point.X & POSITION_WRITE_SHIFT;
            long y = point.Y & POSITION_Y_SHIFT;
            long z = point.Z & POSITION_WRITE_SHIFT;

        writer.WriteLong(x << POSITION_X_SIZE | y << POSITION_Y_SIZE | z);
        }

        /*
        public static void WriteItem(this IMinecraftPrimitiveWriter writer, ItemStack? item)
        {
            writer.WriteBoolean(item != null);
            if (item != null)
            {
                writer.WriteVarInt(item.Id);
                writer.WriteByte(item.Amount);
                writer.WriteNbt(item.Nbt, root: true);
            }
        }

        public static ItemStack? ReadItem(this IMinecraftPrimitiveReader reader)
        {
            bool present = reader.ReadBoolean();
            if (!present)
                return null;
            int item = reader.ReadVarInt();

            var amount = reader.ReadSignedByte();
            NbtCompound? nbt = null;

            nbt = reader.ReadNbt();



            return new ItemStack(item, amount, nbt);
        }
        */
        public static BlockState ReadBlockState(this IMinecraftPrimitiveReader reader)
        {
            int rawId = reader.ReadVarInt();
            return new BlockState(rawId >> 4, rawId & 0xF);
        }

        public static Column ReadChunkColumn(this IMinecraftPrimitiveReader reader)
        {
            int x = reader.ReadInt();
            int z = reader.ReadInt();
            bool fullChunk = reader.ReadBoolean();
            int chunkMask = reader.ReadVarInt();
            byte[] data = reader.ReadByteArray();
            NbtCompound[] tileEntities = new NbtCompound[reader.ReadVarInt()];
            for (int i = 0; i < tileEntities.Length; i++)
            {
                tileEntities[i] = reader.ReadNbt();
            }
            return readColumn(data, x, z, fullChunk, false, chunkMask, tileEntities);
        }
        private static Column readColumn(byte[] data, int x, int z, bool fullChunk, bool hasSkylight, int mask, NbtCompound[] tileEntities)
        {
            MinecraftPrimitiveReader reader =
                new MinecraftPrimitiveReader(new MemoryStream(data));
            Stream ms = reader.BaseStream;

            Exception? ex = null;
            Column? column = null;
            try
            {
                Chunk[] chunks = new Chunk[16];
                for (int index = 0; index < chunks.Length; index++)
                {
                    if ((mask & (1 << index)) != 0)
                    {
                        BlockStorage blocks = new BlockStorage(reader);
                        NibbleArray3d blocklight = new NibbleArray3d(reader, 2048);
                        NibbleArray3d skylight = hasSkylight ? new NibbleArray3d(reader, 2048) : null;
                        chunks[index] = new Chunk(blocks, blocklight, skylight);
                    }
                }

                byte[]? biomeData = null;
                if (fullChunk)
                {
                    biomeData = reader.ReadByteArray(256);
                }

                column = new Column(x, z, chunks, biomeData, tileEntities);
            }
            catch (Exception e)
            {
                ex = e;
            }

            if ((available(ms) || ex != null) && !hasSkylight)
            {
                return readColumn(data, x, z, fullChunk, true, mask, tileEntities);
            }
            else if (ex != null)
            {
                throw new IOException("Failed to read chunk data.", ex);
            }

            return column;

        }

        private static bool available(Stream stream)
        {
            return stream.Length != stream.Position;
        }
    }
}
