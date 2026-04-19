using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SetProjectilePower", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[PacketId(766, 767, 0x79)]
[PacketId(768, MinecraftVersion.LatestProtocol, 0x80)]
public sealed partial class SetProjectilePowerPacket : IPacket
{
    public int Id { get; set; }

    public V766_766Fields? V766_766 { get; set; }
    public V767_LastFields? V767_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(Id);
        switch (protocolVersion)
        {
            case 766:
            {
                var fields = V766_766 ?? throw new InvalidOperationException("SetProjectilePowerPacket 766-766 fields missing.");
                writer.WriteType<Vec3f64>(fields.Power, protocolVersion);
                return;
            }
            case >= 767 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V767_Last ?? throw new InvalidOperationException("SetProjectilePowerPacket 767-last fields missing.");
                writer.WriteDouble(fields.AccelerationPower);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SetProjectilePowerPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Id = reader.ReadVarInt();
        switch (protocolVersion)
        {
            case 766:
            {
                V766_766 = new V766_766Fields { Power = reader.ReadType<Vec3f64>(protocolVersion) };
                V767_Last = null;
                return;
            }
            case >= 767 and <= MinecraftVersion.LatestProtocol:
            {
                V767_Last = new V767_LastFields { AccelerationPower = reader.ReadDouble() };
                V766_766 = null;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SetProjectilePowerPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    public struct V766_766Fields
    {
        public Vec3f64 Power { get; set; }
    }

    public struct V767_LastFields
    {
        public double AccelerationPower { get; set; }
    }
}