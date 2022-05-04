using McProtoNet.API.World;
using McProtoNet.Geometry;

namespace McProtoNet.PacketRepository340.Data.World
{

    public struct Block340 : IBlock
    {



        /// <summary>
        /// Storage for block ID and metadata, as ushort for compatibility, performance and lower memory footprint
        /// For Minecraft 1.12 and lower, first 12 bits contain block ID (0-4095), last 4 bits contain metadata (0-15)
        /// For Minecraft 1.13 and greater, all 16 bits are used to store block state ID (0-65535)
        /// </summary>
        private ushort blockIdAndMeta;

        /// <summary>
        /// Id of the block
        /// </summary>
        public int ID
        {
            get
            {
                return blockIdAndMeta >> 4;
            }
            set
            {

                if (value > (ushort.MaxValue >> 4) || value < 0)
                    throw new ArgumentOutOfRangeException("value", "Invalid block ID. Accepted range: 0-4095");
                blockIdAndMeta = (ushort)(value << 4 | BlockMeta);

            }
        }

        /// <summary>
        /// Metadata of the block.
        /// This field has no effect starting with Minecraft 1.13.
        /// </summary>
        public byte BlockMeta
        {
            get
            {
                return (byte)(blockIdAndMeta & 0x0F);
            }
            set
            {
                blockIdAndMeta = (ushort)((blockIdAndMeta & ~0x0F) | (value & 0x0F));
            }
        }

        public Point3_Int Position { get; set; }


        public Block340(ushort typeAndMeta, Point3_Int position)
        {
            this.blockIdAndMeta = typeAndMeta;
            this.Position = position;
        }

        /// <summary>
        /// String representation of the block
        /// </summary>
        public override string ToString()
        {
            return ID.ToString() + (BlockMeta != 0 ? ":" + BlockMeta.ToString() : "");
        }
    }

}
