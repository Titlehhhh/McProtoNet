using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.block_change", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Location", "Position")]
[PacketField("Type", "int")]
public sealed partial record BlockChangePacket(Position Location, int Type) : IPacket<BlockChangePacket>, IPacket
{
    public static BlockChangePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<BlockChangePacket>(protocolVersion);
        var location = reader.ReadType<Position>(protocolVersion);
        var type = reader.ReadVarInt();
        return new BlockChangePacket(location, type);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<BlockChangePacket>(protocolVersion);
        writer.WriteType<Position>(Location, protocolVersion);
        writer.WriteVarInt(Type);
    }

    public static PacketIdentity Identity => new("play.toClient.block_change", "BlockChange", PacketPhase.Play, PacketDirection.Clientbound, 8);

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
