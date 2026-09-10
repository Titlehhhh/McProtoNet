using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.tab_complete", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("TransactionId", "int")]
[PacketField("Text", "string")]
public sealed partial record TabCompletePacket(int TransactionId, string Text) : IPacket<TabCompletePacket>, IPacket
{
    public static TabCompletePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TabCompletePacket>(protocolVersion);
        var transactionId = reader.ReadVarInt();
        var text = reader.ReadString();
        return new TabCompletePacket(transactionId, text);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TabCompletePacket>(protocolVersion);
        writer.WriteVarInt(TransactionId);
        writer.WriteString(Text);
    }

    public static PacketIdentity Identity => new("play.toServer.tab_complete", "TabComplete", PacketPhase.Play, PacketDirection.Serverbound, 57);

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
