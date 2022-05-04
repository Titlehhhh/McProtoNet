using ProtoLib.API;
using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Client.Game
{


    public class ClientRequestPacket : IPacket
    {
        public ClientRequest Request { get; set; }
        //out.writeVarInt(MagicValues.value(Integer.class, this.request));
        public void Write(IMinecraftStreamWriter stream)
        {
            stream.WriteVarInt((int)Request);
        }

        public void Read(IMinecraftStreamReader stream)
        {

        }

        public ClientRequestPacket()
        {

        }
        public ClientRequestPacket(ClientRequest request)
        {
            Request = request;
        }
    }
}
