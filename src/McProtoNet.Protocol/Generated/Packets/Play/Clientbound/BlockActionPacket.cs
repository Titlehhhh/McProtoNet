using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.block_action", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Location", "Position")]
[PacketField("Byte1", "int")]
[PacketField("Byte2", "int")]
[PacketField("BlockId", "int")]
public sealed partial record BlockActionPacket(Position Location, int Byte1, int Byte2, int BlockId) : IPacket<BlockActionPacket>, IPacket
{
    public static BlockActionPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<BlockActionPacket>(protocolVersion);
        var location = reader.ReadType<Position>(protocolVersion);
        var byte1 = reader.ReadUnsignedByte();
        var byte2 = reader.ReadUnsignedByte();
        var blockId = reader.ReadVarInt();
        return new BlockActionPacket(location, byte1, byte2, blockId);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<BlockActionPacket>(protocolVersion);
        writer.WriteType<Position>(Location, protocolVersion);
        writer.WriteUnsignedByte((byte)Byte1);
        writer.WriteUnsignedByte((byte)Byte2);
        writer.WriteVarInt(BlockId);
    }

    public static PacketIdentity Identity => new("play.toClient.block_action", "BlockAction", PacketPhase.Play, PacketDirection.Clientbound, 6);

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
