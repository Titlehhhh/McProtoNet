namespace McProtoNet.Protocol340.Packets.Server
{


    public sealed class ServerJoinGamePacket : Packet
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
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerJoinGamePacket() { }
    }

}
