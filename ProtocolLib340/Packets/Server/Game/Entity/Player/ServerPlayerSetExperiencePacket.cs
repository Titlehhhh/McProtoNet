using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
{


    public class ServerPlayerSetExperiencePacket : IPacket
    {
        //this.experience = in.readFloat();
        //this.level = in.readVarInt();
        //this.totalExperience = in.readVarInt();
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerPlayerSetExperiencePacket() { }
    }

}
