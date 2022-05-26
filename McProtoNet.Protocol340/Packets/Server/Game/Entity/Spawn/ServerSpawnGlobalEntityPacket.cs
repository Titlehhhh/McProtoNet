namespace McProtoNet.Protocol340.Packets.Server
{


    public sealed class ServerSpawnGlobalEntityPacket : Packet
    {
        //this.entityId = in.readVarInt();
        //this.type = MagicValues.key(GlobalEntityType.class, in.readByte());
        //this.x = in.readDouble();
        //this.y = in.readDouble();
        //this.z = in.readDouble();
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerSpawnGlobalEntityPacket() { }
    }

}
