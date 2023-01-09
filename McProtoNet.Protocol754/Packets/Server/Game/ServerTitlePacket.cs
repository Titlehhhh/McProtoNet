using McProtoNet.Protocol754.Data;
using Org.BouncyCastle.Utilities;

namespace McProtoNet.Protocol754.Packets.Server
{

    
    public sealed class ServerTitlePacket : Packet<Protocol754>
    {
        public TitleAction Action { get; set; }
        public string Title { get; set; }

        public int FadeIn;
        public int Stay;
        public int FadeOut;
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {
            Action = (TitleAction)stream.ReadVarInt();
            switch (this.Action)
            {
                case TitleAction.TITLE:
                case TitleAction.SUBTITLE:
                case TitleAction.ACTION_BAR:
                    this.Title = stream.ReadString();
                    break;
                case TitleAction.TIMES:
                    this.FadeIn = stream.ReadInt();
                    this.Stay = stream.ReadInt();
                    this.FadeOut = stream.ReadInt();
                    break;
                case TitleAction.CLEAR:
                case TitleAction.RESET:
                    break;
            }
        }
        public ServerTitlePacket() { }
    }
}

