using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.chat_command", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Command", "string")]
public sealed partial record ChatCommandPacket(string Command) : IPacket<ChatCommandPacket>, IPacket
{
    public static ChatCommandPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatCommandPacket>(protocolVersion);
        var command = reader.ReadString();
        return new ChatCommandPacket(command);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatCommandPacket>(protocolVersion);
        writer.WriteString(Command);
    }

    public static PacketIdentity Identity => new("play.toServer.chat_command", "ChatCommand", PacketPhase.Play, PacketDirection.Serverbound, 8);

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
