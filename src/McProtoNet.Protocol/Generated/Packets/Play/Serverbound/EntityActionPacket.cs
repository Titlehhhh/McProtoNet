using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.entity_action", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("EntityId", "int")]
[PacketField("ActionId", "int")]
[PacketField("JumpBoost", "int")]
public sealed partial record EntityActionPacket(int EntityId, int ActionId, int JumpBoost) : IPacket<EntityActionPacket>, IPacket
{
    public static EntityActionPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityActionPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        var actionId = reader.ReadVarInt();
        var jumpBoost = reader.ReadVarInt();
        return new EntityActionPacket(entityId, actionId, jumpBoost);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityActionPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
        writer.WriteVarInt(ActionId);
        writer.WriteVarInt(JumpBoost);
    }

    public static PacketIdentity Identity => new("play.toServer.entity_action", "EntityAction", PacketPhase.Play, PacketDirection.Serverbound, 25);

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
