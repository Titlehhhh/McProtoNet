namespace McProtoNet.Protocol754.Packets.Client
{

    public sealed class LoginStartPacket : MinecraftPacket<Protocol754>
    {
        public string Nickname { get; private set; }
        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteString(Nickname);
        }

        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public LoginStartPacket(string nickname)
        {
            Nickname = nickname;
        }
        public LoginStartPacket()
        {

        }
    }
}
