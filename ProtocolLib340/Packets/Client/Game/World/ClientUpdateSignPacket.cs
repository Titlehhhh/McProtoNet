using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Client.Game
{


    public class ClientUpdateSignPacket : IPacket
    {
        public void Read(IMinecraftStreamReader stream)
        {

        }

        //NetUtil.writePosition(out, this.position);
        //for(String line : this.lines) {
        //out.writeString(line);
        //}
        public void Write(IMinecraftStreamWriter stream)
        {

        }
    }
}
