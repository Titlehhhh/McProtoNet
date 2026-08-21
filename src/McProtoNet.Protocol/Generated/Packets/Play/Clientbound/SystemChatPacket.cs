using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(759, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.system_chat", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("ContentJson", "string", Group = "V759", From = 759, To = 759)]
[PacketField("Type", "int", Group = "V759", From = 759, To = 759)]
[PacketField("ContentJson", "string", Group = "V760_764", From = 760, To = 764)]
[PacketField("IsActionBar", "bool", Group = "V760_764", From = 760, To = 764)]
[PacketField("Content", "NbtTag", Group = "V765_Last", From = 765)]
[PacketField("IsActionBar", "bool", Group = "V765_Last", From = 765)]
public sealed partial record SystemChatPacket(SystemChatPacket.V759Layer? V759 = null, SystemChatPacket.V760_764Layer? V760_764 = null, SystemChatPacket.V765_LastLayer? V765_Last = null) : IPacket<SystemChatPacket>, IPacket
{
    public readonly record struct V759Layer(string ContentJson, int Type);
    public readonly record struct V760_764Layer(string ContentJson, bool IsActionBar);
    public readonly record struct V765_LastLayer(NbtTag Content, bool IsActionBar);
    public static SystemChatPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SystemChatPacket>(protocolVersion);
        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            var contentJson = reader.ReadString();
            var type = reader.ReadVarInt();
            return new SystemChatPacket(V759: new V759Layer(contentJson, type));
        }

        if (protocolVersion >= 760 && protocolVersion <= 764)
        {
            var contentJson = reader.ReadString();
            var isActionBar = reader.ReadBoolean();
            return new SystemChatPacket(V760_764: new V760_764Layer(contentJson, isActionBar));
        }

        if (protocolVersion >= 765)
        {
            var content = reader.ReadNbtTag(false)!;
            var isActionBar = reader.ReadBoolean();
            return new SystemChatPacket(V765_Last: new V765_LastLayer(content, isActionBar));
        }

        throw new System.NotSupportedException($"SystemChatPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SystemChatPacket>(protocolVersion);
        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            var layer = V759 ?? throw new WrongLayerException("SystemChatPacket", protocolVersion, "V759");
            string ContentJson = layer.ContentJson;
            int Type = layer.Type;
            writer.WriteString(ContentJson);
            writer.WriteVarInt(Type);
            return;
        }

        if (protocolVersion >= 760 && protocolVersion <= 764)
        {
            var layer = V760_764 ?? throw new WrongLayerException("SystemChatPacket", protocolVersion, "V760_764");
            string ContentJson = layer.ContentJson;
            bool IsActionBar = layer.IsActionBar;
            writer.WriteString(ContentJson);
            writer.WriteBoolean(IsActionBar);
            return;
        }

        if (protocolVersion >= 765)
        {
            var layer = V765_Last ?? throw new WrongLayerException("SystemChatPacket", protocolVersion, "V765_Last");
            NbtTag Content = layer.Content;
            bool IsActionBar = layer.IsActionBar;
            writer.WriteNbt(Content);
            writer.WriteBoolean(IsActionBar);
            return;
        }

        throw new System.NotSupportedException($"SystemChatPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.system_chat", "SystemChat", PacketPhase.Play, PacketDirection.Clientbound, 99);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x5F;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x62;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x60;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x64;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x67;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x69;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x6C;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x73;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x72;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x77;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x79;
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
