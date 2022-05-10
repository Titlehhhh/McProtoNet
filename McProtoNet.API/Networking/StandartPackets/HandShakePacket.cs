using McProtoNet.API.IO;
using McProtoNet.API.Networking;

namespace McProtoNet.API
{
    public sealed class HandShakePacket : IPacket
    {
        public HandShakeIntent Intent { get; set; }
        public int ProtocolVersion { get; set; }
        public ushort Port { get; set; }
        public string Host { get; set; }
        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteVarInt(ProtocolVersion);
            stream.WriteString(Host);
            stream.WriteUnsignedShort(Port);
            stream.WriteVarInt((int)Intent);
        }

        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public HandShakePacket(HandShakeIntent intent, int protocolVersion, ushort port, string host)
        {
            Intent = intent;
            ProtocolVersion = protocolVersion;
            Port = port;
            Host = host;
        }
    }
}