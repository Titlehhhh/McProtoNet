using McProtoNet.IO;
using McProtoNet.Networking;

namespace McProtoNet
{
    public sealed class StatusResponsePacket : Packet
    {

        public override void Read(IMinecraftPrimitiveReader stream)
        {
            throw new NotImplementedException();
            string debug = stream.ReadString();
        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
    }
}
