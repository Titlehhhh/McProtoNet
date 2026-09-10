using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.configuration_acknowledged", PacketPhase.Play, PacketDirection.Serverbound)]
public sealed partial record ConfigurationAcknowledgedPacket() : IPacket<ConfigurationAcknowledgedPacket>, IPacket
{
    public static ConfigurationAcknowledgedPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ConfigurationAcknowledgedPacket>(protocolVersion);
        return new ConfigurationAcknowledgedPacket();
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ConfigurationAcknowledgedPacket>(protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toServer.configuration_acknowledged", "ConfigurationAcknowledged", PacketPhase.Play, PacketDirection.Serverbound, 16);

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
