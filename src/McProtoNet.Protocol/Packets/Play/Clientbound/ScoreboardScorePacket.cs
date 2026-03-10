using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("ScoreboardScore", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class ScoreboardScorePacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 764),
        new(765, MinecraftVersion.LatestProtocol),
    };

    public VFirst_764Fields? VFirst_764 { get; set; }
    public V765_LastFields? V765_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
            {
                var fields = VFirst_764 ?? throw new InvalidOperationException("ScoreboardScore VFirst_764 fields missing.");
                writer.WriteString(fields.ItemName);
                writer.WriteVarInt(fields.Action);
                writer.WriteString(fields.ScoreName);
                if (fields.Action != 1)
                {
                    writer.WriteVarInt(fields.Value ?? throw new InvalidOperationException("ScoreboardScore value missing."));
                }
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V765_Last ?? throw new InvalidOperationException("ScoreboardScore V765_Last fields missing.");
                writer.WriteString(fields.ItemName);
                writer.WriteString(fields.ScoreName);
                writer.WriteVarInt(fields.Value);
                if (fields.DisplayName is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteAnonymousNbtTag(fields.DisplayName, protocolVersion);
                }
                if (fields.NumberFormat.HasValue)
                {
                    writer.WriteBoolean(true);
                    writer.WriteVarInt(fields.NumberFormat.Value);
                    if (fields.NumberFormat.Value is 1 or 2)
                    {
                        writer.WriteAnonymousNbtTag(fields.Styling ?? throw new InvalidOperationException("ScoreboardScore styling missing."), protocolVersion);
                    }
                }
                else
                {
                    writer.WriteBoolean(false);
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.ScoreboardScore), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
            {
                var fields = new VFirst_764Fields
                {
                    ItemName = reader.ReadString(),
                    Action = reader.ReadVarInt(),
                    ScoreName = reader.ReadString()
                };
                if (fields.Action != 1)
                {
                    fields.Value = reader.ReadVarInt();
                }
                VFirst_764 = fields;
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = new V765_LastFields
                {
                    ItemName = reader.ReadString(),
                    ScoreName = reader.ReadString(),
                    Value = reader.ReadVarInt()
                };
                if (reader.ReadBoolean())
                {
                    fields.DisplayName = reader.ReadAnonymousNbtTag(protocolVersion)
                        ?? throw new InvalidOperationException("ScoreboardScore display name missing.");
                }
                if (reader.ReadBoolean())
                {
                    int format = reader.ReadVarInt();
                    fields.NumberFormat = format;
                    if (format is 1 or 2)
                    {
                        fields.Styling = reader.ReadAnonymousNbtTag(protocolVersion)
                            ?? throw new InvalidOperationException("ScoreboardScore styling missing.");
                    }
                }
                V765_Last = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.ScoreboardScore), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_764Fields
    {
        public string ItemName { get; set; }
        public int Action { get; set; }
        public string ScoreName { get; set; }
        public int? Value { get; set; }
    }

    public struct V765_LastFields
    {
        public string ItemName { get; set; }
        public string ScoreName { get; set; }
        public int Value { get; set; }
        public NbtTag? DisplayName { get; set; }
        public int? NumberFormat { get; set; }
        public NbtTag? Styling { get; set; }
    }
}
