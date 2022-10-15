﻿using McProtoNet.Protocol340.Data.World.Chunk;

namespace McProtoNet.Protocol340.Util
{
    public static class MinecraftStreamExt
    {

        private static int POSITION_X_SIZE = 38;
        private static int POSITION_Y_SIZE = 26;
        private static int POSITION_Z_SIZE = 38;
        private static int POSITION_Y_SHIFT = 0xFFF;
        private static int POSITION_WRITE_SHIFT = 0x3FFFFFF;
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
        public static Block ReadBlockState(this IMinecraftPrimitiveReader reader)
        {
            int rawId = reader.ReadVarInt();
            return new Block((ushort)(rawId >> 4), (byte)(rawId & 0xF));
        }

        private static bool available(Stream stream)
        {
            return stream.Length != stream.Position;
        }
    }
}
