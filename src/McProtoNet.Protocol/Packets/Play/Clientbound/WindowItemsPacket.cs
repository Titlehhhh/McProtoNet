using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("WindowItems", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class WindowItemsPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 755),
        new(756, 765),
        new(766, MinecraftVersion.LatestProtocol),
    };

    public Slot?[] Items { get; set; }

    public VFirst_755Fields? VFirst_755 { get; set; }
    public V756_765Fields? V756_765 { get; set; }
    public V766_LastFields? V766_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 755:
            {
                var fields = VFirst_755 ?? throw new InvalidOperationException("WindowItems VFirst_755 missing.");
                writer.WriteUnsignedByte(fields.WindowId);
                writer.WriteArray(Items ?? throw new InvalidOperationException("WindowItems Items missing."), LengthFormat.Short, protocolVersion);
                return;
            }
            case >= 756 and <= 765:
            {
                var fields = V756_765 ?? throw new InvalidOperationException("WindowItems V756_765 missing.");
                writer.WriteUnsignedByte(fields.WindowId);
                writer.WriteVarInt(fields.StateId);
                writer.WriteArray(Items ?? throw new InvalidOperationException("WindowItems Items missing."), LengthFormat.VarInt, protocolVersion);
                writer.WriteSlot(fields.CarriedItem, protocolVersion);
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V766_Last ?? throw new InvalidOperationException("WindowItems V766_Last missing.");
                if (protocolVersion <= 767)
                {
                    writer.WriteUnsignedByte((byte)fields.WindowId);
                }
                else
                {
                    writer.WriteVarInt(fields.WindowId);
                }
                writer.WriteVarInt(fields.StateId);
                writer.WriteArray(Items ?? throw new InvalidOperationException("WindowItems Items missing."), LengthFormat.VarInt, protocolVersion);
                writer.WriteSlot(fields.CarriedItem, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.WindowItems), protocolVersion, SupportedVersionsStatic);
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
                fields.WindowId = reader.ReadUnsignedByte();
                Items = reader.ReadArray<Slot>(LengthFormat.Short, protocolVersion);
                VFirst_755 = fields;
                return;
            }
            case >= 756 and <= 765:
            {
                var fields = new V756_765Fields();
                fields.WindowId = reader.ReadUnsignedByte();
                fields.StateId = reader.ReadVarInt();
                Items = reader.ReadArray<Slot>(LengthFormat.VarInt, protocolVersion);
                fields.CarriedItem = reader.ReadSlot(protocolVersion);
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
                Items = reader.ReadArray<Slot>(LengthFormat.VarInt, protocolVersion);
                fields.CarriedItem = reader.ReadSlot(protocolVersion);
                V766_Last = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.WindowItems), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_755Fields
    {
        public byte WindowId { get; set; }
    }

    public struct V756_765Fields
    {
        public byte WindowId { get; set; }
        public int StateId { get; set; }
        public Slot CarriedItem { get; set; }
    }

    public struct V766_LastFields
    {
        public int WindowId { get; set; }
        public int StateId { get; set; }
        public Slot CarriedItem { get; set; }
    }
}
