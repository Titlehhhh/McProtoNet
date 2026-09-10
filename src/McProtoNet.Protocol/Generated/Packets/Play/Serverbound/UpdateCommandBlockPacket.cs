using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

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

    public static PacketIdentity Identity => new("play.toServer.update_command_block", "UpdateCommandBlock", PacketPhase.Play, PacketDirection.Serverbound, 61);

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
