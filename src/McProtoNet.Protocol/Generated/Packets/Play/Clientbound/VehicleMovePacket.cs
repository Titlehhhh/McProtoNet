using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

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

    public static PacketIdentity Identity => new("play.toClient.vehicle_move", "VehicleMove", PacketPhase.Play, PacketDirection.Clientbound, 127);

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
