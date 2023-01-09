namespace McProtoNet.Protocol340.Packets.Server
{


    public sealed class ServerEntityAttachPacket : Packet<Protocol340>
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
