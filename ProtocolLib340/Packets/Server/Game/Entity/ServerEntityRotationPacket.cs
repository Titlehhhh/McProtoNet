using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
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
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerEntityRotationPacket() { }
    }

}
