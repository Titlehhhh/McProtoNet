using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.world_border_warning_reach", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("WarningBlocks", "int")]
public sealed partial record WorldBorderWarningReachPacket(int WarningBlocks) : IPacket<WorldBorderWarningReachPacket>, IPacket
{
    public static WorldBorderWarningReachPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<WorldBorderWarningReachPacket>(protocolVersion);
        var warningBlocks = reader.ReadVarInt();
        return new WorldBorderWarningReachPacket(warningBlocks);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<WorldBorderWarningReachPacket>(protocolVersion);
        writer.WriteVarInt(WarningBlocks);
    }

    public static PacketIdentity Identity => new("play.toClient.world_border_warning_reach", "WorldBorderWarningReach", PacketPhase.Play, PacketDirection.Clientbound, 133);

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
