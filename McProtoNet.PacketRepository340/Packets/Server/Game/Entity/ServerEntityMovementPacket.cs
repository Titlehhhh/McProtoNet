namespace McProtoNet.PacketRepository340.Packets.Server
{


    public sealed class ServerEntityMovementPacket : Packet
    {
        //this.entityId = in.readVarInt();
        //if(this.pos) {
        //this.moveX = in.readShort() / 4096D;
        //this.moveY = in.readShort() / 4096D;
        //this.moveZ = in.readShort() / 4096D;
        //}
        //
        //if(this.rot) {
        //this.yaw = in.readByte() * 360 / 256f;
        //this.pitch = in.readByte() * 360 / 256f;
        //}
        //
        //if(this.pos || this.rot) {
        //this.onGround = in.readBoolean();
        //}
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityMovementPacket() { }
    }

}
