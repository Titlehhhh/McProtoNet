using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public sealed partial class SpawnPositionPacket : IProtocolType<SpawnPositionPacket>
{
    public Position Location { get; }
    public float Angle { get; }

    public SpawnPositionPacket(Position location, float angle)
    {
        Location = location;
        Angle = angle;
    }

    public static SpawnPositionPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpawnPositionPacket>(protocolVersion);
        if (protocolVersion <= 754)
        {
            var location = reader.ReadType<Position>(protocolVersion);
            return new SpawnPositionPacket(location, default!);
        }

        if (protocolVersion >= 755)
        {
            var location = reader.ReadType<Position>(protocolVersion);
            var angle = reader.ReadFloat();
            return new SpawnPositionPacket(location, angle);
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
            writer.WriteType<Position>(Location, protocolVersion);
            writer.WriteFloat(Angle);
            return;
        }

        throw new System.NotSupportedException($"SpawnPositionPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
            return 0x42;
        if (protocolVersion >= 751 && protocolVersion <= 754)
            return 0x42;
        if (protocolVersion >= 755 && protocolVersion <= 755)
            return 0x4B;
        if (protocolVersion >= 756 && protocolVersion <= 756)
            return 0x4B;
        if (protocolVersion >= 757 && protocolVersion <= 758)
            return 0x4B;
        if (protocolVersion >= 759 && protocolVersion <= 759)
            return 0x4A;
        if (protocolVersion >= 760 && protocolVersion <= 760)
            return 0x4D;
        if (protocolVersion >= 761 && protocolVersion <= 761)
            return 0x4C;
        if (protocolVersion >= 762 && protocolVersion <= 763)
            return 0x50;
        if (protocolVersion >= 764 && protocolVersion <= 764)
            return 0x52;
        if (protocolVersion >= 765 && protocolVersion <= 765)
            return 0x54;
        if (protocolVersion >= 766 && protocolVersion <= 766)
            return 0x56;
        if (protocolVersion >= 767 && protocolVersion <= 767)
            return 0x56;
        if (protocolVersion >= 768 && protocolVersion <= 769)
            return 0x5B;
        if (protocolVersion >= 770 && protocolVersion <= 770)
            return 0x5A;
        if (protocolVersion >= 771 && protocolVersion <= 772)
            return 0x5A;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
