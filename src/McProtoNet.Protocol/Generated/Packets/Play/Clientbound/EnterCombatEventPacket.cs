using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.enter_combat_event", PacketPhase.Play, PacketDirection.Clientbound)]
public sealed partial record EnterCombatEventPacket() : IPacket<EnterCombatEventPacket>, IPacket
{
    public static EnterCombatEventPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EnterCombatEventPacket>(protocolVersion);
        return new EnterCombatEventPacket();
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EnterCombatEventPacket>(protocolVersion);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.enter_combat_event", "EnterCombatEvent", PacketPhase.Play, PacketDirection.Clientbound, 32);

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
