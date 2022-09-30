namespace McProtoNet.Protocol340.Data.World.Chunk
{
    public class NibbleArray3d
    {
        private byte[] data;

        public NibbleArray3d(int size)
        {
            this.data = new byte[size >> 1];
        }

        public NibbleArray3d(byte[] array)
        {
            this.data = array;
        }
        public NibbleArray3d(IMinecraftPrimitiveReader reader, int size)
        {
            data = reader.ReadByteArray(size);
        }

        //    public NibbleArray3d(NetInput in, int size) throws IOException
        //    {
        //     this.data = in.readBytes(size);
        // }

        //  public void write(NetOutput out) throws IOException
        // {
        //     out.writeBytes(this.data);
        // }
        public byte[] Data => this.data;

        public int this[int x,int y, int z]
        {
            get
            {
                int key = y << 8 | z << 4 | x;
                int index = key >> 1;
                int part = key & 1;
                return part == 0 ? this.data[index] & 15 : this.data[index] >> 4 & 15;
            }
            set
            {
                int key = y << 8 | z << 4 | x;
                int index = key >> 1;
                int part = key & 1;
                if (part == 0)
                {
                    this.data[index] = (byte)(this.data[index] & 240 | value & 15);
                }
                else
                {
                    this.data[index] = (byte)(this.data[index] & 15 | (value & 15) << 4);
                }
            }
        }
        


        public void Fill(int val)
        {
            for (int index = 0; index < this.data.Length << 1; index++)
            {
                int ind = index >> 1;
                int part = index & 1;
                if (part == 0)
                {
                    this.data[ind] = (byte)(this.data[ind] & 240 | val & 15);
                }
                else
                {
                    this.data[ind] = (byte)(this.data[ind] & 15 | (val & 15) << 4);
                }
            }
        }


    }
}
