using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.query_block_nbt", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("TransactionId", "int")]
[PacketField("Location", "Position")]
public sealed partial record QueryBlockNbtPacket(int TransactionId, Position Location) : IPacket<QueryBlockNbtPacket>, IPacket
{
    public static QueryBlockNbtPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<QueryBlockNbtPacket>(protocolVersion);
        var transactionId = reader.ReadVarInt();
        var location = reader.ReadType<Position>(protocolVersion);
        return new QueryBlockNbtPacket(transactionId, location);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<QueryBlockNbtPacket>(protocolVersion);
        writer.WriteVarInt(TransactionId);
        writer.WriteType<Position>(Location, protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toServer.query_block_nbt", "QueryBlockNbt", PacketPhase.Play, PacketDirection.Serverbound, 42);

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
