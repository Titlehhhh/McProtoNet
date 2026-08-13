using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.vehicle_move", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("X", "double")]
[PacketField("Y", "double")]
[PacketField("Z", "double")]
[PacketField("Yaw", "float")]
[PacketField("Pitch", "float")]
public sealed partial record VehicleMovePacket(double X, double Y, double Z, float Yaw, float Pitch) : IPacket<VehicleMovePacket>, IPacket
{
    public static VehicleMovePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<VehicleMovePacket>(protocolVersion);
        var x = reader.ReadDouble();
        var y = reader.ReadDouble();
        var z = reader.ReadDouble();
        var yaw = reader.ReadFloat();
        var pitch = reader.ReadFloat();
        return new VehicleMovePacket(x, y, z, yaw, pitch);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<VehicleMovePacket>(protocolVersion);
        writer.WriteDouble(X);
        writer.WriteDouble(Y);
        writer.WriteDouble(Z);
        writer.WriteFloat(Yaw);
        writer.WriteFloat(Pitch);
    }

    public static PacketIdentity Identity => new("play.toClient.vehicle_move", "VehicleMove", PacketPhase.Play, PacketDirection.Clientbound, 109);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x2C;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x2B;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x2C;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x29;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x2B;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x2A;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x2E;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x2F;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x31;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x33;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x32;
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
