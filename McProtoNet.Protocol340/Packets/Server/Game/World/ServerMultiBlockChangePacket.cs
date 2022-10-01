using McProtoNet.Protocol340.Data;
using McProtoNet.Protocol340.Data.World.Chunk;
using McProtoNet.Protocol340.Util;

namespace McProtoNet.Protocol340.Packets.Server
{


    public sealed class ServerMultiBlockChangePacket : Packet
    {
        public BlockChangeRecord[] Records { get; private set; }

        //int chunkX = in.readInt();
        //int chunkZ = in.readInt();
        //this.records = new BlockChangeRecord[in.readVarInt()];
        //for(int index = 0; index < this.records.length; index++) {
        //short pos = in.readShort();
        //BlockState block = NetUtil.readBlockState(in);
        //int x = (chunkX << 4) + (pos >> 12 & 15);
        //int y = pos & 255;
        //int z = (chunkZ << 4) + (pos >> 8 & 15);
        //this.records[index] = new BlockChangeRecord(new Position(x, y, z), block);
        //}
        public override void Read(IMinecraftPrimitiveReader stream)
        {
            int chunkX = stream.ReadInt();
            int chunkZ = stream.ReadInt();
            Records = new BlockChangeRecord[stream.ReadVarInt()];
            for (int index = 0; index < this.Records.Length; index++)
            {
                short pos = stream.ReadShort();
                BlockState block = stream.ReadBlockState();
                int x = (chunkX << 4) + (pos >> 12 & 15);
                int y = pos & 255;
                int z = (chunkZ << 4) + (pos >> 8 & 15);
                Records[index] = new BlockChangeRecord(new Point3_Int(x, y, z), block);
            }
        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerMultiBlockChangePacket() { }
    }

}
