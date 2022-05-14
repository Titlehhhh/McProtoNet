namespace McProtoNet.PacketRepository340.Packets.Server
{


    public sealed class ServerEntityAttachPacket : Packet
    {
        //this.entityId = in.readInt();
        //this.attachedToId = in.readInt();
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityAttachPacket() { }
    }

}
