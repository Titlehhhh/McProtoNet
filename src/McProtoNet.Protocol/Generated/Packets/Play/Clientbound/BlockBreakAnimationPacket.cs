using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.block_break_animation", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("Location", "Position")]
[PacketField("DestroyStage", "int")]
public sealed partial record BlockBreakAnimationPacket(int EntityId, Position Location, int DestroyStage) : IPacket<BlockBreakAnimationPacket>, IPacket
{
    public static BlockBreakAnimationPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<BlockBreakAnimationPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        var location = reader.ReadType<Position>(protocolVersion);
        var destroyStage = reader.ReadSignedByte();
        return new BlockBreakAnimationPacket(entityId, location, destroyStage);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<BlockBreakAnimationPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
        writer.WriteType<Position>(Location, protocolVersion);
        writer.WriteSignedByte((sbyte)DestroyStage);
    }

    public static PacketIdentity Identity => new("play.toClient.block_break_animation", "BlockBreakAnimation", PacketPhase.Play, PacketDirection.Clientbound, 7);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x08;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x08;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x09;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 761)
        {
            id = 0x06;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x07;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 769)
        {
            id = 0x06;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 776)
        {
            id = 0x05;
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
