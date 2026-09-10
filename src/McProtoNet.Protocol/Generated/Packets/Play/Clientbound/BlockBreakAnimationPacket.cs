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
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
