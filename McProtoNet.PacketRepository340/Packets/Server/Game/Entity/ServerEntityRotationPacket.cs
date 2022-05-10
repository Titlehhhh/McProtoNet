using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerEntityRotationPacket : IPacket
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
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityRotationPacket() { }
    }

}
