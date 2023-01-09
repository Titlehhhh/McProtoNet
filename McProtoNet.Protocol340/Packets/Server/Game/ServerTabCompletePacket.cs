namespace McProtoNet.Protocol340.Packets.Server
{


    public sealed class ServerTabCompletePacket : Packet<Protocol340>
    {
        //this.matches = new String[in.readVarInt()];
        //for(int index = 0; index < this.matches.length; index++) {
        //this.matches[index] = in.readString();
        //}
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerTabCompletePacket() { }
    }

}
