using System;
using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SpawnEntityLiving", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class SpawnEntityLivingPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 758)
    };

    public int EntityId { get; set; }
    public Guid EntityUUID { get; set; }
    public int Type { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public sbyte Yaw { get; set; }
    public sbyte Pitch { get; set; }
    public sbyte HeadPitch { get; set; }
    public short VelocityX { get; set; }
    public short VelocityY { get; set; }
    public short VelocityZ { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
                writer.WriteVarInt(EntityId);
                writer.WriteUUID(EntityUUID);
                writer.WriteVarInt(Type);
                writer.WriteDouble(X);
                writer.WriteDouble(Y);
                writer.WriteDouble(Z);
                writer.WriteSignedByte(Yaw);
                writer.WriteSignedByte(Pitch);
                writer.WriteSignedByte(HeadPitch);
                writer.WriteSignedShort(VelocityX);
                writer.WriteSignedShort(VelocityY);
                writer.WriteSignedShort(VelocityZ);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.SpawnEntityLiving), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
                EntityId = reader.ReadVarInt();
                EntityUUID = reader.ReadUUID();
                Type = reader.ReadVarInt();
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                Yaw = reader.ReadSignedByte();
                Pitch = reader.ReadSignedByte();
                HeadPitch = reader.ReadSignedByte();
                VelocityX = reader.ReadSignedShort();
                VelocityY = reader.ReadSignedShort();
                VelocityZ = reader.ReadSignedShort();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.SpawnEntityLiving), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
