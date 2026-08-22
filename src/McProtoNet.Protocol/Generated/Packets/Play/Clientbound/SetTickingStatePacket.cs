using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(765, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.set_ticking_state", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("TickRate", "float")]
[PacketField("IsFrozen", "bool")]
public sealed partial record SetTickingStatePacket(float TickRate, bool IsFrozen) : IPacket<SetTickingStatePacket>, IPacket
{
    public static SetTickingStatePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetTickingStatePacket>(protocolVersion);
        var tickRate = reader.ReadFloat();
        var isFrozen = reader.ReadBoolean();
        return new SetTickingStatePacket(tickRate, isFrozen);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetTickingStatePacket>(protocolVersion);
        writer.WriteFloat(TickRate);
        writer.WriteBoolean(IsFrozen);
    }

    public static PacketIdentity Identity => new("play.toClient.set_ticking_state", "SetTickingState", PacketPhase.Play, PacketDirection.Clientbound, 85);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x6E;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x71;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 772)
        {
            id = 0x78;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x7D;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x7F;
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
