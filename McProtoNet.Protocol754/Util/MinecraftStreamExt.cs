﻿using McProtoNet.NBT;
using McProtoNet.Protocol754.Data;

namespace McProtoNet.Protocol754
{
    public static class MinecraftStreamExt
    {
        private const int POSITION_X_SIZE = 38;
        private const int POSITION_Y_SIZE = 12;
        private const int POSITION_Z_SIZE = 38;
        private const int POSITION_Y_SHIFT = 0xFFF;
        private const int POSITION_WRITE_SHIFT = 0x3FFFFFF;
        public static Point3_Int ReadPoint3_Int(this IMinecraftPrimitiveReader reader)
        {
            long val = reader.ReadLong();
            int x = (int)(val >> POSITION_X_SIZE);
            int y = (int)(val & POSITION_Y_SHIFT);
            int z = (int)(val << 26 >> POSITION_Z_SIZE);
            return new Point3_Int(x, y, z);
        }
        public static void WritePoint3_int(this IMinecraftPrimitiveWriter writer, Point3_Int point)
        {
            long x = point.X & POSITION_WRITE_SHIFT;
            long y = point.Y & POSITION_Y_SHIFT;
            long z = point.Z & POSITION_WRITE_SHIFT;

            writer.WriteLong(x << POSITION_X_SIZE | z << POSITION_Y_SIZE | y);
        }


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
    }
}
