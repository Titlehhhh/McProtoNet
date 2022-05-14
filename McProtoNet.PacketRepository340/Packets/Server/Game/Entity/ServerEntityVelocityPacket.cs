namespace McProtoNet.PacketRepository340.Packets.Server
{


    public sealed class ServerEntityVelocityPacket : Packet
    {
        //this.entityId = in.readVarInt();
        //this.motX = in.readShort() / 8000D;
        //this.motY = in.readShort() / 8000D;
        //this.motZ = in.readShort() / 8000D;
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityVelocityPacket() { }
    }

}
