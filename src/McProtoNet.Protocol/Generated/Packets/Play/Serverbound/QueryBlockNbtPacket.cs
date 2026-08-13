using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

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

    public static PacketIdentity Identity => new("play.toServer.query_block_nbt", "QueryBlockNbt", PacketPhase.Play, PacketDirection.Serverbound, 37);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x01;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 772)
        {
            id = 0x01;
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
