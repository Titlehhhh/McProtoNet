namespace McProtoNet.PacketRepository340.Packets.Server
{


    public sealed class ServerEntityMetadataPacket : Packet
    {
        //this.entityId = in.readVarInt();
        //this.metadata = NetUtil.readEntityMetadata(in);
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityMetadataPacket() { }
    }

}
