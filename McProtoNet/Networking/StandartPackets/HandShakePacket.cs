using McProtoNet.IO;
using McProtoNet.Networking;

namespace McProtoNet
{
    public sealed class HandShakePacket : Packet
    {
        public HandShakeIntent Intent { get; set; }
        public int ProtocolVersion { get; set; }
        public ushort Port { get; set; }
        public string Host { get; set; }
        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteVarInt(ProtocolVersion);
            stream.WriteString(Host);
            stream.WriteUnsignedShort(Port);
            stream.WriteVarInt((int)Intent);
        }

        public override void Read(IMinecraftPrimitiveReader stream)
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