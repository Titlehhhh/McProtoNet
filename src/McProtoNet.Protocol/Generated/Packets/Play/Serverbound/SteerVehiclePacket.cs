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

    public static PacketIdentity Identity => new("play.toServer.steer_vehicle", "SteerVehicle", PacketPhase.Play, PacketDirection.Serverbound, 56);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
