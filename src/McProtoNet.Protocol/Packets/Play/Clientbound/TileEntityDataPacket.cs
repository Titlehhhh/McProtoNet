using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("TileEntityData", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class TileEntityDataPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 756),
        new(757, 763),
        new(764, MinecraftVersion.LatestProtocol),
    };

    public Position Location { get; set; }
    public NbtTag? NbtData { get; set; }

    public VFirst_756Fields? VFirst_756 { get; set; }
    public V757_763Fields? V757_763 { get; set; }
    public V764_LastFields? V764_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 756:
            {
                var fields = VFirst_756 ?? throw new InvalidOperationException("TileEntityData VFirst_756 missing.");
                writer.WritePosition(Location, protocolVersion);
                writer.WriteUnsignedByte(fields.Action);
                writer.WriteOptionalNbtTag(NbtData, protocolVersion);
                return;
            }
            case >= 757 and <= 763:
            {
                var fields = V757_763 ?? throw new InvalidOperationException("TileEntityData V757_763 missing.");
                writer.WritePosition(Location, protocolVersion);
                writer.WriteVarInt(fields.Action);
                writer.WriteOptionalNbtTag(NbtData, protocolVersion);
                return;
            }
            case >= 764 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V764_Last ?? throw new InvalidOperationException("TileEntityData V764_Last missing.");
                writer.WritePosition(Location, protocolVersion);
                writer.WriteVarInt(fields.Action);
                writer.WriteAnonOptionalNbtTag(NbtData, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.TileEntityData), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 756:
            {
                var fields = new VFirst_756Fields();
                Location = reader.ReadPosition(protocolVersion);
                fields.Action = reader.ReadUnsignedByte();
                NbtData = reader.ReadOptionalNbtTag(protocolVersion);
                VFirst_756 = fields;
                return;
            }
            case >= 757 and <= 763:
            {
                var fields = new V757_763Fields();
                Location = reader.ReadPosition(protocolVersion);
                fields.Action = reader.ReadVarInt();
                NbtData = reader.ReadOptionalNbtTag(protocolVersion);
                V757_763 = fields;
                return;
            }
            case >= 764 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = new V764_LastFields();
                Location = reader.ReadPosition(protocolVersion);
                fields.Action = reader.ReadVarInt();
                NbtData = reader.ReadAnonOptionalNbtTag(protocolVersion);
                V764_Last = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.TileEntityData), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_756Fields
    {
        public byte Action { get; set; }
    }

    public struct V757_763Fields
    {
        public int Action { get; set; }
    }

    public struct V764_LastFields
    {
        public int Action { get; set; }
    }
}
