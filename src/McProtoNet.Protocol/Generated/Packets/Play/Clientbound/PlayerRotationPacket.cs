using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.player_rotation", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Yaw", "float")]
[PacketField("Pitch", "float")]
[PacketField("RelativeYaw", "bool", Group = "V773_Last", From = 773)]
[PacketField("RelativePitch", "bool", Group = "V773_Last", From = 773)]
public sealed partial record PlayerRotationPacket(float Yaw, float Pitch, PlayerRotationPacket.V773_LastLayer? V773_Last = null) : IPacket<PlayerRotationPacket>, IPacket
{
    public readonly record struct V773_LastLayer(bool RelativeYaw, bool RelativePitch);
    public static PlayerRotationPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PlayerRotationPacket>(protocolVersion);
        if (protocolVersion >= 768 && protocolVersion <= 772)
        {
            var yaw = reader.ReadFloat();
            var pitch = reader.ReadFloat();
            return new PlayerRotationPacket(yaw, pitch);
        }

        if (protocolVersion >= 773)
        {
            var yaw = reader.ReadFloat();
            var relativeYaw = reader.ReadBoolean();
            var pitch = reader.ReadFloat();
            var relativePitch = reader.ReadBoolean();
            return new PlayerRotationPacket(yaw, pitch, V773_Last: new V773_LastLayer(relativeYaw, relativePitch));
        }

        throw new System.NotSupportedException($"PlayerRotationPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PlayerRotationPacket>(protocolVersion);
        if (protocolVersion >= 768 && protocolVersion <= 772)
        {
            writer.WriteFloat(Yaw);
            writer.WriteFloat(Pitch);
            return;
        }

        if (protocolVersion >= 773)
        {
            var layer = V773_Last ?? throw new WrongLayerException("PlayerRotationPacket", protocolVersion, "V773_Last");
            bool RelativeYaw = layer.RelativeYaw;
            bool RelativePitch = layer.RelativePitch;
            writer.WriteFloat(Yaw);
            writer.WriteBoolean(RelativeYaw);
            writer.WriteFloat(Pitch);
            writer.WriteBoolean(RelativePitch);
            return;
        }

        throw new System.NotSupportedException($"PlayerRotationPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.player_rotation", "PlayerRotation", PacketPhase.Play, PacketDirection.Clientbound, 67);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x43;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x42;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x47;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x49;
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
