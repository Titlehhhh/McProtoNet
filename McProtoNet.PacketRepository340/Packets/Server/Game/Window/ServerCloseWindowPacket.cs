namespace McProtoNet.PacketRepository340.Packets.Server
{


    public sealed class ServerCloseWindowPacket : Packet
    {
        //this.windowId = in.readUnsignedByte();
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerCloseWindowPacket() { }
    }

}
