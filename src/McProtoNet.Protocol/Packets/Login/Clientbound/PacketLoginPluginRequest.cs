using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.ClientboundPackets.Login;

public abstract class LoginPluginRequestPacket : IServerPacket
{
    public int MessageId { get; set; }
    public string Channel { get; set; }
    public byte[] Data { get; set; }


    public static PacketIdentifier PacketId => ServerLoginPacket.LoginPluginRequest;

    public PacketIdentifier GetPacketId() => PacketId;

    public sealed class V393_769 : LoginPluginRequestPacket
    {
        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            MessageId = reader.ReadVarInt();
            Channel = reader.ReadString();
            Data = reader.ReadRestBuffer();
        }

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion is >= 393 and <= 769;
        }
    }

    public static bool SupportedVersion(int protocolVersion)
    {
        return V393_769.SupportedVersion(protocolVersion);
    }

    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
}