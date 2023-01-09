namespace McProtoNet.Protocol340.Packets.Server
{


    public sealed class ServerPlayerChangeHeldItemPacket : Packet<Protocol340>
    {
        //this.slot = in.readByte();
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerPlayerChangeHeldItemPacket() { }
    }

}
