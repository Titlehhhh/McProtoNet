namespace McProtoNet.PacketRepository340.Packets.Server
{


    public sealed class ServerPluginMessagePacket : Packet
    {
        //this.channel = in.readString();
        //this.data = in.readBytes(in.available());
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerPluginMessagePacket() { }
    }

}
