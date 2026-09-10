using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.start_configuration", PacketPhase.Play, PacketDirection.Clientbound)]
public sealed partial record StartConfigurationPacket() : IPacket<StartConfigurationPacket>, IPacket
{
    public static StartConfigurationPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<StartConfigurationPacket>(protocolVersion);
        return new StartConfigurationPacket();
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<StartConfigurationPacket>(protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toClient.start_configuration", "StartConfiguration", PacketPhase.Play, PacketDirection.Clientbound, 103);

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
