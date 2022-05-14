namespace McProtoNet.PacketRepository340.Packets.Server
{


    public sealed class ServerSpawnExpOrbPacket : Packet
    {
        //this.entityId = in.readVarInt();
        //this.x = in.readDouble();
        //this.y = in.readDouble();
        //this.z = in.readDouble();
        //this.exp = in.readShort();
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerSpawnExpOrbPacket() { }
    }

}
