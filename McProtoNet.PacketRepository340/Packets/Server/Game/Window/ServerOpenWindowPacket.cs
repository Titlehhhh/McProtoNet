namespace McProtoNet.PacketRepository340.Packets.Server
{


    public sealed class ServerOpenWindowPacket : Packet
    {
        //this.windowId = in.readUnsignedByte();
        //this.type = MagicValues.key(WindowType.class, in.readString());
        //this.name = in.readString();
        //this.slots = in.readUnsignedByte();
        //if(this.type == WindowType.HORSE) {
        //this.ownerEntityId = in.readInt();
        //}
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerOpenWindowPacket() { }
    }

}
