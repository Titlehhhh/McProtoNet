using McProtoNet.Core.IO;

namespace McProtoNet.Core.Protocol
{
    public sealed class StatusResponsePacket : Packet
    {
        public string JsonResponse { get; private set; }

        public override void Read(IMinecraftPrimitiveReader stream)
        {
            JsonResponse = stream.ReadString();
        }

        public override void Write(IMinecraftPrimitiveWriter stream)
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
