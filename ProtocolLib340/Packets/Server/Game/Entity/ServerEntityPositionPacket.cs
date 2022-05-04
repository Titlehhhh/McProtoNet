using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
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
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerEntityPositionPacket() { }
    }

}
