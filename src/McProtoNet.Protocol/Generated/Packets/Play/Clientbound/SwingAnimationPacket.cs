using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(777, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.swing_animation", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("Hand", "int")]
[PacketField("AnimationType", "int")]
[PacketField("Duration", "int")]
public sealed partial record SwingAnimationPacket(int EntityId, int Hand, int AnimationType, int Duration) : IPacket<SwingAnimationPacket>, IPacket
{
    public static SwingAnimationPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SwingAnimationPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        var hand = reader.ReadVarInt();
        var animationType = reader.ReadVarInt();
        var duration = reader.ReadVarInt();
        return new SwingAnimationPacket(entityId, hand, animationType, duration);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SwingAnimationPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
        writer.WriteVarInt(Hand);
        writer.WriteVarInt(AnimationType);
        writer.WriteVarInt(Duration);
    }

    public static PacketIdentity Identity => new("play.toClient.swing_animation", "SwingAnimation", PacketPhase.Play, PacketDirection.Clientbound, 113);

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
