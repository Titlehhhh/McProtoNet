using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Client.Game
{

    public class ClientCloseWindowPacket : IPacket
    {
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        //out.writeByte(this.windowId);
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
    }
}
