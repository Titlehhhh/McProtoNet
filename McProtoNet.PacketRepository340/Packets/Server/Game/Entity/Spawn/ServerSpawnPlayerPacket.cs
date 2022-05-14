namespace McProtoNet.PacketRepository340.Packets.Server
{


    public sealed class ServerSpawnPlayerPacket : Packet
    {
        //this.entityId = in.readVarInt();
        //this.uuid = in.readUUID();
        //this.x = in.readDouble();
        //this.y = in.readDouble();
        //this.z = in.readDouble();
        //this.yaw = in.readByte() * 360 / 256f;
        //this.pitch = in.readByte() * 360 / 256f;
        //this.metadata = NetUtil.readEntityMetadata(in);
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerSpawnPlayerPacket() { }
    }

}
