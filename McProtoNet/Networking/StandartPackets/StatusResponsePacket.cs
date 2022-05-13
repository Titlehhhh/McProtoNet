using McProtoNet.IO;
using McProtoNet.Networking;

namespace McProtoNet
{
    public sealed class StatusResponsePacket : IPacket
    {

        public void Read(IMinecraftPrimitiveReader stream)
        {
            throw new NotImplementedException();
            string debug = stream.ReadString();
        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
    }
}
