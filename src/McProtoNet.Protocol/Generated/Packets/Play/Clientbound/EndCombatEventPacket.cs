using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.end_combat_event", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Duration", "int")]
[PacketField("EntityId", "int", Group = "V755_762", From = 755, To = 762)]
public sealed partial record EndCombatEventPacket(int Duration, EndCombatEventPacket.V755_762Layer? V755_762 = null) : IPacket<EndCombatEventPacket>, IPacket
{
    public readonly record struct V755_762Layer(int EntityId);
    public static EndCombatEventPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EndCombatEventPacket>(protocolVersion);
        if (protocolVersion >= 755 && protocolVersion <= 762)
        {
            var duration = reader.ReadVarInt();
            var entityId = reader.ReadSignedInt();
            return new EndCombatEventPacket(duration, V755_762: new V755_762Layer(entityId));
        }

        if (protocolVersion >= 763)
        {
            var duration = reader.ReadVarInt();
            return new EndCombatEventPacket(duration);
        }

        throw new System.NotSupportedException($"EndCombatEventPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EndCombatEventPacket>(protocolVersion);
        if (protocolVersion >= 755 && protocolVersion <= 762)
        {
            var layer = V755_762 ?? throw new WrongLayerException("EndCombatEventPacket", protocolVersion, "V755_762");
            int EntityId = layer.EntityId;
            writer.WriteVarInt(Duration);
            writer.WriteSignedInt(EntityId);
            return;
        }

        if (protocolVersion >= 763)
        {
            writer.WriteVarInt(Duration);
            return;
        }

        throw new System.NotSupportedException($"EndCombatEventPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.end_combat_event", "EndCombatEvent", PacketPhase.Play, PacketDirection.Clientbound, 29);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x33;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x31;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x34;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x32;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x36;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x38;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x3A;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x3C;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x3B;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x40;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x42;
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
