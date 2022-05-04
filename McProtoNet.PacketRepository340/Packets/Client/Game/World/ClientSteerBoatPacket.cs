using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Client.Game
{


    public class ClientSteerBoatPacket : IPacket
    {
        public void Read(IMinecraftStreamReader stream)
        {

        }

        //out.WriteBooleanean(this.rightPaddleTurning);
        //out.WriteBooleanean(this.leftPaddleTurning);
        public void Write(IMinecraftStreamWriter stream)
        {

        }
    }
}
