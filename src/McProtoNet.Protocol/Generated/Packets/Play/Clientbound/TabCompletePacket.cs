using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.tab_complete", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("TransactionId", "int")]
[PacketField("Start", "int")]
[PacketField("Length", "int")]
[PacketField("Matches", "TabCompleteMatch[]")]
public sealed partial record TabCompletePacket(int TransactionId, int Start, int Length, TabCompleteMatch[] Matches) : IPacket<TabCompletePacket>, IPacket
{
    public static TabCompletePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TabCompletePacket>(protocolVersion);
        var transactionId = reader.ReadVarInt();
        var start = reader.ReadVarInt();
        var length = reader.ReadVarInt();
        int matchesCount = reader.ReadVarInt();
        var matches = new TabCompleteMatch[matchesCount];
        for (int i = 0; i < matches.Length; i++)
            matches[i] = reader.ReadType<TabCompleteMatch>(protocolVersion);
        return new TabCompletePacket(transactionId, start, length, matches);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TabCompletePacket>(protocolVersion);
        writer.WriteVarInt(TransactionId);
        writer.WriteVarInt(Start);
        writer.WriteVarInt(Length);
        writer.WriteVarInt(Matches.Length);
        foreach (var matchesItem in Matches)
            writer.WriteType<TabCompleteMatch>(matchesItem, protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toClient.tab_complete", "TabComplete", PacketPhase.Play, PacketDirection.Clientbound, 111);

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
