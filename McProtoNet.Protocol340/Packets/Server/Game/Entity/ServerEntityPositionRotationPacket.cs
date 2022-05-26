namespace McProtoNet.Protocol340.Packets.Server
{


    public sealed class ServerEntityPositionRotationPacket : Packet
    {
        //protected ServerEntityPositionRotationPacket() {
        //this.pos = true;
        //this.rot = true;
        //}
        //
        //public ServerEntityPositionRotationPacket(int entityId, double moveX, double moveY, double moveZ, float yaw, float pitch, boolean onGround) {
        //super(entityId, onGround);
        //this.pos = true;
        //this.rot = true;
        //this.moveX = moveX;
        //this.moveY = moveY;
        //this.moveZ = moveZ;
        //this.yaw = yaw;
        //this.pitch = pitch;
        //}
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityPositionRotationPacket() { }
    }

}
