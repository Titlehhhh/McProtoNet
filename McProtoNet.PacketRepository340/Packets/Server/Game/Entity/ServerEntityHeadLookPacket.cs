namespace McProtoNet.PacketRepository340.Packets.Server
{


    public sealed class ServerEntityHeadLookPacket : Packet
    {
        //this.entityId = in.readVarInt();
        //this.headYaw = in.readByte() * 360 / 256f;
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityHeadLookPacket() { }
    }

}
