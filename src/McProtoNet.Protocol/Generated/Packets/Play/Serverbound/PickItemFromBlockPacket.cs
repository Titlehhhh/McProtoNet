using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(769, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.pick_item_from_block", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Position", "Position")]
[PacketField("IncludeData", "bool")]
public sealed partial record PickItemFromBlockPacket(Position Position, bool IncludeData) : IPacket<PickItemFromBlockPacket>, IPacket
{
    public static PickItemFromBlockPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PickItemFromBlockPacket>(protocolVersion);
        var position = reader.ReadType<Position>(protocolVersion);
        var includeData = reader.ReadBoolean();
        return new PickItemFromBlockPacket(position, includeData);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PickItemFromBlockPacket>(protocolVersion);
        writer.WriteType<Position>(Position, protocolVersion);
        writer.WriteBoolean(IncludeData);
    }

    public static PacketIdentity Identity => new("play.toServer.pick_item_from_block", "PickItemFromBlock", PacketPhase.Play, PacketDirection.Serverbound, 34);

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
