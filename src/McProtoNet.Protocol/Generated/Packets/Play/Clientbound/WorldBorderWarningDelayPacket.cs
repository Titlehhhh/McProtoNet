using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.world_border_warning_delay", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("WarningTime", "int")]
public sealed partial record WorldBorderWarningDelayPacket(int WarningTime) : IPacket<WorldBorderWarningDelayPacket>, IPacket
{
    public static WorldBorderWarningDelayPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<WorldBorderWarningDelayPacket>(protocolVersion);
        var warningTime = reader.ReadVarInt();
        return new WorldBorderWarningDelayPacket(warningTime);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<WorldBorderWarningDelayPacket>(protocolVersion);
        writer.WriteVarInt(WarningTime);
    }

    public static PacketIdentity Identity => new("play.toClient.world_border_warning_delay", "WorldBorderWarningDelay", PacketPhase.Play, PacketDirection.Clientbound, 132);

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
