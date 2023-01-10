namespace McProtoNet.Protocol754.Packets.Server
{
    public sealed class StatusResponsePacket : MinecraftPacket<Protocol754>
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
