namespace McProtoNet.PacketRepository340.Packets.Server
{


    public sealed class ServerPlayerPositionRotationPacket : Packet
    {
        //this.x = in.readDouble();
        //this.y = in.readDouble();
        //this.z = in.readDouble();
        //this.yaw = in.readFloat();
        //this.pitch = in.readFloat();
        //this.relative = new ArrayList<PositionElement>();
        //int flags = in.readUnsignedByte();
        //for(PositionElement element : PositionElement.values()) {
        //int bit = 1 << MagicValues.value(Integer.class, element);
        //if((flags & bit) == bit) {
        //this.relative.add(element);
        //}
        //}
        //
        //this.teleportId = in.readVarInt();
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerPlayerPositionRotationPacket() { }
    }

}
