using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerJoinGamePacket : IPacket
    {
        //this.entityId = in.readInt();
        //int gamemode = in.readUnsignedByte();
        //this.hardcore = (gamemode & 8) == 8;
        //gamemode &= -9;
        //this.gamemode = MagicValues.key(GameMode.class, gamemode);
        //this.dimension = in.readInt();
        //this.difficulty = MagicValues.key(Difficulty.class, in.readUnsignedByte());
        //this.maxPlayers = in.readUnsignedByte();
        //this.worldType = MagicValues.key(WorldType.class, in.readString().toLowerCase());
        //this.reducedDebugInfo = in.readBoolean();
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerJoinGamePacket() { }
    }

}
