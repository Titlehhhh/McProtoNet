using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.select_bundle_item", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("SlotId", "int")]
[PacketField("SelectedItemIndex", "int")]
public sealed partial record SelectBundleItemPacket(int SlotId, int SelectedItemIndex) : IPacket<SelectBundleItemPacket>, IPacket
{
    public static SelectBundleItemPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SelectBundleItemPacket>(protocolVersion);
        var slotId = reader.ReadVarInt();
        var selectedItemIndex = reader.ReadVarInt();
        return new SelectBundleItemPacket(slotId, selectedItemIndex);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SelectBundleItemPacket>(protocolVersion);
        writer.WriteVarInt(SlotId);
        writer.WriteVarInt(SelectedItemIndex);
    }

    public static PacketIdentity Identity => new("play.toServer.select_bundle_item", "SelectBundleItem", PacketPhase.Play, PacketDirection.Serverbound, 44);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 768 && protocolVersion <= 774)
        {
            id = 0x02;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x03;
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
