﻿using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SetSlot", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class SetSlotPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 755),
        new(756, 765),
        new(766, MinecraftVersion.LatestProtocol),
    };

    public short Slot { get; set; }
    public Slot? Item { get; set; }

    public VFirst_755Fields? VFirst_755 { get; set; }
    public V756_765Fields? V756_765 { get; set; }
    public V766_LastFields? V766_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 755:
            {
                var fields = VFirst_755 ?? throw new InvalidOperationException("SetSlot VFirst_755 missing.");
                writer.WriteSignedByte(fields.WindowId);
                writer.WriteSignedShort(Slot);
                writer.WriteSlot(Item ?? throw new InvalidOperationException("SetSlot Item missing."), protocolVersion);
                return;
            }
            case >= 756 and <= 765:
            {
                var fields = V756_765 ?? throw new InvalidOperationException("SetSlot V756_765 missing.");
                writer.WriteSignedByte(fields.WindowId);
                writer.WriteVarInt(fields.StateId);
                writer.WriteSignedShort(Slot);
                writer.WriteSlot(Item ?? throw new InvalidOperationException("SetSlot Item missing."), protocolVersion);
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V766_Last ?? throw new InvalidOperationException("SetSlot V766_Last missing.");
                if (protocolVersion <= 767)
                {
                    writer.WriteUnsignedByte((byte)fields.WindowId);
                }
                else
                {
                    writer.WriteVarInt(fields.WindowId);
                }
                writer.WriteVarInt(fields.StateId);
                writer.WriteSignedShort(Slot);
                writer.WriteSlot(Item ?? throw new InvalidOperationException("SetSlot Item missing."), protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.SetSlot), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 755:
            {
                var fields = new VFirst_755Fields();
                fields.WindowId = reader.ReadSignedByte();
                Slot = reader.ReadSignedShort();
                Item = reader.ReadSlot(protocolVersion);
                VFirst_755 = fields;
                return;
            }
            case >= 756 and <= 765:
            {
                var fields = new V756_765Fields();
                fields.WindowId = reader.ReadSignedByte();
                fields.StateId = reader.ReadVarInt();
                Slot = reader.ReadSignedShort();
                Item = reader.ReadSlot(protocolVersion);
                V756_765 = fields;
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = new V766_LastFields();
                fields.WindowId = protocolVersion <= 767
                    ? reader.ReadUnsignedByte()
                    : reader.ReadVarInt();
                fields.StateId = reader.ReadVarInt();
                Slot = reader.ReadSignedShort();
                Item = reader.ReadSlot(protocolVersion);
                V766_Last = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.SetSlot), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_755Fields
    {
        public sbyte WindowId { get; set; }
    }

    public struct V756_765Fields
    {
        public sbyte WindowId { get; set; }
        public int StateId { get; set; }
    }

    public struct V766_LastFields
    {
        public int WindowId { get; set; }
        public int StateId { get; set; }
    }
}
