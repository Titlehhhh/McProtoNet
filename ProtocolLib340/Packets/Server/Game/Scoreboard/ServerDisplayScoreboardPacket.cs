using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
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
