using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
{


    public class ServerEntityAnimationPacket : IPacket
    {
        //this.entityId = in.readVarInt();
        //this.animation = MagicValues.key(Animation.class, in.readUnsignedByte());
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerEntityAnimationPacket() { }
    }

}
