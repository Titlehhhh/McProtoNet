namespace McProtoNet.Protocol340.Packets.Server
{


    public sealed class ServerEntityPositionPacket : Packet
    {
        //protected ServerEntityPositionPacket() {
        //this.pos = true;
        //}
        //
        //public ServerEntityPositionPacket(int entityId, double moveX, double moveY, double moveZ, boolean onGround) {
        //super(entityId, onGround);
        //this.pos = true;
        //this.moveX = moveX;
        //this.moveY = moveY;
        //this.moveZ = moveZ;
        //}
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityPositionPacket() { }
    }

}
