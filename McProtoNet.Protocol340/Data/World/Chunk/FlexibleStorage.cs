namespace McProtoNet.Protocol340.Data.World.Chunk
{
    public class FlexibleStorage
    {
        private long[] data;
        private int bitsPerEntry;
        private int size;
        private long maxEntryValue;

        public FlexibleStorage(int bitsPerEntry, int size) :
            this(bitsPerEntry, new long[roundToNearest(size * bitsPerEntry, 64) / 64])
        {

        }

        public FlexibleStorage(int bitsPerEntry, long[] data)
        {
            if (bitsPerEntry < 4)
            {
                bitsPerEntry = 4;
            }

            this.bitsPerEntry = bitsPerEntry;
            this.data = data;

            this.size = this.data.Length * 64 / this.bitsPerEntry;
            this.maxEntryValue = (1L << this.bitsPerEntry) - 1;
        }

        private static int roundToNearest(int value, int roundTo)
        {
            if (roundTo == 0)
            {
                return 0;
            }
            else if (value == 0)
            {
                return roundTo;
            }
            else
            {
                if (value < 0)
                {
                    roundTo *= -1;
                }

                int remainder = value % roundTo;
                return remainder != 0 ? value + roundTo - remainder : value;
            }
        }
        public long[] Data => this.data;
        
        public int BitsPerEntry => bitsPerEntry;

        public int Size => size;
        
        public int this[int index]
        {
            get
            {
                if (index < 0 || index > this.size - 1)
                {
                    throw new ArgumentOutOfRangeException();
                }

                int bitIndex = index * this.bitsPerEntry;
                int startIndex = bitIndex / 64;
                int endIndex = ((index + 1) * this.bitsPerEntry - 1) / 64;
                int startBitSubIndex = bitIndex % 64;
                if (startIndex == endIndex)
                {
                    return (int)(this.data[startIndex] >>> startBitSubIndex & this.maxEntryValue);
                }
                else
                {
                    int endBitSubIndex = 64 - startBitSubIndex;
                    return (int)((this.data[startIndex] >>> startBitSubIndex | this.data[endIndex] << endBitSubIndex) & this.maxEntryValue);
                }
            }
            set
            {
                if (index < 0 || index > this.size - 1)
                {
                    throw new ArgumentOutOfRangeException();
                }

                if (value < 0 || value > this.maxEntryValue)
                {
                    throw new ArgumentOutOfRangeException("Value cannot be outside of accepted range.");
                }

                int bitIndex = index * this.bitsPerEntry;
                int startIndex = bitIndex / 64;
                int endIndex = ((index + 1) * this.bitsPerEntry - 1) / 64;
                int startBitSubIndex = bitIndex % 64;
                this.data[startIndex] = this.data[startIndex] & ~(this.maxEntryValue << startBitSubIndex) | ((long)value & this.maxEntryValue) << startBitSubIndex;
                if (startIndex != endIndex)
                {
                    int endBitSubIndex = 64 - startBitSubIndex;
                    this.data[endIndex] = this.data[endIndex] >>> endBitSubIndex << endBitSubIndex | ((long)value & this.maxEntryValue) >> endBitSubIndex;
                }
            }
        }

       



    }
}
