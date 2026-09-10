using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.rel_entity_move", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("Dx", "int")]
[PacketField("Dy", "int")]
[PacketField("Dz", "int")]
[PacketField("OnGround", "bool")]
public sealed partial record RelEntityMovePacket(int EntityId, int Dx, int Dy, int Dz, bool OnGround) : IPacket<RelEntityMovePacket>, IPacket
{
    public static RelEntityMovePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RelEntityMovePacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        var dx = reader.ReadSignedShort();
        var dy = reader.ReadSignedShort();
        var dz = reader.ReadSignedShort();
        var onGround = reader.ReadBoolean();
        return new RelEntityMovePacket(entityId, dx, dy, dz, onGround);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RelEntityMovePacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
        writer.WriteSignedShort((short)Dx);
        writer.WriteSignedShort((short)Dy);
        writer.WriteSignedShort((short)Dz);
        writer.WriteBoolean(OnGround);
    }

    public static PacketIdentity Identity => new("play.toClient.rel_entity_move", "RelEntityMove", PacketPhase.Play, PacketDirection.Clientbound, 78);

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
