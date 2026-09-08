using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, 754)]
[Packet("play.toClient.combat_event", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Action", "CombatEventAction")]
public sealed partial record CombatEventPacket(CombatEventAction Action) : IPacket<CombatEventPacket>, IPacket
{
    public static CombatEventPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CombatEventPacket>(protocolVersion);
        var _event = reader.ReadVarInt();
        var action = CombatEventAction.Read(ref reader, protocolVersion, (int)_event);
        return new CombatEventPacket(action);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CombatEventPacket>(protocolVersion);
        writer.WriteVarInt(Action.Discriminator(protocolVersion));
        Action.Write(writer, protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toClient.combat_event", "CombatEvent", PacketPhase.Play, PacketDirection.Clientbound, 21);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x32;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x31;
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
