using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.animation", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("Animation", "int")]
public sealed partial record AnimationPacket(int EntityId, int Animation) : IPacket<AnimationPacket>, IPacket
{
    public static AnimationPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AnimationPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        var animation = reader.ReadUnsignedByte();
        return new AnimationPacket(entityId, animation);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AnimationPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
        writer.WriteUnsignedByte((byte)Animation);
    }

    public static PacketIdentity Identity => new("play.toClient.animation", "Animation", PacketPhase.Play, PacketDirection.Clientbound, 4);

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
