namespace McProtoNet.PacketRepository340.Packets.Server
{


    public sealed class ServerEntityRotationPacket : Packet
    {
        //protected ServerEntityRotationPacket() {
        //this.rot = true;
        //}
        //
        //public ServerEntityRotationPacket(int entityId, float yaw, float pitch, boolean onGround) {
        //super(entityId, onGround);
        //this.rot = true;
        //this.yaw = yaw;
        //this.pitch = pitch;
        //}
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityRotationPacket() { }
    }

}
