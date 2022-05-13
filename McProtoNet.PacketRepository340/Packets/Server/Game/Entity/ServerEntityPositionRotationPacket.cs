namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerEntityPositionRotationPacket : IPacket
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
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityPositionRotationPacket() { }
    }

}
