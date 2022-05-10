using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerDifficultyPacket : IPacket
    {
        //this.difficulty = MagicValues.key(Difficulty.class, in.readUnsignedByte());
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerDifficultyPacket() { }
    }

}
