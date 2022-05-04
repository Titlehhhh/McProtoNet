using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Client.Game
{


    public class ClientTeleportConfirmPacket : IPacket
    {
        public int ID { get; set; }

        public void Write(IMinecraftStreamWriter stream)
        {
            stream.WriteVarInt(ID);
        }

        public void Read(IMinecraftStreamReader stream)
        {

        }

        public ClientTeleportConfirmPacket(int iD)
        {
            ID = iD;
        }
    }
}
