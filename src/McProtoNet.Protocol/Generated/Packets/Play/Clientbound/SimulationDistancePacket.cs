using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(757, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.simulation_distance", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Distance", "int")]
public sealed partial record SimulationDistancePacket(int Distance) : IPacket<SimulationDistancePacket>, IPacket
{
    public static SimulationDistancePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SimulationDistancePacket>(protocolVersion);
        var distance = reader.ReadVarInt();
        return new SimulationDistancePacket(distance);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SimulationDistancePacket>(protocolVersion);
        writer.WriteVarInt(Distance);
    }

    public static PacketIdentity Identity => new("play.toClient.simulation_distance", "SimulationDistance", PacketPhase.Play, PacketDirection.Clientbound, 97);

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
