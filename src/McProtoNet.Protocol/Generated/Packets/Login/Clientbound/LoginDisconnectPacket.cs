using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public sealed partial class LoginDisconnectPacket : IProtocolType<LoginDisconnectPacket>
{
    public string Reason { get; }

    public LoginDisconnectPacket(string reason)
    {
        Reason = reason;
    }

    public static LoginDisconnectPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginDisconnectPacket>(protocolVersion);
        var reason = reader.ReadString();
        return new LoginDisconnectPacket(reason);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginDisconnectPacket>(protocolVersion);
        writer.WriteString(Reason);
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 735 && protocolVersion <= 765)
            return 0x00;
        if (protocolVersion >= 766 && protocolVersion <= 772)
            return 0x00;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
