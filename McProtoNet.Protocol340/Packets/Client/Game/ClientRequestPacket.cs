namespace McProtoNet.Protocol340.Packets.Client.Game
{


    public sealed class ClientRequestPacket : Packet
    {
        public ClientRequest Request { get; set; }
        //out.writeVarInt(MagicValues.value(Integer.class, this.request));
        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteVarInt((int)Request);
        }

        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public ClientRequestPacket()
        {

        }
        public ClientRequestPacket(ClientRequest request)
        {
            Request = request;
        }
    }
}
