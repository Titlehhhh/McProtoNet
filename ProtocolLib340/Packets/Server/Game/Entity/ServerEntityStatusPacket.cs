using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
{


    public class ServerEntityStatusPacket : IPacket
    {
        //this.entityId = in.readInt();
        //this.status = MagicValues.key(EntityStatus.class, in.readByte());
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerEntityStatusPacket() { }
    }

}
