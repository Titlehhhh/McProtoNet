using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, 764)]
[Packet("play.toClient.resource_pack_send", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Url", "string")]
[PacketField("Hash", "string")]
[PacketField("Forced", "bool", Group = "V755_764", From = 755, To = 764)]
[PacketField("PromptMessage", "string?", Group = "V755_764", From = 755, To = 764)]
public sealed partial record ResourcePackSendPacket(string Url, string Hash, ResourcePackSendPacket.V755_764Layer? V755_764 = null) : IPacket<ResourcePackSendPacket>, IPacket
{
    public readonly record struct V755_764Layer(bool Forced, string? PromptMessage);
    public static ResourcePackSendPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ResourcePackSendPacket>(protocolVersion);
        if (protocolVersion <= 754)
        {
            var url = reader.ReadString();
            var hash = reader.ReadString();
            return new ResourcePackSendPacket(url, hash);
        }

        if (protocolVersion >= 755 && protocolVersion <= 764)
        {
            var url = reader.ReadString();
            var hash = reader.ReadString();
            var forced = reader.ReadBoolean();
            string? promptMessage = null;
            if (reader.ReadBoolean())
                promptMessage = reader.ReadString();
            return new ResourcePackSendPacket(url, hash, V755_764: new V755_764Layer(forced, promptMessage));
        }

        throw new System.NotSupportedException($"ResourcePackSendPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ResourcePackSendPacket>(protocolVersion);
        if (protocolVersion <= 754)
        {
            writer.WriteString(Url);
            writer.WriteString(Hash);
            return;
        }

        if (protocolVersion >= 755 && protocolVersion <= 764)
        {
            var layer = V755_764 ?? throw new WrongLayerException("ResourcePackSendPacket", protocolVersion, "V755_764");
            bool Forced = layer.Forced;
            string? PromptMessage = layer.PromptMessage;
            writer.WriteString(Url);
            writer.WriteString(Hash);
            writer.WriteBoolean(Forced);
            writer.WriteBoolean(PromptMessage is not null);
            if (PromptMessage is { } promptMessageValue)
                writer.WriteString(promptMessageValue);
            return;
        }

        throw new System.NotSupportedException($"ResourcePackSendPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.resource_pack_send", "ResourcePackSend", PacketPhase.Play, PacketDirection.Clientbound, 71);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x39;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x38;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x3C;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x3A;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x3D;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x3C;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x40;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x42;
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
