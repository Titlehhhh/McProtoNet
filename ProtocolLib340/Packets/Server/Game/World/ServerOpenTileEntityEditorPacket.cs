using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
{


    public class ServerOpenTileEntityEditorPacket : IPacket
    {
        //this.position = NetUtil.readPosition(in);
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerOpenTileEntityEditorPacket() { }
    }

}
