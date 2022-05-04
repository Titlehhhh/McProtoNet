using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
{


    public class ServerSwitchCameraPacket : IPacket
    {
        //this.cameraEntityId = in.readVarInt();
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerSwitchCameraPacket() { }
    }

}
