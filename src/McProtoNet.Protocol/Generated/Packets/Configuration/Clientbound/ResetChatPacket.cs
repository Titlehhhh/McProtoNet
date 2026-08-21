using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toClient.reset_chat", PacketPhase.Configuration, PacketDirection.Clientbound)]
public sealed partial record ResetChatPacket() : IPacket<ResetChatPacket>, IPacket
{
    public static ResetChatPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ResetChatPacket>(protocolVersion);
        return new ResetChatPacket();
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ResetChatPacket>(protocolVersion);
    }

    public static PacketIdentity Identity => new("configuration.toClient.reset_chat", "ResetChat", PacketPhase.Configuration, PacketDirection.Clientbound, 12);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 766 && protocolVersion <= 776)
        {
            id = 0x06;
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
