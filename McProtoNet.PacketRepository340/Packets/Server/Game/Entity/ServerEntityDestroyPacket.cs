using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
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
