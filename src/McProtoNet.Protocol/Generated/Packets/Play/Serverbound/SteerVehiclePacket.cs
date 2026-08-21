using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, 767)]
[Packet("play.toServer.steer_vehicle", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Sideways", "float")]
[PacketField("Forward", "float")]
[PacketField("Jump", "int")]
public sealed partial record SteerVehiclePacket(float Sideways, float Forward, int Jump) : IPacket<SteerVehiclePacket>, IPacket
{
    public static SteerVehiclePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SteerVehiclePacket>(protocolVersion);
        var sideways = reader.ReadFloat();
        var forward = reader.ReadFloat();
        var jump = reader.ReadUnsignedByte();
        return new SteerVehiclePacket(sideways, forward, jump);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SteerVehiclePacket>(protocolVersion);
        writer.WriteFloat(Sideways);
        writer.WriteFloat(Forward);
        writer.WriteUnsignedByte((byte)Jump);
    }

    public static PacketIdentity Identity => new("play.toServer.steer_vehicle", "SteerVehicle", PacketPhase.Play, PacketDirection.Serverbound, 52);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x1D;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x1D;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x1C;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x1E;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x1F;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x1E;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x1F;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x22;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x23;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x26;
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
