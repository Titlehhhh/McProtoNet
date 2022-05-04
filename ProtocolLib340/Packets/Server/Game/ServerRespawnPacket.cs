using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
{


    public class ServerRespawnPacket : IPacket
    {
        //this.dimension = in.readInt();
        //this.difficulty = MagicValues.key(Difficulty.class, in.readUnsignedByte());
        //this.gamemode = MagicValues.key(GameMode.class, in.readUnsignedByte());
        //this.worldType = MagicValues.key(WorldType.class, in.readString().toLowerCase());
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerRespawnPacket() { }
    }

}
