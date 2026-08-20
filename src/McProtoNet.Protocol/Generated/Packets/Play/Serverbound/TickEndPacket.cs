using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.tick_end", PacketPhase.Play, PacketDirection.Serverbound)]
public sealed partial record TickEndPacket() : IPacket<TickEndPacket>, IPacket
{
    public static TickEndPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TickEndPacket>(protocolVersion);
        return new TickEndPacket();
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TickEndPacket>(protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toServer.tick_end", "TickEnd", PacketPhase.Play, PacketDirection.Serverbound, 55);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 768 && protocolVersion <= 770)
        {
            id = 0x0B;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x0C;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x0D;
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
