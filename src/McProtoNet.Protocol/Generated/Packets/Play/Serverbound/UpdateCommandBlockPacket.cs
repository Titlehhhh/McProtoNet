using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.update_command_block", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Location", "Position")]
[PacketField("Command", "string")]
[PacketField("Mode", "int")]
[PacketField("Flags", "int")]
public sealed partial record UpdateCommandBlockPacket(Position Location, string Command, int Mode, int Flags) : IPacket<UpdateCommandBlockPacket>, IPacket
{
    public static UpdateCommandBlockPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateCommandBlockPacket>(protocolVersion);
        var location = reader.ReadType<Position>(protocolVersion);
        var command = reader.ReadString();
        var mode = reader.ReadVarInt();
        var flags = reader.ReadUnsignedByte();
        return new UpdateCommandBlockPacket(location, command, mode, flags);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateCommandBlockPacket>(protocolVersion);
        writer.WriteType<Position>(Location, protocolVersion);
        writer.WriteString(Command);
        writer.WriteVarInt(Mode);
        writer.WriteUnsignedByte((byte)Flags);
    }

    public static PacketIdentity Identity => new("play.toServer.update_command_block", "UpdateCommandBlock", PacketPhase.Play, PacketDirection.Serverbound, 57);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x25;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 758)
        {
            id = 0x26;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x28;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 763)
        {
            id = 0x29;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x2C;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x2D;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x30;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 768)
        {
            id = 0x32;
            return true;
        }

        if (protocolVersion >= 769 && protocolVersion <= 770)
        {
            id = 0x34;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x35;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x36;
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
