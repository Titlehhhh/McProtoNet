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

    public static PacketIdentity Identity => new("play.toServer.set_test_block", "SetTestBlock", PacketPhase.Play, PacketDirection.Serverbound, 52);

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
