

namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x03, PacketCategory.Game, 754, PacketSide.Client)]
    public sealed class ClientChatPacket : Packet
    {
        public string Message { get; private set; }
        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteString(Message);
        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {
            Message = stream.ReadString();
        }

        public ClientChatPacket(string message)
        {
            Message = message;
        }

        public ClientChatPacket() { }
    }
}

