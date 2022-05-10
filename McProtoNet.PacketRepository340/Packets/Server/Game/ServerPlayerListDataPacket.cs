using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerPlayerListDataPacket : IPacket
    {
        //this.header = Message.fromString(in.readString());
        //this.footer = Message.fromString(in.readString());
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerPlayerListDataPacket() { }
    }

}
