using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Serverbound;
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
public readonly partial record struct LoginAcknowledgedPacket() : IProtocolType<LoginAcknowledgedPacket>
{
    public static LoginAcknowledgedPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginAcknowledgedPacket>(protocolVersion);
        return new LoginAcknowledgedPacket();
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginAcknowledgedPacket>(protocolVersion);
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 764 && protocolVersion <= 765)
            return 0x03;
        if (protocolVersion >= 766 && protocolVersion <= 772)
            return 0x03;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
