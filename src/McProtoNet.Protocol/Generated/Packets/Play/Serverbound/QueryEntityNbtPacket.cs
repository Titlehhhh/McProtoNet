using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.query_entity_nbt", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("TransactionId", "int")]
[PacketField("EntityId", "int")]
public sealed partial record QueryEntityNbtPacket(int TransactionId, int EntityId) : IPacket<QueryEntityNbtPacket>, IPacket
{
    public static QueryEntityNbtPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<QueryEntityNbtPacket>(protocolVersion);
        var transactionId = reader.ReadVarInt();
        var entityId = reader.ReadVarInt();
        return new QueryEntityNbtPacket(transactionId, entityId);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<QueryEntityNbtPacket>(protocolVersion);
        writer.WriteVarInt(TransactionId);
        writer.WriteVarInt(EntityId);
    }

    public static PacketIdentity Identity => new("play.toServer.query_entity_nbt", "QueryEntityNbt", PacketPhase.Play, PacketDirection.Serverbound, 43);

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
