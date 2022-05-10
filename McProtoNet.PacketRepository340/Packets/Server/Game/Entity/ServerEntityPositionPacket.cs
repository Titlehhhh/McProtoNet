using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerEntityPositionPacket : IPacket
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
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityPositionPacket() { }
    }

}
