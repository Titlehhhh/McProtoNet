using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SetProjectilePower", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class SetProjectilePowerPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(766, 766),
        new(767, MinecraftVersion.LatestProtocol),
    };

    public int Id { get; set; }
    public Vec3f64 Power { get; set; }

    public V767_LastFields? V767_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 766 and <= 766:
            {
                writer.WriteVarInt(Id);
                writer.WriteVec3f64(Power, protocolVersion);
                return;
            }
            case >= 767 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V767_Last ?? throw new InvalidOperationException("SetProjectilePower V767_Last fields missing.");
                writer.WriteVarInt(Id);
                writer.WriteDouble(fields.AccelerationPower);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.SetProjectilePower), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 766 and <= 766:
            {
                Id = reader.ReadVarInt();
                Power = reader.ReadVec3f64(protocolVersion);
                return;
            }
            case >= 767 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = new V767_LastFields();
                Id = reader.ReadVarInt();
                fields.AccelerationPower = reader.ReadDouble();
                V767_Last = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.SetProjectilePower), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V767_LastFields
    {
        public double AccelerationPower { get; set; }
    }

}
