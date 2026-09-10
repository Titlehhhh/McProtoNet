using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Login.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("login.toClient.login_plugin_request", PacketPhase.Login, PacketDirection.Clientbound)]
[PacketField("MessageId", "int")]
[PacketField("Channel", "string")]
[PacketField("Data", "byte[]")]
public sealed partial record LoginPluginRequestPacket(int MessageId, string Channel, byte[] Data) : IPacket<LoginPluginRequestPacket>, IPacket
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

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
