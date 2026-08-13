using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

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

    public static PacketIdentity Identity => new("play.toClient.hurt_animation", "HurtAnimation", PacketPhase.Play, PacketDirection.Clientbound, 46);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x21;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x22;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x24;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x25;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x24;
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
