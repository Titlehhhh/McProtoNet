using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

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

    public static PacketIdentity Identity => new("play.toClient.chat_suggestions", "ChatSuggestions", PacketPhase.Play, PacketDirection.Clientbound, 12);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x15;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x14;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x16;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x17;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 769)
        {
            id = 0x18;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 776)
        {
            id = 0x17;
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
