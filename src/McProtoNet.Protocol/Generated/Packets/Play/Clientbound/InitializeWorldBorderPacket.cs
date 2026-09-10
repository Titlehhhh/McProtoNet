using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.initialize_world_border", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("X", "double")]
[PacketField("Z", "double")]
[PacketField("OldDiameter", "double")]
[PacketField("NewDiameter", "double")]
[PacketField("Speed", "long")]
[PacketField("PortalTeleportBoundary", "int")]
[PacketField("WarningBlocks", "int")]
[PacketField("WarningTime", "int")]
public sealed partial record InitializeWorldBorderPacket(double X, double Z, double OldDiameter, double NewDiameter, long Speed, int PortalTeleportBoundary, int WarningBlocks, int WarningTime) : IPacket<InitializeWorldBorderPacket>, IPacket
{
    public static InitializeWorldBorderPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<InitializeWorldBorderPacket>(protocolVersion);
        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            var x = reader.ReadDouble();
            var z = reader.ReadDouble();
            var oldDiameter = reader.ReadDouble();
            var newDiameter = reader.ReadDouble();
            var speed = reader.ReadVarLong();
            var portalTeleportBoundary = reader.ReadVarInt();
            var warningBlocks = reader.ReadVarInt();
            var warningTime = reader.ReadVarInt();
            return new InitializeWorldBorderPacket(x, z, oldDiameter, newDiameter, speed, portalTeleportBoundary, warningBlocks, warningTime);
        }

        if (protocolVersion >= 759)
        {
            var x = reader.ReadDouble();
            var z = reader.ReadDouble();
            var oldDiameter = reader.ReadDouble();
            var newDiameter = reader.ReadDouble();
            var speed = reader.ReadVarInt();
            var portalTeleportBoundary = reader.ReadVarInt();
            var warningBlocks = reader.ReadVarInt();
            var warningTime = reader.ReadVarInt();
            return new InitializeWorldBorderPacket(x, z, oldDiameter, newDiameter, speed, portalTeleportBoundary, warningBlocks, warningTime);
        }

        throw new System.NotSupportedException($"InitializeWorldBorderPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<InitializeWorldBorderPacket>(protocolVersion);
        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            writer.WriteDouble(X);
            writer.WriteDouble(Z);
            writer.WriteDouble(OldDiameter);
            writer.WriteDouble(NewDiameter);
            writer.WriteVarLong(Speed);
            writer.WriteVarInt(PortalTeleportBoundary);
            writer.WriteVarInt(WarningBlocks);
            writer.WriteVarInt(WarningTime);
            return;
        }

        if (protocolVersion >= 759)
        {
            writer.WriteDouble(X);
            writer.WriteDouble(Z);
            writer.WriteDouble(OldDiameter);
            writer.WriteDouble(NewDiameter);
            writer.WriteVarInt((int)Speed);
            writer.WriteVarInt(PortalTeleportBoundary);
            writer.WriteVarInt(WarningBlocks);
            writer.WriteVarInt(WarningTime);
            return;
        }

        throw new System.NotSupportedException($"InitializeWorldBorderPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.initialize_world_border", "InitializeWorldBorder", PacketPhase.Play, PacketDirection.Clientbound, 53);

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
