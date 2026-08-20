using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

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

    public static PacketIdentity Identity => new("play.toServer.configuration_acknowledged", "ConfigurationAcknowledged", PacketPhase.Play, PacketDirection.Serverbound, 13);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x0B;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x0C;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 770)
        {
            id = 0x0E;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x0F;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x10;
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
