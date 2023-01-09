namespace McProtoNet.Protocol340.Packets.Client.Game
{


    public sealed class ClientConfirmTransactionPacket : Packet<Protocol340>
    {
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        //out.writeByte(this.windowId);
        //out.writeShort(this.actionId);
        //out.WriteBooleanean(this.accepted);
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
    }
}
