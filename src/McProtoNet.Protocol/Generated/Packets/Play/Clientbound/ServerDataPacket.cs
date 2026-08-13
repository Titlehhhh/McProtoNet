using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(759, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.server_data", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("MotdJson", "string?", Group = "V759", From = 759, To = 759)]
[PacketField("Icon", "string?", Group = "V759", From = 759, To = 759)]
[PacketField("PreviewsChat", "bool", Group = "V759", From = 759, To = 759)]
[PacketField("MotdJson", "string?", Group = "V760", From = 760, To = 760)]
[PacketField("Icon", "string?", Group = "V760", From = 760, To = 760)]
[PacketField("PreviewsChat", "bool", Group = "V760", From = 760, To = 760)]
[PacketField("EnforcesSecureChat", "bool", Group = "V760", From = 760, To = 760)]
[PacketField("MotdJson", "string?", Group = "V761", From = 761, To = 761)]
[PacketField("Icon", "string?", Group = "V761", From = 761, To = 761)]
[PacketField("EnforcesSecureChat", "bool", Group = "V761", From = 761, To = 761)]
[PacketField("MotdJson", "string", Group = "V762_764", From = 762, To = 764)]
[PacketField("IconBytes", "byte[]?", Group = "V762_764", From = 762, To = 764)]
[PacketField("EnforcesSecureChat", "bool", Group = "V762_764", From = 762, To = 764)]
[PacketField("Motd", "NbtTag", Group = "V765", From = 765, To = 765)]
[PacketField("IconBytes", "byte[]?", Group = "V765", From = 765, To = 765)]
[PacketField("EnforcesSecureChat", "bool", Group = "V765", From = 765, To = 765)]
[PacketField("Motd", "NbtTag", Group = "V766_Last", From = 766)]
[PacketField("IconBytes", "byte[]?", Group = "V766_Last", From = 766)]
public sealed partial record ServerDataPacket(ServerDataPacket.V759Layer? V759 = null, ServerDataPacket.V760Layer? V760 = null, ServerDataPacket.V761Layer? V761 = null, ServerDataPacket.V762_764Layer? V762_764 = null, ServerDataPacket.V765Layer? V765 = null, ServerDataPacket.V766_LastLayer? V766_Last = null) : IPacket<ServerDataPacket>, IPacket
{
    public readonly record struct V759Layer(string? MotdJson, string? Icon, bool PreviewsChat);
    public readonly record struct V760Layer(string? MotdJson, string? Icon, bool PreviewsChat, bool EnforcesSecureChat);
    public readonly record struct V761Layer(string? MotdJson, string? Icon, bool EnforcesSecureChat);
    public readonly record struct V762_764Layer(string MotdJson, byte[]? IconBytes, bool EnforcesSecureChat);
    public readonly record struct V765Layer(NbtTag Motd, byte[]? IconBytes, bool EnforcesSecureChat);
    public readonly record struct V766_LastLayer(NbtTag Motd, byte[]? IconBytes);
    public static ServerDataPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ServerDataPacket>(protocolVersion);
        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            string? motdJson = null;
            if (reader.ReadBoolean())
                motdJson = reader.ReadString();
            string? icon = null;
            if (reader.ReadBoolean())
                icon = reader.ReadString();
            var previewsChat = reader.ReadBoolean();
            return new ServerDataPacket(V759: new V759Layer(motdJson, icon, previewsChat));
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            string? motdJson = null;
            if (reader.ReadBoolean())
                motdJson = reader.ReadString();
            string? icon = null;
            if (reader.ReadBoolean())
                icon = reader.ReadString();
            var previewsChat = reader.ReadBoolean();
            var enforcesSecureChat = reader.ReadBoolean();
            return new ServerDataPacket(V760: new V760Layer(motdJson, icon, previewsChat, enforcesSecureChat));
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            string? motdJson = null;
            if (reader.ReadBoolean())
                motdJson = reader.ReadString();
            string? icon = null;
            if (reader.ReadBoolean())
                icon = reader.ReadString();
            var enforcesSecureChat = reader.ReadBoolean();
            return new ServerDataPacket(V761: new V761Layer(motdJson, icon, enforcesSecureChat));
        }

        if (protocolVersion >= 762 && protocolVersion <= 764)
        {
            var motdJson = reader.ReadString();
            byte[]? iconBytes = null;
            if (reader.ReadBoolean())
                iconBytes = reader.ReadByteArray();
            var enforcesSecureChat = reader.ReadBoolean();
            return new ServerDataPacket(V762_764: new V762_764Layer(motdJson, iconBytes, enforcesSecureChat));
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            var motd = reader.ReadNbtTag(false)!;
            byte[]? iconBytes = null;
            if (reader.ReadBoolean())
                iconBytes = reader.ReadByteArray();
            var enforcesSecureChat = reader.ReadBoolean();
            return new ServerDataPacket(V765: new V765Layer(motd, iconBytes, enforcesSecureChat));
        }

        if (protocolVersion >= 766)
        {
            var motd = reader.ReadNbtTag(false)!;
            byte[]? iconBytes = null;
            if (reader.ReadBoolean())
                iconBytes = reader.ReadByteArray();
            return new ServerDataPacket(V766_Last: new V766_LastLayer(motd, iconBytes));
        }

        throw new System.NotSupportedException($"ServerDataPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ServerDataPacket>(protocolVersion);
        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            var layer = V759 ?? throw new WrongLayerException("ServerDataPacket", protocolVersion, "V759");
            string? MotdJson = layer.MotdJson;
            string? Icon = layer.Icon;
            bool PreviewsChat = layer.PreviewsChat;
            writer.WriteBoolean(MotdJson is not null);
            if (MotdJson is { } motdJsonValue)
                writer.WriteString(motdJsonValue);
            writer.WriteBoolean(Icon is not null);
            if (Icon is { } iconValue)
                writer.WriteString(iconValue);
            writer.WriteBoolean(PreviewsChat);
            return;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            var layer = V760 ?? throw new WrongLayerException("ServerDataPacket", protocolVersion, "V760");
            string? MotdJson = layer.MotdJson;
            string? Icon = layer.Icon;
            bool PreviewsChat = layer.PreviewsChat;
            bool EnforcesSecureChat = layer.EnforcesSecureChat;
            writer.WriteBoolean(MotdJson is not null);
            if (MotdJson is { } motdJsonValue)
                writer.WriteString(motdJsonValue);
            writer.WriteBoolean(Icon is not null);
            if (Icon is { } iconValue)
                writer.WriteString(iconValue);
            writer.WriteBoolean(PreviewsChat);
            writer.WriteBoolean(EnforcesSecureChat);
            return;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            var layer = V761 ?? throw new WrongLayerException("ServerDataPacket", protocolVersion, "V761");
            string? MotdJson = layer.MotdJson;
            string? Icon = layer.Icon;
            bool EnforcesSecureChat = layer.EnforcesSecureChat;
            writer.WriteBoolean(MotdJson is not null);
            if (MotdJson is { } motdJsonValue)
                writer.WriteString(motdJsonValue);
            writer.WriteBoolean(Icon is not null);
            if (Icon is { } iconValue)
                writer.WriteString(iconValue);
            writer.WriteBoolean(EnforcesSecureChat);
            return;
        }

        if (protocolVersion >= 762 && protocolVersion <= 764)
        {
            var layer = V762_764 ?? throw new WrongLayerException("ServerDataPacket", protocolVersion, "V762_764");
            string? MotdJson = layer.MotdJson;
            byte[]? IconBytes = layer.IconBytes;
            bool EnforcesSecureChat = layer.EnforcesSecureChat;
            writer.WriteString((MotdJson ?? throw new System.InvalidOperationException("MotdJson is required at this protocol version.")));
            writer.WriteBoolean(IconBytes is not null);
            if (IconBytes is { } iconBytesValue)
                writer.WriteByteArray(iconBytesValue);
            writer.WriteBoolean(EnforcesSecureChat);
            return;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            var layer = V765 ?? throw new WrongLayerException("ServerDataPacket", protocolVersion, "V765");
            NbtTag Motd = layer.Motd;
            byte[]? IconBytes = layer.IconBytes;
            bool EnforcesSecureChat = layer.EnforcesSecureChat;
            writer.WriteNbt(Motd);
            writer.WriteBoolean(IconBytes is not null);
            if (IconBytes is { } iconBytesValue)
                writer.WriteByteArray(iconBytesValue);
            writer.WriteBoolean(EnforcesSecureChat);
            return;
        }

        if (protocolVersion >= 766)
        {
            var layer = V766_Last ?? throw new WrongLayerException("ServerDataPacket", protocolVersion, "V766_Last");
            NbtTag Motd = layer.Motd;
            byte[]? IconBytes = layer.IconBytes;
            writer.WriteNbt(Motd);
            writer.WriteBoolean(IconBytes is not null);
            if (IconBytes is { } iconBytesValue)
                writer.WriteByteArray(iconBytesValue);
            return;
        }

        throw new System.NotSupportedException($"ServerDataPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.server_data", "ServerData", PacketPhase.Play, PacketDirection.Clientbound, 75);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x3F;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x42;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x41;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x45;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x47;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x49;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x4B;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x50;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x4F;
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
