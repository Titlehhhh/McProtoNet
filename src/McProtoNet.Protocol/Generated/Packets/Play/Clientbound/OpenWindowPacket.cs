using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.open_window", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("WindowId", "int")]
[PacketField("InventoryType", "int")]
[PacketField("WindowTitleJson", "string", Group = "VUntil764", To = 764)]
[PacketField("WindowTitle", "NbtTag", Group = "V765_Last", From = 765)]
public sealed partial record OpenWindowPacket(int WindowId, int InventoryType, OpenWindowPacket.VUntil764Layer? VUntil764 = null, OpenWindowPacket.V765_LastLayer? V765_Last = null) : IPacket<OpenWindowPacket>, IPacket
{
    public readonly record struct VUntil764Layer(string WindowTitleJson);
    public readonly record struct V765_LastLayer(NbtTag WindowTitle);
    public static OpenWindowPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<OpenWindowPacket>(protocolVersion);
        if (protocolVersion <= 764)
        {
            var windowId = reader.ReadVarInt();
            var inventoryType = reader.ReadVarInt();
            var windowTitleJson = reader.ReadString();
            return new OpenWindowPacket(windowId, inventoryType, VUntil764: new VUntil764Layer(windowTitleJson));
        }

        if (protocolVersion >= 765)
        {
            var windowId = reader.ReadVarInt();
            var inventoryType = reader.ReadVarInt();
            var windowTitle = reader.ReadNbtTag(false)!;
            return new OpenWindowPacket(windowId, inventoryType, V765_Last: new V765_LastLayer(windowTitle));
        }

        throw new System.NotSupportedException($"OpenWindowPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<OpenWindowPacket>(protocolVersion);
        if (protocolVersion <= 764)
        {
            var layer = VUntil764 ?? throw new WrongLayerException("OpenWindowPacket", protocolVersion, "VUntil764");
            string WindowTitleJson = layer.WindowTitleJson;
            writer.WriteVarInt(WindowId);
            writer.WriteVarInt(InventoryType);
            writer.WriteString(WindowTitleJson);
            return;
        }

        if (protocolVersion >= 765)
        {
            var layer = V765_Last ?? throw new WrongLayerException("OpenWindowPacket", protocolVersion, "V765_Last");
            NbtTag WindowTitle = layer.WindowTitle;
            writer.WriteVarInt(WindowId);
            writer.WriteVarInt(InventoryType);
            writer.WriteNbt(WindowTitle);
            return;
        }

        throw new System.NotSupportedException($"OpenWindowPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.open_window", "OpenWindow", PacketPhase.Play, PacketDirection.Clientbound, 63);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x2E;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x2D;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x2E;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x2B;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x2D;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x2C;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x30;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x31;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x33;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x35;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x34;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x39;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x3B;
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
