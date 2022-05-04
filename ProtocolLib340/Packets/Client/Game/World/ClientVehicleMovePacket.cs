using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Client.Game
{


    public class ClientVehicleMovePacket : IPacket
    {
        public void Read(IMinecraftStreamReader stream)
        {

        }

        //out.writeDouble(this.x);
        //out.writeDouble(this.y);
        //out.writeDouble(this.z);
        //out.writeFloat(this.yaw);
        //out.writeFloat(this.pitch);
        public void Write(IMinecraftStreamWriter stream)
        {

        }
    }
}
