using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.entity_status", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("EntityStatus", "int")]
public sealed partial record EntityStatusPacket(int EntityId, int EntityStatus) : IPacket<EntityStatusPacket>, IPacket
{
    public static EntityStatusPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityStatusPacket>(protocolVersion);
        var entityId = reader.ReadSignedInt();
        var entityStatus = reader.ReadSignedByte();
        return new EntityStatusPacket(entityId, entityStatus);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityStatusPacket>(protocolVersion);
        writer.WriteSignedInt(EntityId);
        writer.WriteSignedByte((sbyte)EntityStatus);
    }

    public static PacketIdentity Identity => new("play.toClient.entity_status", "EntityStatus", PacketPhase.Play, PacketDirection.Clientbound, 39);

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
