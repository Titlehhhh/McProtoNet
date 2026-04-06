using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("SelectBundleItem", PacketState.Play, PacketDirection.Serverbound)]
public sealed partial class SelectBundleItemPacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(768, MinecraftVersion.LatestProtocol)
    };

    public int SlotId { get; set; }
    public int SelectedItemIndex { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 768 and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(SlotId);
                writer.WriteVarInt(SelectedItemIndex);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.SelectBundleItem), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 768 and <= MinecraftVersion.LatestProtocol:
                SlotId = reader.ReadVarInt();
                SelectedItemIndex = reader.ReadVarInt();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.SelectBundleItem), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
