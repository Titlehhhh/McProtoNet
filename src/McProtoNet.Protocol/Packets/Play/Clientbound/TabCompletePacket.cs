using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("TabComplete", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class TabCompletePacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 764),
        new(765, MinecraftVersion.LatestProtocol),
    };

    public int TransactionId { get; set; }
    public int Start { get; set; }
    public int Length { get; set; }

    public VFirst_764Fields? VFirst_764 { get; set; }
    public V765_LastFields? V765_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
            {
                var fields = VFirst_764 ?? throw new InvalidOperationException("TabComplete VFirst_764 fields missing.");
                writer.WriteVarInt(TransactionId);
                writer.WriteVarInt(Start);
                writer.WriteVarInt(Length);
                writer.WriteVarInt(fields.Matches.Length);
                for (int i = 0; i < fields.Matches.Length; i++)
                {
                    writer.WriteString(fields.Matches[i].Match);
                    if (fields.Matches[i].Tooltip is null)
                    {
                        writer.WriteBoolean(false);
                    }
                    else
                    {
                        writer.WriteBoolean(true);
                        writer.WriteString(fields.Matches[i].Tooltip!);
                    }
                }
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V765_Last ?? throw new InvalidOperationException("TabComplete V765_Last fields missing.");
                writer.WriteVarInt(TransactionId);
                writer.WriteVarInt(Start);
                writer.WriteVarInt(Length);
                writer.WriteVarInt(fields.Matches.Length);
                for (int i = 0; i < fields.Matches.Length; i++)
                {
                    writer.WriteString(fields.Matches[i].Match);
                    if (fields.Matches[i].Tooltip is null)
                    {
                        writer.WriteBoolean(false);
                    }
                    else
                    {
                        writer.WriteBoolean(true);
                        writer.WriteAnonymousNbtTag(fields.Matches[i].Tooltip!, protocolVersion);
                    }
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.TabComplete), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
            {
                TransactionId = reader.ReadVarInt();
                Start = reader.ReadVarInt();
                Length = reader.ReadVarInt();
                int count = reader.ReadVarInt();
                var matches = new MatchEntryVFirst_764[count];
                for (int i = 0; i < matches.Length; i++)
                {
                    matches[i] = new MatchEntryVFirst_764
                    {
                        Match = reader.ReadString(),
                        Tooltip = reader.ReadOptional(ReadDelegates.String)
                    };
                }
                VFirst_764 = new VFirst_764Fields { Matches = matches };
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                TransactionId = reader.ReadVarInt();
                Start = reader.ReadVarInt();
                Length = reader.ReadVarInt();
                int count = reader.ReadVarInt();
                var matches = new MatchEntryV765_Last[count];
                for (int i = 0; i < matches.Length; i++)
                {
                    matches[i] = new MatchEntryV765_Last
                    {
                        Match = reader.ReadString(),
                        Tooltip = reader.ReadBoolean()
                            ? reader.ReadAnonymousNbtTag(protocolVersion)
                            : null
                    };
                }
                V765_Last = new V765_LastFields { Matches = matches };
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.TabComplete), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_764Fields
    {
        public MatchEntryVFirst_764[] Matches { get; set; }
    }

    public struct V765_LastFields
    {
        public MatchEntryV765_Last[] Matches { get; set; }
    }

    public struct MatchEntryVFirst_764
    {
        public string Match { get; set; }
        public string? Tooltip { get; set; }
    }

    public struct MatchEntryV765_Last
    {
        public string Match { get; set; }
        public NbtTag? Tooltip { get; set; }
    }
}
