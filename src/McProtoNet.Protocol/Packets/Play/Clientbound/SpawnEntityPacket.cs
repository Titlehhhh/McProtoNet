using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SpawnEntity", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class SpawnEntityPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 758),
        new(759, MinecraftVersion.LatestProtocol),
    };

    public int EntityId { get; set; }
    public Guid ObjectUUID { get; set; }
    public int Type { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public sbyte Pitch { get; set; }
    public sbyte Yaw { get; set; }
    public int ObjectData { get; set; }
    public short VelocityX { get; set; }
    public short VelocityY { get; set; }
    public short VelocityZ { get; set; }

    public V759_LastFields? V759_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
                writer.WriteVarInt(EntityId);
                writer.WriteUUID(ObjectUUID);
                writer.WriteVarInt(Type);
                writer.WriteDouble(X);
                writer.WriteDouble(Y);
                writer.WriteDouble(Z);
                writer.WriteSignedByte(Pitch);
                writer.WriteSignedByte(Yaw);
                writer.WriteSignedInt(ObjectData);
                writer.WriteSignedShort(VelocityX);
                writer.WriteSignedShort(VelocityY);
                writer.WriteSignedShort(VelocityZ);
                return;
            case >= 759 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V759_Last ?? throw new InvalidOperationException("SpawnEntity V759_Last missing.");
                writer.WriteVarInt(EntityId);
                writer.WriteUUID(ObjectUUID);
                writer.WriteVarInt(Type);
                writer.WriteDouble(X);
                writer.WriteDouble(Y);
                writer.WriteDouble(Z);
                writer.WriteSignedByte(Pitch);
                writer.WriteSignedByte(Yaw);
                writer.WriteSignedByte(fields.HeadPitch);
                writer.WriteVarInt(ObjectData);
                writer.WriteSignedShort(VelocityX);
                writer.WriteSignedShort(VelocityY);
                writer.WriteSignedShort(VelocityZ);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.SpawnEntity), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
                EntityId = reader.ReadVarInt();
                ObjectUUID = reader.ReadUUID();
                Type = reader.ReadVarInt();
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                Pitch = reader.ReadSignedByte();
                Yaw = reader.ReadSignedByte();
                ObjectData = reader.ReadSignedInt();
                VelocityX = reader.ReadSignedShort();
                VelocityY = reader.ReadSignedShort();
                VelocityZ = reader.ReadSignedShort();
                return;
            case >= 759 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = new V759_LastFields();
                EntityId = reader.ReadVarInt();
                ObjectUUID = reader.ReadUUID();
                Type = reader.ReadVarInt();
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                Pitch = reader.ReadSignedByte();
                Yaw = reader.ReadSignedByte();
                fields.HeadPitch = reader.ReadSignedByte();
                ObjectData = reader.ReadVarInt();
                VelocityX = reader.ReadSignedShort();
                VelocityY = reader.ReadSignedShort();
                VelocityZ = reader.ReadSignedShort();
                V759_Last = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.SpawnEntity), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V759_LastFields
    {
        public sbyte HeadPitch { get; set; }
    }
}
