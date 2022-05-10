using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerBlockChangePacket : IPacket
    {
        //this.record = new BlockChangeRecord(NetUtil.readPosition(in), NetUtil.readBlockState(in));
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerBlockChangePacket() { }
    }

}
