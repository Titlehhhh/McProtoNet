using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.position_look", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("X", "double")]
[PacketField("Y", "double")]
[PacketField("Z", "double")]
[PacketField("Yaw", "float")]
[PacketField("Pitch", "float")]
[PacketField("OnGround", "bool", Group = "VUntil767", To = 767)]
[PacketField("Flags", "MovementFlags", Group = "V768_Last", From = 768)]
public sealed partial record PositionLookPacket(double X, double Y, double Z, float Yaw, float Pitch, PositionLookPacket.VUntil767Layer? VUntil767 = null, PositionLookPacket.V768_LastLayer? V768_Last = null) : IPacket<PositionLookPacket>, IPacket
{
    public readonly record struct VUntil767Layer(bool OnGround);
    public readonly record struct V768_LastLayer(MovementFlags Flags);
    public static PositionLookPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PositionLookPacket>(protocolVersion);
        if (protocolVersion <= 767)
        {
            var x = reader.ReadDouble();
            var y = reader.ReadDouble();
            var z = reader.ReadDouble();
            var yaw = reader.ReadFloat();
            var pitch = reader.ReadFloat();
            var onGround = reader.ReadBoolean();
            return new PositionLookPacket(x, y, z, yaw, pitch, VUntil767: new VUntil767Layer(onGround));
        }

        if (protocolVersion >= 768)
        {
            var x = reader.ReadDouble();
            var y = reader.ReadDouble();
            var z = reader.ReadDouble();
            var yaw = reader.ReadFloat();
            var pitch = reader.ReadFloat();
            var flags = reader.ReadType<MovementFlags>(protocolVersion);
            return new PositionLookPacket(x, y, z, yaw, pitch, V768_Last: new V768_LastLayer(flags));
        }

        throw new System.NotSupportedException($"PositionLookPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PositionLookPacket>(protocolVersion);
        if (protocolVersion <= 767)
        {
            var layer = VUntil767 ?? throw new WrongLayerException("PositionLookPacket", protocolVersion, "VUntil767");
            bool OnGround = layer.OnGround;
            writer.WriteDouble(X);
            writer.WriteDouble(Y);
            writer.WriteDouble(Z);
            writer.WriteFloat(Yaw);
            writer.WriteFloat(Pitch);
            writer.WriteBoolean(OnGround);
            return;
        }

        if (protocolVersion >= 768)
        {
            var layer = V768_Last ?? throw new WrongLayerException("PositionLookPacket", protocolVersion, "V768_Last");
            MovementFlags Flags = layer.Flags;
            writer.WriteDouble(X);
            writer.WriteDouble(Y);
            writer.WriteDouble(Z);
            writer.WriteFloat(Yaw);
            writer.WriteFloat(Pitch);
            writer.WriteType<MovementFlags>(Flags, protocolVersion);
            return;
        }

        throw new System.NotSupportedException($"PositionLookPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toServer.position_look", "PositionLook", PacketPhase.Play, PacketDirection.Serverbound, 37);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x13;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x13;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x12;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x14;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x15;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x14;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x15;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x17;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x18;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x1B;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 770)
        {
            id = 0x1D;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x1E;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x1F;
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
