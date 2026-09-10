using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.world_border_center", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("X", "double")]
[PacketField("Z", "double")]
public sealed partial record WorldBorderCenterPacket(double X, double Z) : IPacket<WorldBorderCenterPacket>, IPacket
{
    public static WorldBorderCenterPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<WorldBorderCenterPacket>(protocolVersion);
        var x = reader.ReadDouble();
        var z = reader.ReadDouble();
        return new WorldBorderCenterPacket(x, z);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<WorldBorderCenterPacket>(protocolVersion);
        writer.WriteDouble(X);
        writer.WriteDouble(Z);
    }

    public static PacketIdentity Identity => new("play.toClient.world_border_center", "WorldBorderCenter", PacketPhase.Play, PacketDirection.Clientbound, 129);

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
