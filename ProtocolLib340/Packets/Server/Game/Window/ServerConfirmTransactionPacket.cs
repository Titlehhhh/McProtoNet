using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
{


    public class ServerConfirmTransactionPacket : IPacket
    {
        //this.windowId = in.readUnsignedByte();
        //this.actionId = in.readShort();
        //this.accepted = in.readBoolean();
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerConfirmTransactionPacket() { }
    }

}
