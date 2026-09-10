using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

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

    public static PacketIdentity Identity => new("play.toClient.resource_pack_send", "ResourcePackSend", PacketPhase.Play, PacketDirection.Clientbound, 82);

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
