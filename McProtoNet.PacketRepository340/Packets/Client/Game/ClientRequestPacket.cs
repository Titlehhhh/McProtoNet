using McProtoNet.API;
using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Client.Game
{


    public class ClientRequestPacket : IPacket
    {
        public ClientRequest Request { get; set; }
        //out.writeVarInt(MagicValues.value(Integer.class, this.request));
        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteVarInt((int)Request);
        }

        public void Read(IMinecraftPrimitiveReader stream)
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
