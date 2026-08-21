using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.update_command_block_minecart", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("EntityId", "int")]
[PacketField("Command", "string")]
[PacketField("TrackOutput", "bool")]
public sealed partial record UpdateCommandBlockMinecartPacket(int EntityId, string Command, bool TrackOutput) : IPacket<UpdateCommandBlockMinecartPacket>, IPacket
{
    public static UpdateCommandBlockMinecartPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateCommandBlockMinecartPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        var command = reader.ReadString();
        var trackOutput = reader.ReadBoolean();
        return new UpdateCommandBlockMinecartPacket(entityId, command, trackOutput);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateCommandBlockMinecartPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
        writer.WriteString(Command);
        writer.WriteBoolean(TrackOutput);
    }

    public static PacketIdentity Identity => new("play.toServer.update_command_block_minecart", "UpdateCommandBlockMinecart", PacketPhase.Play, PacketDirection.Serverbound, 58);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x26;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 758)
        {
            id = 0x27;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x29;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 763)
        {
            id = 0x2A;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x2D;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x2E;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x31;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 768)
        {
            id = 0x33;
            return true;
        }

        if (protocolVersion >= 769 && protocolVersion <= 770)
        {
            id = 0x35;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x36;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x37;
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
