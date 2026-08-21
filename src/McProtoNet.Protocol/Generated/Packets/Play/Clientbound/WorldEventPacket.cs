using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.world_event", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EffectId", "int")]
[PacketField("Location", "Position")]
[PacketField("Data", "int")]
[PacketField("Global", "bool")]
public sealed partial record WorldEventPacket(int EffectId, Position Location, int Data, bool Global) : IPacket<WorldEventPacket>, IPacket
{
    public static WorldEventPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<WorldEventPacket>(protocolVersion);
        var effectId = reader.ReadSignedInt();
        var location = reader.ReadType<Position>(protocolVersion);
        var data = reader.ReadSignedInt();
        var global = reader.ReadBoolean();
        return new WorldEventPacket(effectId, location, data, global);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<WorldEventPacket>(protocolVersion);
        writer.WriteSignedInt(EffectId);
        writer.WriteType<Position>(Location, protocolVersion);
        writer.WriteSignedInt(Data);
        writer.WriteBoolean(Global);
    }

    public static PacketIdentity Identity => new("play.toClient.world_event", "WorldEvent", PacketPhase.Play, PacketDirection.Clientbound, 119);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x22;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x21;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x23;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x20;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x22;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x21;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x25;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x26;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x28;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x29;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x28;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x2D;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x2E;
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
