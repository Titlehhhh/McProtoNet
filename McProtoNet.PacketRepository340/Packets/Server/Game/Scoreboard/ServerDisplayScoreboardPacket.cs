namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerDisplayScoreboardPacket : IPacket
    {
        //this.position = MagicValues.key(ScoreboardPosition.class, in.readByte());
        //this.name = in.readString();
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerDisplayScoreboardPacket() { }
    }

}
