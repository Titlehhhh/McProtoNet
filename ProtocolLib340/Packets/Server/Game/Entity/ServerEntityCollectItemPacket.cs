using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
{


    public class ServerEntityCollectItemPacket : IPacket
    {
        //this.collectedEntityId = in.readVarInt();
        //this.collectorEntityId = in.readVarInt();
        //this.itemCount = in.readVarInt();
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerEntityCollectItemPacket() { }
    }

}
