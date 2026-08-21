using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;
[ProtocolSupport(773, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toServer.accept_code_of_conduct", PacketPhase.Configuration, PacketDirection.Serverbound)]
public sealed partial record AcceptCodeOfConductPacket() : IPacket<AcceptCodeOfConductPacket>, IPacket
{
    public static AcceptCodeOfConductPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AcceptCodeOfConductPacket>(protocolVersion);
        return new AcceptCodeOfConductPacket();
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AcceptCodeOfConductPacket>(protocolVersion);
    }

    public static PacketIdentity Identity => new("configuration.toServer.accept_code_of_conduct", "AcceptCodeOfConduct", PacketPhase.Configuration, PacketDirection.Serverbound, 0);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 773 && protocolVersion <= 776)
        {
            id = 0x09;
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
