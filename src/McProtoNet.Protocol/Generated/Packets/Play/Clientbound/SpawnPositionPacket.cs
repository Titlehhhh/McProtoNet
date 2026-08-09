using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.spawn_position", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Location", "Position")]
[PacketField("Angle", "float", Group = "V755_Last", From = 755)]
public sealed partial record SpawnPositionPacket(Position Location, SpawnPositionPacket.V755_LastLayer? V755_Last = null) : IPacket<SpawnPositionPacket>
{
    public readonly record struct V755_LastLayer(float Angle);
    public static SpawnPositionPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpawnPositionPacket>(protocolVersion);
        if (protocolVersion <= 754)
        {
            var location = reader.ReadType<Position>(protocolVersion);
            return new SpawnPositionPacket(location);
        }

        if (protocolVersion >= 755)
        {
            var location = reader.ReadType<Position>(protocolVersion);
            var angle = reader.ReadFloat();
            return new SpawnPositionPacket(location, V755_Last: new V755_LastLayer(angle));
        }

        throw new System.NotSupportedException($"SpawnPositionPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpawnPositionPacket>(protocolVersion);
        if (protocolVersion <= 754)
        {
            writer.WriteType<Position>(Location, protocolVersion);
            return;
        }

        if (protocolVersion >= 755)
        {
            var layer = V755_Last ?? throw new WrongLayerException("SpawnPositionPacket", protocolVersion, "V755_Last");
            float Angle = layer.Angle;
            writer.WriteType<Position>(Location, protocolVersion);
            writer.WriteFloat(Angle);
            return;
        }

        throw new System.NotSupportedException($"SpawnPositionPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.spawn_position", "SpawnPosition", PacketPhase.Play, PacketDirection.Clientbound, 13);

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x42;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x42;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x4B;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x4A;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x4D;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x4C;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x50;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x52;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x54;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x56;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x5B;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x5A;
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
