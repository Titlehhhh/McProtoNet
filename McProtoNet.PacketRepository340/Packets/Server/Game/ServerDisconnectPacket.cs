namespace McProtoNet.PacketRepository340.Packets.Server
{


    public sealed class ServerDisconnectPacket : Packet
    {

        public string Messaage { get; private set; }
        //this.message = Message.fromString(in.readString());
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerDisconnectPacket() { }
    }

}
