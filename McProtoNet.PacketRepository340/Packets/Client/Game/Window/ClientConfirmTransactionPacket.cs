using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Client.Game
{


    public class ClientConfirmTransactionPacket : IPacket
    {
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        //out.writeByte(this.windowId);
        //out.writeShort(this.actionId);
        //out.WriteBooleanean(this.accepted);
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
    }
}
