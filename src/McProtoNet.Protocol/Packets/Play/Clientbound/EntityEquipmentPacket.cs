using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;
using System.Collections.Generic;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("EntityEquipment", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class EntityEquipmentPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 765),
        new(766, MinecraftVersion.LatestProtocol),
    };

    public int EntityId { get; set; }
    public EquipmentEntry[] Equipments { get; set; } = Array.Empty<EquipmentEntry>();

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(EntityId);
                for (int i = 0; i < Equipments.Length; i++)
                {
                    byte slot = (byte)Equipments[i].Slot;
                    if (i == Equipments.Length - 1)
                    {
                        slot |= 0x80;
                    }
                    writer.WriteUnsignedByte(slot);
                    writer.WriteSlot(Equipments[i].Item, protocolVersion);
                }
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.EntityEquipment), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                EntityId = reader.ReadVarInt();
                var entries = new List<EquipmentEntry>();
                while (true)
                {
                    byte slotRaw = reader.ReadUnsignedByte();
                    bool isLast = (slotRaw & 0x80) != 0;
                    sbyte slot = unchecked((sbyte)(slotRaw & 0x7F));
                    Slot item = reader.ReadSlot(protocolVersion);
                    entries.Add(new EquipmentEntry
                    {
                        Slot = slot,
                        Item = item
                    });
                    if (isLast)
                    {
                        break;
                    }
                }
                Equipments = entries.ToArray();
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.EntityEquipment), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct EquipmentEntry
    {
        public sbyte Slot { get; set; }
        public Slot Item { get; set; }
    }
}
