using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(775, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.attack", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("EntityId", "int")]
public sealed partial record AttackPacket(int EntityId) : IPacket<AttackPacket>, IPacket
{
    public static AttackPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AttackPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        return new AttackPacket(entityId);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AttackPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
    }

    public static PacketIdentity Identity => new("play.toServer.attack", "Attack", PacketPhase.Play, PacketDirection.Serverbound, 2);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 775 && protocolVersion <= 776)
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
