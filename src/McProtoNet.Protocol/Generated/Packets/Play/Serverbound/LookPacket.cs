using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.look", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Yaw", "float")]
[PacketField("Pitch", "float")]
[PacketField("OnGround", "bool", Group = "VUntil767", To = 767)]
[PacketField("Flags", "MovementFlags", Group = "V768_Last", From = 768)]
public sealed partial record LookPacket(float Yaw, float Pitch, LookPacket.VUntil767Layer? VUntil767 = null, LookPacket.V768_LastLayer? V768_Last = null) : IPacket<LookPacket>, IPacket
{
    public readonly record struct VUntil767Layer(bool OnGround);
    public readonly record struct V768_LastLayer(MovementFlags Flags);
    public static LookPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LookPacket>(protocolVersion);
        if (protocolVersion <= 767)
        {
            var yaw = reader.ReadFloat();
            var pitch = reader.ReadFloat();
            var onGround = reader.ReadBoolean();
            return new LookPacket(yaw, pitch, VUntil767: new VUntil767Layer(onGround));
        }

        if (protocolVersion >= 768)
        {
            var yaw = reader.ReadFloat();
            var pitch = reader.ReadFloat();
            var flags = reader.ReadType<MovementFlags>(protocolVersion);
            return new LookPacket(yaw, pitch, V768_Last: new V768_LastLayer(flags));
        }

        throw new System.NotSupportedException($"LookPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LookPacket>(protocolVersion);
        if (protocolVersion <= 767)
        {
            var layer = VUntil767 ?? throw new WrongLayerException("LookPacket", protocolVersion, "VUntil767");
            bool OnGround = layer.OnGround;
            writer.WriteFloat(Yaw);
            writer.WriteFloat(Pitch);
            writer.WriteBoolean(OnGround);
            return;
        }

        if (protocolVersion >= 768)
        {
            var layer = V768_Last ?? throw new WrongLayerException("LookPacket", protocolVersion, "V768_Last");
            MovementFlags Flags = layer.Flags;
            writer.WriteFloat(Yaw);
            writer.WriteFloat(Pitch);
            writer.WriteType<MovementFlags>(Flags, protocolVersion);
            return;
        }

        throw new System.NotSupportedException($"LookPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toServer.look", "Look", PacketPhase.Play, PacketDirection.Serverbound, 28);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x14;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x14;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x13;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x15;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x16;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x15;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x16;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x18;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x19;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x1C;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 770)
        {
            id = 0x1E;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x1F;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x20;
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
