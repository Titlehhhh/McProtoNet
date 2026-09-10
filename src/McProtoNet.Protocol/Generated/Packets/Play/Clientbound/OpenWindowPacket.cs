using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
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

    public static PacketIdentity Identity => new("play.toClient.open_window", "OpenWindow", PacketPhase.Play, PacketDirection.Clientbound, 67);

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
