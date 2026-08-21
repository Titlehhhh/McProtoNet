using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toClient.finish_configuration", PacketPhase.Configuration, PacketDirection.Clientbound)]
public sealed partial record FinishConfigurationPacket() : IPacket<FinishConfigurationPacket>, IPacket
{
    public static FinishConfigurationPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<FinishConfigurationPacket>(protocolVersion);
        return new FinishConfigurationPacket();
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<FinishConfigurationPacket>(protocolVersion);
    }

    public static PacketIdentity Identity => new("configuration.toClient.finish_configuration", "FinishConfiguration", PacketPhase.Configuration, PacketDirection.Clientbound, 8);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x02;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 776)
        {
            id = 0x03;
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
