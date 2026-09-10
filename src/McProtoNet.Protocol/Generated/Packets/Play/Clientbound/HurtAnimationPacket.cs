using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(762, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.hurt_animation", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("Yaw", "float")]
public sealed partial record HurtAnimationPacket(int EntityId, float Yaw) : IPacket<HurtAnimationPacket>, IPacket
{
    public static HurtAnimationPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<HurtAnimationPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        var yaw = reader.ReadFloat();
        return new HurtAnimationPacket(entityId, yaw);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<HurtAnimationPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
        writer.WriteFloat(Yaw);
    }

    public static PacketIdentity Identity => new("play.toClient.hurt_animation", "HurtAnimation", PacketPhase.Play, PacketDirection.Clientbound, 52);

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
