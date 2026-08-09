using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("login.toClient.login_plugin_request", PacketPhase.Login, PacketDirection.Clientbound)]
[PacketField("MessageId", "int")]
[PacketField("Channel", "string")]
[PacketField("Data", "byte[]")]
public sealed partial record LoginPluginRequestPacket(int MessageId, string Channel, byte[] Data) : IPacket<LoginPluginRequestPacket>
{
    public static LoginPluginRequestPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginPluginRequestPacket>(protocolVersion);
        var messageId = reader.ReadVarInt();
        var channel = reader.ReadString();
        var data = reader.ReadRestBytes();
        return new LoginPluginRequestPacket(messageId, channel, data);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginPluginRequestPacket>(protocolVersion);
        writer.WriteVarInt(MessageId);
        writer.WriteString(Channel);
        writer.WriteRestBytes(Data);
    }

    public static PacketIdentity Identity => new("login.toClient.login_plugin_request", "LoginPluginRequest", PacketPhase.Login, PacketDirection.Clientbound, 4);

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 772)
        {
            id = 0x04;
            return true;
        }

        id = 0;
        return false;
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (TryGetPacketId(protocolVersion, out var id))
            return id;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
