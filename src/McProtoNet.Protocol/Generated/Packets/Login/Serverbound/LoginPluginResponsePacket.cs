using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("login.toServer.login_plugin_response", PacketPhase.Login, PacketDirection.Serverbound)]
[PacketField("MessageId", "int")]
[PacketField("Data", "byte[]?")]
public sealed partial record LoginPluginResponsePacket(int MessageId, byte[]? Data) : IPacket<LoginPluginResponsePacket>
{
    public static LoginPluginResponsePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginPluginResponsePacket>(protocolVersion);
        var messageId = reader.ReadVarInt();
        byte[]? data = null;
        if (reader.ReadBoolean())
            data = reader.ReadRestBytes();
        return new LoginPluginResponsePacket(messageId, data);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginPluginResponsePacket>(protocolVersion);
        writer.WriteVarInt(MessageId);
        writer.WriteBoolean(Data is not null);
        if (Data is { } dataValue)
            writer.WriteRestBytes(dataValue);
    }

    public static PacketIdentity Identity => new("login.toServer.login_plugin_response", "LoginPluginResponse", PacketPhase.Login, PacketDirection.Serverbound, 3);

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 772)
        {
            id = 0x02;
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
