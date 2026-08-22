using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.spawn_position", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Location", "Position", Group = "VUntil754", To = 754)]
[PacketField("Location", "Position", Group = "V755_772", From = 755, To = 772)]
[PacketField("Angle", "float", Group = "V755_772", From = 755, To = 772)]
[PacketField("RespawnData", "RespawnData", Group = "V773_Last", From = 773)]
public sealed partial record SpawnPositionPacket(SpawnPositionPacket.VUntil754Layer? VUntil754 = null, SpawnPositionPacket.V755_772Layer? V755_772 = null, SpawnPositionPacket.V773_LastLayer? V773_Last = null) : IPacket<SpawnPositionPacket>, IPacket
{
    public readonly record struct VUntil754Layer(Position Location);
    public readonly record struct V755_772Layer(Position Location, float Angle);
    public readonly record struct V773_LastLayer(RespawnData RespawnData);
    public static SpawnPositionPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpawnPositionPacket>(protocolVersion);
        if (protocolVersion <= 754)
        {
            var location = reader.ReadType<Position>(protocolVersion);
            return new SpawnPositionPacket(VUntil754: new VUntil754Layer(location));
        }

        if (protocolVersion >= 755 && protocolVersion <= 772)
        {
            var location = reader.ReadType<Position>(protocolVersion);
            var angle = reader.ReadFloat();
            return new SpawnPositionPacket(V755_772: new V755_772Layer(location, angle));
        }

        if (protocolVersion >= 773)
        {
            var respawnData = reader.ReadType<RespawnData>(protocolVersion);
            return new SpawnPositionPacket(V773_Last: new V773_LastLayer(respawnData));
        }

        throw new System.NotSupportedException($"SpawnPositionPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpawnPositionPacket>(protocolVersion);
        if (protocolVersion <= 754)
        {
            var layer = VUntil754 ?? throw new WrongLayerException("SpawnPositionPacket", protocolVersion, "VUntil754");
            Position Location = layer.Location;
            writer.WriteType<Position>(Location, protocolVersion);
            return;
        }

        if (protocolVersion >= 755 && protocolVersion <= 772)
        {
            var layer = V755_772 ?? throw new WrongLayerException("SpawnPositionPacket", protocolVersion, "V755_772");
            Position Location = layer.Location;
            float Angle = layer.Angle;
            writer.WriteType<Position>(Location, protocolVersion);
            writer.WriteFloat(Angle);
            return;
        }

        if (protocolVersion >= 773)
        {
            var layer = V773_Last ?? throw new WrongLayerException("SpawnPositionPacket", protocolVersion, "V773_Last");
            RespawnData RespawnData = layer.RespawnData;
            writer.WriteType<RespawnData>(RespawnData, protocolVersion);
            return;
        }

        throw new System.NotSupportedException($"SpawnPositionPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.spawn_position", "SpawnPosition", PacketPhase.Play, PacketDirection.Clientbound, 95);

    PacketIdentity IPacket.Identity => Identity;

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

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x5F;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x61;
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
