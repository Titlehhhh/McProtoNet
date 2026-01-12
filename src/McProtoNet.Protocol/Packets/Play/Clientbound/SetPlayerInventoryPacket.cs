using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SetPlayerInventory", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class SetPlayerInventoryPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(768, 768),
        new(769, MinecraftVersion.LatestProtocol)
    };

    public int SlotId { get; set; }
    public Slot? Contents { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case 768:
                writer.WriteVarInt(SlotId);
                if (Contents is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteSlot(Contents.Value, protocolVersion);
                }
                return;
            case >= 769 and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(SlotId);
                writer.WriteSlot(Contents ?? throw new InvalidOperationException("SetPlayerInventory Contents missing."), protocolVersion);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.SetPlayerInventory), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case 768:
                SlotId = reader.ReadVarInt();
                Contents = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadSlot(protocolVersion));
                return;
            case >= 769 and <= MinecraftVersion.LatestProtocol:
                SlotId = reader.ReadVarInt();
                Contents = reader.ReadSlot(protocolVersion);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.SetPlayerInventory), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
