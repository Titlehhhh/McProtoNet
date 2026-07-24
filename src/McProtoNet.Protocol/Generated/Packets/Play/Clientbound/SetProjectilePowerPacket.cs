using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class SetProjectilePowerPacket : IProtocolType<SetProjectilePowerPacket>
{
    public int Id { get; }
    public Vec3f64 Power { get; }
    public double AccelerationPower { get; }

    public SetProjectilePowerPacket(int id, Vec3f64 power, double accelerationPower)
    {
        Id = id;
        Power = power;
        AccelerationPower = accelerationPower;
    }

    public static SetProjectilePowerPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetProjectilePowerPacket>(protocolVersion);
        if (protocolVersion >= 766 && protocolVersion <= 766)
        {
            var id = reader.ReadVarInt();
            var power = reader.ReadType<Vec3f64>(protocolVersion);
            return new SetProjectilePowerPacket(id, power, default!);
        }

        if (protocolVersion >= 767)
        {
            var id = reader.ReadVarInt();
            var accelerationPower = reader.ReadDouble();
            return new SetProjectilePowerPacket(id, default!, accelerationPower);
        }

        throw new System.NotSupportedException($"SetProjectilePowerPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetProjectilePowerPacket>(protocolVersion);
        if (protocolVersion >= 766 && protocolVersion <= 766)
        {
            writer.WriteVarInt(Id);
            writer.WriteType<Vec3f64>(Power, protocolVersion);
            return;
        }

        if (protocolVersion >= 767)
        {
            writer.WriteVarInt(Id);
            writer.WriteDouble(AccelerationPower);
            return;
        }

        throw new System.NotSupportedException($"SetProjectilePowerPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 766 && protocolVersion <= 766)
            return 0x79;
        if (protocolVersion >= 767 && protocolVersion <= 767)
            return 0x79;
        if (protocolVersion >= 768 && protocolVersion <= 769)
            return 0x80;
        if (protocolVersion >= 770 && protocolVersion <= 770)
            return 0x80;
        if (protocolVersion >= 771 && protocolVersion <= 772)
            return 0x80;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
