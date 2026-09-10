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
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
