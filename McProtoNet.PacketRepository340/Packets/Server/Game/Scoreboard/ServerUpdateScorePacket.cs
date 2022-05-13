namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerUpdateScorePacket : IPacket
    {
        //this.entry = in.readString();
        //this.action = MagicValues.key(ScoreboardAction.class, in.readVarInt());
        //this.objective = in.readString();
        //if(this.action == ScoreboardAction.ADD_OR_UPDATE) {
        //this.value = in.readVarInt();
        //}
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerUpdateScorePacket() { }
    }

}
