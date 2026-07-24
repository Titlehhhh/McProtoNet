using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public sealed partial class LoginPluginResponsePacket : IProtocolType<LoginPluginResponsePacket>
{
    public int MessageId { get; }
    public byte[]? Data { get; }

    public LoginPluginResponsePacket(int messageId, byte[]? data)
    {
        MessageId = messageId;
        Data = data;
    }

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

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 735 && protocolVersion <= 763)
            return 0x02;
        if (protocolVersion >= 764 && protocolVersion <= 765)
            return 0x02;
        if (protocolVersion >= 766 && protocolVersion <= 772)
            return 0x02;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
