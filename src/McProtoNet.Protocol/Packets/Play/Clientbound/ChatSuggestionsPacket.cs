using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("ChatSuggestions", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(760, MinecraftVersion.LatestProtocol)]
[PacketId(760, 760, 0x15)]
[PacketId(761, 761, 0x14)]
[PacketId(762, 763, 0x16)]
[PacketId(764, 765, 0x17)]
[PacketId(766, 769, 0x18)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x17)]
public sealed partial class ChatSuggestionsPacket : IServerPacket
{
    public int Action { get; set; }
    public string[] Entries { get;set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(Action);
        writer.WriteArray(Entries);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Action = reader.ReadVarInt();
        Entries = reader.ReadArray<string>(LengthFormat.VarInt);
    }
}