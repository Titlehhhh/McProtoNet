using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
{


    public class ServerEntityDestroyPacket : IPacket
    {
        //this.entityIds = new int[in.readVarInt()];
        //for(int index = 0; index < this.entityIds.length; index++) {
        //this.entityIds[index] = in.readVarInt();
        //}
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerEntityDestroyPacket() { }
    }

}
