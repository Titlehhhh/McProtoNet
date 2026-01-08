using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("ChatSuggestions", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class ChatSuggestionsPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(760, MinecraftVersion.LatestProtocol),
    };

    public int Action { get; set; }
    public string[] Entries { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 760 and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(Action);
                writer.WriteVarInt(Entries.Length);
                for (int i = 0; i < Entries.Length; i++)
                {
                    writer.WriteString(Entries[i]);
                }
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.ChatSuggestions), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 760 and <= MinecraftVersion.LatestProtocol:
                Action = reader.ReadVarInt();
                Entries = reader.ReadArray<string, StringArrayReader>(LengthFormat.VarInt);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.ChatSuggestions), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
