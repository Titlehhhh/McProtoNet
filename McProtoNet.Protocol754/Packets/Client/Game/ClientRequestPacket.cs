using McProtoNet.Protocol754.Data;

namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x04, PacketCategory.Game, 754, PacketSide.Client)]
    public sealed class ClientRequestPacket : Packet
    {
        public ClientRequest Request { get; private set; }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteVarInt(Request);
        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientRequestPacket(ClientRequest request)
        {
            Request = request;
        }
        public ClientRequestPacket() { }


    }
}
