using System;
using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("UpdateStructureBlock", PacketState.Play, PacketDirection.Serverbound)]
public sealed partial class UpdateStructureBlockPacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 758),
        new(759, MinecraftVersion.LatestProtocol)
    };

    public Position Location { get; set; }
    public int Action { get; set; }
    public int Mode { get; set; }
    public string Name { get; set; } = string.Empty;
    public sbyte OffsetX { get; set; }
    public sbyte OffsetY { get; set; }
    public sbyte OffsetZ { get; set; }
    public sbyte SizeX { get; set; }
    public sbyte SizeY { get; set; }
    public sbyte SizeZ { get; set; }
    public int Mirror { get; set; }
    public int Rotation { get; set; }
    public string Metadata { get; set; } = string.Empty;
    public float Integrity { get; set; }
    public byte Flags { get; set; }

    public VFirst_758Fields? VFirst_758 { get; set; }
    public V759_LastFields? V759_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            {
                var fields = VFirst_758 ?? throw new InvalidOperationException("UpdateStructureBlock VFirst_758 fields missing.");
                writer.WritePosition(Location, protocolVersion);
                writer.WriteVarInt(Action);
                writer.WriteVarInt(Mode);
                writer.WriteString(Name);
                writer.WriteSignedByte(OffsetX);
                writer.WriteSignedByte(OffsetY);
                writer.WriteSignedByte(OffsetZ);
                writer.WriteSignedByte(SizeX);
                writer.WriteSignedByte(SizeY);
                writer.WriteSignedByte(SizeZ);
                writer.WriteVarInt(Mirror);
                writer.WriteVarInt(Rotation);
                writer.WriteString(Metadata);
                writer.WriteFloat(Integrity);
                writer.WriteVarLong(fields.Seed);
                writer.WriteUnsignedByte(Flags);
                return;
            }
            case >= 759 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V759_Last ?? throw new InvalidOperationException("UpdateStructureBlock V759_Last fields missing.");
                writer.WritePosition(Location, protocolVersion);
                writer.WriteVarInt(Action);
                writer.WriteVarInt(Mode);
                writer.WriteString(Name);
                writer.WriteSignedByte(OffsetX);
                writer.WriteSignedByte(OffsetY);
                writer.WriteSignedByte(OffsetZ);
                writer.WriteSignedByte(SizeX);
                writer.WriteSignedByte(SizeY);
                writer.WriteSignedByte(SizeZ);
                writer.WriteVarInt(Mirror);
                writer.WriteVarInt(Rotation);
                writer.WriteString(Metadata);
                writer.WriteFloat(Integrity);
                writer.WriteVarInt(fields.Seed);
                writer.WriteUnsignedByte(Flags);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.UpdateStructureBlock), protocolVersion,
                    SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
                Location = reader.ReadPosition(protocolVersion);
                Action = reader.ReadVarInt();
                Mode = reader.ReadVarInt();
                Name = reader.ReadString();
                OffsetX = reader.ReadSignedByte();
                OffsetY = reader.ReadSignedByte();
                OffsetZ = reader.ReadSignedByte();
                SizeX = reader.ReadSignedByte();
                SizeY = reader.ReadSignedByte();
                SizeZ = reader.ReadSignedByte();
                Mirror = reader.ReadVarInt();
                Rotation = reader.ReadVarInt();
                Metadata = reader.ReadString();
                Integrity = reader.ReadFloat();
                VFirst_758 = new VFirst_758Fields
                {
                    Seed = reader.ReadVarLong()
                };
                Flags = reader.ReadUnsignedByte();
                V759_Last = null;
                return;
            case >= 759 and <= MinecraftVersion.LatestProtocol:
                Location = reader.ReadPosition(protocolVersion);
                Action = reader.ReadVarInt();
                Mode = reader.ReadVarInt();
                Name = reader.ReadString();
                OffsetX = reader.ReadSignedByte();
                OffsetY = reader.ReadSignedByte();
                OffsetZ = reader.ReadSignedByte();
                SizeX = reader.ReadSignedByte();
                SizeY = reader.ReadSignedByte();
                SizeZ = reader.ReadSignedByte();
                Mirror = reader.ReadVarInt();
                Rotation = reader.ReadVarInt();
                Metadata = reader.ReadString();
                Integrity = reader.ReadFloat();
                V759_Last = new V759_LastFields
                {
                    Seed = reader.ReadVarInt()
                };
                Flags = reader.ReadUnsignedByte();
                VFirst_758 = null;
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.UpdateStructureBlock), protocolVersion,
                    SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_758Fields
    {
        public long Seed { get; set; }
    }

    public struct V759_LastFields
    {
        public int Seed { get; set; }
    }
}
