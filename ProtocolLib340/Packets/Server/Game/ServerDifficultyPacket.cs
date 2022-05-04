using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
{


    public class ServerDifficultyPacket : IPacket
    {
        //this.difficulty = MagicValues.key(Difficulty.class, in.readUnsignedByte());
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerDifficultyPacket() { }
    }

}
