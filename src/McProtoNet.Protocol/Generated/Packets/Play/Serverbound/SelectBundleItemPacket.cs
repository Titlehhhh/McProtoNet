using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

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

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("SlotId");
        writer.WriteNumberValue(SlotId);
        writer.WritePropertyName("SelectedItemIndex");
        writer.WriteNumberValue(SelectedItemIndex);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toServer.select_bundle_item", "SelectBundleItem", PacketPhase.Play, PacketDirection.Serverbound, 46);

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
