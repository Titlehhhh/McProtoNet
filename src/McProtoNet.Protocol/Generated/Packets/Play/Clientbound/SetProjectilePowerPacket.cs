using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.set_projectile_power", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Id", "int")]
[PacketField("Power", "Vec3f64", Group = "V766", From = 766, To = 766)]
[PacketField("AccelerationPower", "double", Group = "V767_Last", From = 767)]
public sealed partial record SetProjectilePowerPacket(int Id, SetProjectilePowerPacket.V766Layer? V766 = null, SetProjectilePowerPacket.V767_LastLayer? V767_Last = null) : IPacket<SetProjectilePowerPacket>, IPacket
{
    public readonly record struct V766Layer(Vec3f64 Power);
    public readonly record struct V767_LastLayer(double AccelerationPower);
    public static SetProjectilePowerPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetProjectilePowerPacket>(protocolVersion);
        if (protocolVersion >= 766 && protocolVersion <= 766)
        {
            var id = reader.ReadVarInt();
            var power = reader.ReadType<Vec3f64>(protocolVersion);
            return new SetProjectilePowerPacket(id, V766: new V766Layer(power));
        }

        if (protocolVersion >= 767)
        {
            var id = reader.ReadVarInt();
            var accelerationPower = reader.ReadDouble();
            return new SetProjectilePowerPacket(id, V767_Last: new V767_LastLayer(accelerationPower));
        }

        throw new System.NotSupportedException($"SetProjectilePowerPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetProjectilePowerPacket>(protocolVersion);
        if (protocolVersion >= 766 && protocolVersion <= 766)
        {
            var layer = V766 ?? throw new WrongLayerException("SetProjectilePowerPacket", protocolVersion, "V766");
            Vec3f64 Power = layer.Power;
            writer.WriteVarInt(Id);
            writer.WriteType<Vec3f64>(Power, protocolVersion);
            return;
        }

        if (protocolVersion >= 767)
        {
            var layer = V767_Last ?? throw new WrongLayerException("SetProjectilePowerPacket", protocolVersion, "V767_Last");
            double AccelerationPower = layer.AccelerationPower;
            writer.WriteVarInt(Id);
            writer.WriteDouble(AccelerationPower);
            return;
        }

        throw new System.NotSupportedException($"SetProjectilePowerPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.set_projectile_power", "SetProjectilePower", PacketPhase.Play, PacketDirection.Clientbound, 84);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x79;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 772)
        {
            id = 0x80;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x85;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x87;
            return true;
        }

        id = 0;
        return false;
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (TryGetPacketId(protocolVersion, out var id))
            return id;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
