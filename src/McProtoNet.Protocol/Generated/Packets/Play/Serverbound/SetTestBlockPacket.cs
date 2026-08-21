using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(770, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.set_test_block", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Position", "Position")]
[PacketField("Mode", "int")]
[PacketField("Message", "string")]
public sealed partial record SetTestBlockPacket(Position Position, int Mode, string Message) : IPacket<SetTestBlockPacket>, IPacket
{
    public static SetTestBlockPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetTestBlockPacket>(protocolVersion);
        var position = reader.ReadType<Position>(protocolVersion);
        var mode = reader.ReadVarInt();
        var message = reader.ReadString();
        return new SetTestBlockPacket(position, mode, message);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetTestBlockPacket>(protocolVersion);
        writer.WriteType<Position>(Position, protocolVersion);
        writer.WriteVarInt(Mode);
        writer.WriteString(Message);
    }

    public static PacketIdentity Identity => new("play.toServer.set_test_block", "SetTestBlock", PacketPhase.Play, PacketDirection.Serverbound, 48);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 770 && protocolVersion <= 770)
        {
            id = 0x39;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x3A;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x3C;
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
