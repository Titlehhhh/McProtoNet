using McProtoNet.Protocol340.Data;
using McProtoNet.Protocol340.Data.World;
using McProtoNet.Protocol340.Util;

namespace McProtoNet.Protocol340.Packets.Server
{


    public sealed class ServerBlockChangePacket : Packet
    {
        public BlockChangeRecord Record { get; set; }

        //this.record = new BlockChangeRecord(NetUtil.readPosition(in), NetUtil.readBlockState(in));
        public override void Read(IMinecraftPrimitiveReader stream)
        {
            Record = new BlockChangeRecord(stream.ReadPoint3_Int(), stream.ReadBlockState());
        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerBlockChangePacket() { }
    }

}
