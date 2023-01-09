using McProtoNet.Core.IO;
using McProtoNet.Core.Protocol;

namespace McProtoNet.Core.Packets.DefaultPackets.Server
{
    public sealed class StatusResponsePacket : IMinecraftPacket
    {
        public string JsonResponse { get; private set; }

        public void Read(IMinecraftPrimitiveReader stream)
        {
            JsonResponse = stream.ReadString();
        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteString(JsonResponse);
        }
        public StatusResponsePacket()
        {

        }

        public StatusResponsePacket(string jsonResponse)
        {
            JsonResponse = jsonResponse;
        }
    }
}
