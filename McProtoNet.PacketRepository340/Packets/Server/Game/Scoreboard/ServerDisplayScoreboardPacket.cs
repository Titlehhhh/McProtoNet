using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerDisplayScoreboardPacket : IPacket
    {
        //this.position = MagicValues.key(ScoreboardPosition.class, in.readByte());
        //this.name = in.readString();
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerDisplayScoreboardPacket() { }
    }

}
