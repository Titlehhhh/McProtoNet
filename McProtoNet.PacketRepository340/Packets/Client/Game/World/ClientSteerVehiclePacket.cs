using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Client.Game
{


    public class ClientSteerVehiclePacket : IPacket
    {
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        //out.writeFloat(this.sideways);
        //out.writeFloat(this.forward);
        //byte flags = 0;
        //if(this.jump) {
        //flags = (byte) (flags | 1);
        //}
        //
        //if(this.dismount) {
        //flags = (byte) (flags | 2);
        //}
        //
        //out.writeByte(flags);
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
    }
}
