using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toServer.finish_configuration", PacketPhase.Configuration, PacketDirection.Serverbound)]
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

    public static PacketIdentity Identity => new("configuration.toServer.finish_configuration", "FinishConfiguration", PacketPhase.Configuration, PacketDirection.Serverbound, 5);

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
