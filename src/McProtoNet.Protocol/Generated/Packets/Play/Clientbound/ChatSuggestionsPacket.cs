using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(760, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.chat_suggestions", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Action", "int")]
[PacketField("Entries", "string[]")]
public sealed partial record ChatSuggestionsPacket(int Action, string[] Entries) : IPacket<ChatSuggestionsPacket>, IPacket
{
    public static ChatSuggestionsPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatSuggestionsPacket>(protocolVersion);
        var action = reader.ReadVarInt();
        int entriesCount = reader.ReadVarInt();
        var entries = new string[entriesCount];
        for (int i = 0; i < entries.Length; i++)
            entries[i] = reader.ReadString();
        return new ChatSuggestionsPacket(action, entries);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatSuggestionsPacket>(protocolVersion);
        writer.WriteVarInt(Action);
        writer.WriteVarInt(Entries.Length);
        foreach (var entriesItem in Entries)
            writer.WriteString(entriesItem);
    }

    public static PacketIdentity Identity => new("play.toClient.chat_suggestions", "ChatSuggestions", PacketPhase.Play, PacketDirection.Clientbound, 13);

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
