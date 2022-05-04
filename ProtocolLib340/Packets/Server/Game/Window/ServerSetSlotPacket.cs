using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
{


    public class ServerSetSlotPacket : IPacket
    {
        //this.windowId = in.readUnsignedByte();
        //this.slot = in.readShort();
        //this.item = NetUtil.readItem(in);
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerSetSlotPacket() { }
    }

}
