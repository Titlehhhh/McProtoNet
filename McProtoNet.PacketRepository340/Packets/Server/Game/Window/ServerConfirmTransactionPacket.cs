namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerConfirmTransactionPacket : IPacket
    {
        //this.windowId = in.readUnsignedByte();
        //this.actionId = in.readShort();
        //this.accepted = in.readBoolean();
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerConfirmTransactionPacket() { }
    }

}
