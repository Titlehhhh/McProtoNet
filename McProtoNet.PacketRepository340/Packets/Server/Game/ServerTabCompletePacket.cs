using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerTabCompletePacket : IPacket
    {
        //this.matches = new String[in.readVarInt()];
        //for(int index = 0; index < this.matches.length; index++) {
        //this.matches[index] = in.readString();
        //}
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerTabCompletePacket() { }
    }

}
