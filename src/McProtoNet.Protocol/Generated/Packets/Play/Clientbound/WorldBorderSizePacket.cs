using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.world_border_size", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Diameter", "double")]
public sealed partial record WorldBorderSizePacket(double Diameter) : IPacket<WorldBorderSizePacket>, IPacket
{
    public static WorldBorderSizePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<WorldBorderSizePacket>(protocolVersion);
        var diameter = reader.ReadDouble();
        return new WorldBorderSizePacket(diameter);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<WorldBorderSizePacket>(protocolVersion);
        writer.WriteDouble(Diameter);
    }

    public static PacketIdentity Identity => new("play.toClient.world_border_size", "WorldBorderSize", PacketPhase.Play, PacketDirection.Clientbound, 130);

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
