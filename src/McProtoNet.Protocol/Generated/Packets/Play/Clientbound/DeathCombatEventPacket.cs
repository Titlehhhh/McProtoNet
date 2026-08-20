using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.death_combat_event", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("PlayerId", "int")]
[PacketField("EntityId", "int", Group = "V755_762", From = 755, To = 762)]
[PacketField("MessageJson", "string", Group = "V755_762", From = 755, To = 762)]
[PacketField("MessageJson", "string", Group = "V763_764", From = 763, To = 764)]
[PacketField("Message", "NbtTag", Group = "V765_Last", From = 765)]
public sealed partial record DeathCombatEventPacket(int PlayerId, DeathCombatEventPacket.V755_762Layer? V755_762 = null, DeathCombatEventPacket.V763_764Layer? V763_764 = null, DeathCombatEventPacket.V765_LastLayer? V765_Last = null) : IPacket<DeathCombatEventPacket>, IPacket
{
    public readonly record struct V755_762Layer(int EntityId, string MessageJson);
    public readonly record struct V763_764Layer(string MessageJson);
    public readonly record struct V765_LastLayer(NbtTag Message);
    public static DeathCombatEventPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DeathCombatEventPacket>(protocolVersion);
        if (protocolVersion >= 755 && protocolVersion <= 762)
        {
            var playerId = reader.ReadVarInt();
            var entityId = reader.ReadSignedInt();
            var messageJson = reader.ReadString();
            return new DeathCombatEventPacket(playerId, V755_762: new V755_762Layer(entityId, messageJson));
        }

        if (protocolVersion >= 763 && protocolVersion <= 764)
        {
            var playerId = reader.ReadVarInt();
            var messageJson = reader.ReadString();
            return new DeathCombatEventPacket(playerId, V763_764: new V763_764Layer(messageJson));
        }

        if (protocolVersion >= 765)
        {
            var playerId = reader.ReadVarInt();
            var message = reader.ReadNbtTag(false)!;
            return new DeathCombatEventPacket(playerId, V765_Last: new V765_LastLayer(message));
        }

        throw new System.NotSupportedException($"DeathCombatEventPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DeathCombatEventPacket>(protocolVersion);
        if (protocolVersion >= 755 && protocolVersion <= 762)
        {
            var layer = V755_762 ?? throw new WrongLayerException("DeathCombatEventPacket", protocolVersion, "V755_762");
            int EntityId = layer.EntityId;
            string MessageJson = layer.MessageJson;
            writer.WriteVarInt(PlayerId);
            writer.WriteSignedInt(EntityId);
            writer.WriteString(MessageJson);
            return;
        }

        if (protocolVersion >= 763 && protocolVersion <= 764)
        {
            var layer = V763_764 ?? throw new WrongLayerException("DeathCombatEventPacket", protocolVersion, "V763_764");
            string MessageJson = layer.MessageJson;
            writer.WriteVarInt(PlayerId);
            writer.WriteString(MessageJson);
            return;
        }

        if (protocolVersion >= 765)
        {
            var layer = V765_Last ?? throw new WrongLayerException("DeathCombatEventPacket", protocolVersion, "V765_Last");
            NbtTag Message = layer.Message;
            writer.WriteVarInt(PlayerId);
            writer.WriteNbt(Message);
            return;
        }

        throw new System.NotSupportedException($"DeathCombatEventPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.death_combat_event", "DeathCombatEvent", PacketPhase.Play, PacketDirection.Clientbound, 25);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x35;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x33;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x36;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x34;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x38;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x3A;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x3C;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x3E;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x3D;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x42;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x44;
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
