using McProtoNet.API.IO;
using McProtoNet.API.Networking;

namespace McProtoNet.API
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
