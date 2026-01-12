using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("Position", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class PositionPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 754),
        new(755, 761),
        new(762, 765),
        new(766, 767),
        new(768, MinecraftVersion.LatestProtocol)
    };

    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public float Yaw { get; set; }
    public float Pitch { get; set; }
    public int TeleportId { get; set; }

    public VFirst_754Fields? VFirst_754 { get; set; }
    public V755_761Fields? V755_761 { get; set; }
    public V762_765Fields? V762_765 { get; set; }
    public V766_767Fields? V766_767 { get; set; }
    public V768_LastFields? V768_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
            {
                var fields = VFirst_754 ?? throw new InvalidOperationException("Position VFirst_754 missing.");
                writer.WriteDouble(X);
                writer.WriteDouble(Y);
                writer.WriteDouble(Z);
                writer.WriteFloat(Yaw);
                writer.WriteFloat(Pitch);
                writer.WriteSignedByte(fields.Flags);
                writer.WriteVarInt(TeleportId);
                return;
            }
            case >= 755 and <= 761:
            {
                var fields = V755_761 ?? throw new InvalidOperationException("Position V755_761 missing.");
                writer.WriteDouble(X);
                writer.WriteDouble(Y);
                writer.WriteDouble(Z);
                writer.WriteFloat(Yaw);
                writer.WriteFloat(Pitch);
                writer.WriteSignedByte(fields.Flags);
                writer.WriteVarInt(TeleportId);
                writer.WriteBoolean(fields.DismountVehicle);
                return;
            }
            case >= 762 and <= 765:
            {
                var fields = V762_765 ?? throw new InvalidOperationException("Position V762_765 missing.");
                writer.WriteDouble(X);
                writer.WriteDouble(Y);
                writer.WriteDouble(Z);
                writer.WriteFloat(Yaw);
                writer.WriteFloat(Pitch);
                writer.WriteSignedByte(fields.Flags);
                writer.WriteVarInt(TeleportId);
                return;
            }
            case >= 766 and <= 767:
            {
                var fields = V766_767 ?? throw new InvalidOperationException("Position V766_767 missing.");
                writer.WriteDouble(X);
                writer.WriteDouble(Y);
                writer.WriteDouble(Z);
                writer.WriteFloat(Yaw);
                writer.WriteFloat(Pitch);
                writer.WritePositionUpdateRelatives(fields.Flags, protocolVersion);
                writer.WriteVarInt(TeleportId);
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V768_Last ?? throw new InvalidOperationException("Position V768_Last missing.");
                writer.WriteVarInt(TeleportId);
                writer.WriteDouble(X);
                writer.WriteDouble(Y);
                writer.WriteDouble(Z);
                writer.WriteDouble(fields.Dx);
                writer.WriteDouble(fields.Dy);
                writer.WriteDouble(fields.Dz);
                writer.WriteFloat(Yaw);
                writer.WriteFloat(Pitch);
                writer.WritePositionUpdateRelatives(fields.Flags, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.Position), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
            {
                var fields = new VFirst_754Fields();
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                Yaw = reader.ReadFloat();
                Pitch = reader.ReadFloat();
                fields.Flags = reader.ReadSignedByte();
                TeleportId = reader.ReadVarInt();
                VFirst_754 = fields;
                return;
            }
            case >= 755 and <= 761:
            {
                var fields = new V755_761Fields();
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                Yaw = reader.ReadFloat();
                Pitch = reader.ReadFloat();
                fields.Flags = reader.ReadSignedByte();
                TeleportId = reader.ReadVarInt();
                fields.DismountVehicle = reader.ReadBoolean();
                V755_761 = fields;
                return;
            }
            case >= 762 and <= 765:
            {
                var fields = new V762_765Fields();
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                Yaw = reader.ReadFloat();
                Pitch = reader.ReadFloat();
                fields.Flags = reader.ReadSignedByte();
                TeleportId = reader.ReadVarInt();
                V762_765 = fields;
                return;
            }
            case >= 766 and <= 767:
            {
                var fields = new V766_767Fields();
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                Yaw = reader.ReadFloat();
                Pitch = reader.ReadFloat();
                fields.Flags = reader.ReadPositionUpdateRelatives(protocolVersion);
                TeleportId = reader.ReadVarInt();
                V766_767 = fields;
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = new V768_LastFields();
                TeleportId = reader.ReadVarInt();
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                fields.Dx = reader.ReadDouble();
                fields.Dy = reader.ReadDouble();
                fields.Dz = reader.ReadDouble();
                Yaw = reader.ReadFloat();
                Pitch = reader.ReadFloat();
                fields.Flags = reader.ReadPositionUpdateRelatives(protocolVersion);
                V768_Last = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.Position), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_754Fields
    {
        public sbyte Flags { get; set; }
    }

    public struct V755_761Fields
    {
        public sbyte Flags { get; set; }
        public bool DismountVehicle { get; set; }
    }

    public struct V762_765Fields
    {
        public sbyte Flags { get; set; }
    }

    public struct V766_767Fields
    {
        public PositionUpdateRelatives Flags { get; set; }
    }

    public struct V768_LastFields
    {
        public double Dx { get; set; }
        public double Dy { get; set; }
        public double Dz { get; set; }
        public PositionUpdateRelatives Flags { get; set; }
    }
}
