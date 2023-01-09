namespace McProtoNet.Protocol340.Packets.Server
{


    public sealed class ServerOpenTileEntityEditorPacket : Packet<Protocol340>
    {
        //this.position = NetUtil.readPosition(in);
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerOpenTileEntityEditorPacket() { }
    }

}
