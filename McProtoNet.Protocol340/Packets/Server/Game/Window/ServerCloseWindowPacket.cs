namespace McProtoNet.Protocol340.Packets.Server
{


    public sealed class ServerCloseWindowPacket : MinecraftPacket<Protocol340>
    {
         

        //this.windowId = in.readUnsignedByte();
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerCloseWindowPacket() { }
    }

}
