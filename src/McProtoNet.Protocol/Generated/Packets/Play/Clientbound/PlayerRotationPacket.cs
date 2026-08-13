using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.player_rotation", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Yaw", "float")]
[PacketField("Pitch", "float")]
public sealed partial record PlayerRotationPacket(float Yaw, float Pitch) : IPacket<PlayerRotationPacket>, IPacket
{
    public static PlayerRotationPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PlayerRotationPacket>(protocolVersion);
        var yaw = reader.ReadFloat();
        var pitch = reader.ReadFloat();
        return new PlayerRotationPacket(yaw, pitch);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PlayerRotationPacket>(protocolVersion);
        writer.WriteFloat(Yaw);
        writer.WriteFloat(Pitch);
    }

    public static PacketIdentity Identity => new("play.toClient.player_rotation", "PlayerRotation", PacketPhase.Play, PacketDirection.Clientbound, 63);

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
