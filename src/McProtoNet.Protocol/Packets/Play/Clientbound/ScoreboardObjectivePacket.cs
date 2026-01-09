using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("ScoreboardObjective", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class ScoreboardObjectivePacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 764),
        new(765, MinecraftVersion.LatestProtocol),
    };

    public string Name { get; set; } = string.Empty;
    public sbyte Action { get; set; }

    public VFirst_764Fields? VFirst_764 { get; set; }
    public V765_LastFields? V765_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
            {
                var fields = VFirst_764 ?? throw new InvalidOperationException("ScoreboardObjective VFirst_764 fields missing.");
                writer.WriteString(Name);
                writer.WriteSignedByte(Action);
                if (Action == 0 || Action == 2)
                {
                    writer.WriteString(fields.DisplayText ?? throw new InvalidOperationException("ScoreboardObjective displayText missing."));
                    writer.WriteVarInt(fields.Type ?? throw new InvalidOperationException("ScoreboardObjective type missing."));
                }
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V765_Last ?? throw new InvalidOperationException("ScoreboardObjective V765_Last fields missing.");
                writer.WriteString(Name);
                writer.WriteSignedByte(Action);
                if (Action == 0 || Action == 2)
                {
                    writer.WriteAnonymousNbtTag(fields.DisplayText ?? throw new InvalidOperationException("ScoreboardObjective displayText missing."), protocolVersion);
                    writer.WriteVarInt(fields.Type ?? throw new InvalidOperationException("ScoreboardObjective type missing."));
                    if (fields.NumberFormat.HasValue)
                    {
                        writer.WriteBoolean(true);
                        writer.WriteVarInt(fields.NumberFormat.Value);
                        if (fields.NumberFormat.Value is 1 or 2)
                        {
                            writer.WriteAnonymousNbtTag(fields.Styling ?? throw new InvalidOperationException("ScoreboardObjective styling missing."), protocolVersion);
                        }
                    }
                    else
                    {
                        writer.WriteBoolean(false);
                    }
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.ScoreboardObjective), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
            {
                var fields = new VFirst_764Fields();
                Name = reader.ReadString();
                Action = reader.ReadSignedByte();
                if (Action == 0 || Action == 2)
                {
                    fields.DisplayText = reader.ReadString();
                    fields.Type = reader.ReadVarInt();
                }
                VFirst_764 = fields;
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = new V765_LastFields();
                Name = reader.ReadString();
                Action = reader.ReadSignedByte();
                if (Action == 0 || Action == 2)
                {
                    fields.DisplayText = reader.ReadAnonymousNbtTag(protocolVersion)
                        ?? throw new InvalidOperationException("ScoreboardObjective displayText missing.");
                    fields.Type = reader.ReadVarInt();
                    if (reader.ReadBoolean())
                    {
                        int format = reader.ReadVarInt();
                        fields.NumberFormat = format;
                        if (format is 1 or 2)
                        {
                            fields.Styling = reader.ReadAnonymousNbtTag(protocolVersion)
                                ?? throw new InvalidOperationException("ScoreboardObjective styling missing.");
                        }
                    }
                }
                V765_Last = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.ScoreboardObjective), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_764Fields
    {
        public string? DisplayText { get; set; }
        public int? Type { get; set; }
    }

    public struct V765_LastFields
    {
        public NbtTag? DisplayText { get; set; }
        public int? Type { get; set; }
        public int? NumberFormat { get; set; }
        public NbtTag? Styling { get; set; }
    }
}
