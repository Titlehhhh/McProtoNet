using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerPlayerSetExperiencePacket : IPacket
    {
        //this.experience = in.readFloat();
        //this.level = in.readVarInt();
        //this.totalExperience = in.readVarInt();
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerPlayerSetExperiencePacket() { }
    }

}
