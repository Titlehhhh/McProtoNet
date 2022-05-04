using ProtoLib.API;
using ProtoLib.API.IO;
using ProtoLib.API.Networking;
using ProtoLib.API.Protocol;


namespace ProtocolLib754.Packets.Client
{

    [PacketInfo(0x15, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientPlayerMovementPacket : IPacket
    {
        public bool OnGround { get; private set; }

        public void Write(IMinecraftStreamWriter stream)
        {
            stream.WriteBoolean(OnGround);
        }
        public void Read(IMinecraftStreamReader stream)
        {
            OnGround = stream.ReadBoolean();
        }
        public ClientPlayerMovementPacket() { }

        public ClientPlayerMovementPacket(bool OnGround)
        {
            this.OnGround = OnGround;
        }
    }
}
