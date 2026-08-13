using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.world_border_lerp_size", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("OldDiameter", "double")]
[PacketField("NewDiameter", "double")]
[PacketField("Speed", "long")]
public sealed partial record WorldBorderLerpSizePacket(double OldDiameter, double NewDiameter, long Speed) : IPacket<WorldBorderLerpSizePacket>, IPacket
{
    public static WorldBorderLerpSizePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<WorldBorderLerpSizePacket>(protocolVersion);
        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            var oldDiameter = reader.ReadDouble();
            var newDiameter = reader.ReadDouble();
            var speed = reader.ReadVarLong();
            return new WorldBorderLerpSizePacket(oldDiameter, newDiameter, speed);
        }

        if (protocolVersion >= 759)
        {
            var oldDiameter = reader.ReadDouble();
            var newDiameter = reader.ReadDouble();
            var speed = reader.ReadVarInt();
            return new WorldBorderLerpSizePacket(oldDiameter, newDiameter, speed);
        }

        throw new System.NotSupportedException($"WorldBorderLerpSizePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<WorldBorderLerpSizePacket>(protocolVersion);
        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            writer.WriteDouble(OldDiameter);
            writer.WriteDouble(NewDiameter);
            writer.WriteVarLong(Speed);
            return;
        }

        if (protocolVersion >= 759)
        {
            writer.WriteDouble(OldDiameter);
            writer.WriteDouble(NewDiameter);
            writer.WriteVarInt((int)Speed);
            return;
        }

        throw new System.NotSupportedException($"WorldBorderLerpSizePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.world_border_lerp_size", "WorldBorderLerpSize", PacketPhase.Play, PacketDirection.Clientbound, 111);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x43;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x42;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x45;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x44;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x48;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x4A;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x4C;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x4E;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x53;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x52;
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
