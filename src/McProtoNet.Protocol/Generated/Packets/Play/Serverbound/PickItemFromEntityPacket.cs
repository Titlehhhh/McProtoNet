using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(769, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.pick_item_from_entity", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("EntityId", "int")]
[PacketField("IncludeData", "bool")]
public sealed partial record PickItemFromEntityPacket(int EntityId, bool IncludeData) : IPacket<PickItemFromEntityPacket>, IPacket
{
    public static PickItemFromEntityPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PickItemFromEntityPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        var includeData = reader.ReadBoolean();
        return new PickItemFromEntityPacket(entityId, includeData);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PickItemFromEntityPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
        writer.WriteBoolean(IncludeData);
    }

    public static PacketIdentity Identity => new("play.toServer.pick_item_from_entity", "PickItemFromEntity", PacketPhase.Play, PacketDirection.Serverbound, 35);

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
