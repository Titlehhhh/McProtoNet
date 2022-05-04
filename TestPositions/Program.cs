

using ProtoLib.API.IO;
using ProtoLib.Geometry;

namespace Gh
{
    public static class Program
    {
        private static int POSITION_X_SIZE = 38;
        private static int POSITION_Y_SIZE = 26;
        private static int POSITION_Z_SIZE = 38;
        private static int POSITION_Y_SHIFT = 0xFFF;
        private static int POSITION_WRITE_SHIFT = 0x3FFFFFF;


        public static void Main()
        {
            Console.WriteLine("start");

            Test();
        }
        static void Test()
        {
            for (int x = -33554432; x <= 33554431; x++)
                for (int y = 0; y <= 2047; y++)
                    for (int z = -33554432; z <= 33554431; z++)
                    {
                        MinecraftStream ms = new MinecraftStream();
                        Point3_Int before = new Point3_Int(x, y, z);
                        Console.WriteLine("Before: "+before);
                        ms.WritePos(before);
                        Point3_Int after = ms.ReadPos();
                        Console.WriteLine("After: " + after);
                    }
        }
        static void WritePos(this MinecraftStream stream, Point3_Int pos)
        {
            long x = pos.X & POSITION_WRITE_SHIFT;
            long y = pos.Y & POSITION_Y_SHIFT;
            long z = pos.Z & POSITION_WRITE_SHIFT;

            stream.WriteLong(x << POSITION_X_SIZE | y << POSITION_Y_SIZE | z);
        }
        static Point3_Int ReadPos(this MinecraftStream stream)
        {
            long val = stream.ReadLong();

            int x = (int)(val >> POSITION_X_SIZE);
            int y = (int)((val >> POSITION_Y_SIZE) & POSITION_Y_SHIFT);
            int z = (int)((val << POSITION_Z_SIZE) >> POSITION_Z_SIZE);


            return new Point3_Int(x, y, z);
        }
    }
}
