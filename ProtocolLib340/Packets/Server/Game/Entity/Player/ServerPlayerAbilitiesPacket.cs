using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
{


    public class ServerPlayerAbilitiesPacket : IPacket
    {
        //byte flags = in.readByte();
        //this.invincible = (flags & 1) > 0;
        //this.canFly = (flags & 2) > 0;
        //this.flying = (flags & 4) > 0;
        //this.creative = (flags & 8) > 0;
        //this.flySpeed = in.readFloat();
        //this.walkSpeed = in.readFloat();
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerPlayerAbilitiesPacket() { }
    }

}
