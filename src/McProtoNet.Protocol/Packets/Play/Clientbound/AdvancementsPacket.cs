using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("Advancements", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class AdvancementsPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 762),
        new(763, 763),
        new(764, 764),
        new(765, MinecraftVersion.LatestProtocol),
    };

    public VFirst_762Fields? VFirst_762 { get; set; }
    public V763Fields? V763 { get; set; }
    public V764Fields? V764 { get; set; }
    public V765_LastFields? V765_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 762:
            {
                var fields = VFirst_762 ?? throw new InvalidOperationException("Advancements VFirst_762 fields missing.");
                writer.WriteBoolean(fields.Reset);
                WriteAdvancementMapping(ref writer, fields.AdvancementMapping, includeCriteria: true, includeTelemetry: false,
                    protocolVersion);
                WriteStringArray(ref writer, fields.Identifiers);
                WriteProgressMapping(ref writer, fields.ProgressMapping);
                return;
            }
            case 763:
            {
                var fields = V763 ?? throw new InvalidOperationException("Advancements V763 fields missing.");
                writer.WriteBoolean(fields.Reset);
                WriteAdvancementMapping(ref writer, fields.AdvancementMapping, includeCriteria: true, includeTelemetry: true,
                    protocolVersion);
                WriteStringArray(ref writer, fields.Identifiers);
                WriteProgressMapping(ref writer, fields.ProgressMapping);
                return;
            }
            case 764:
            {
                var fields = V764 ?? throw new InvalidOperationException("Advancements V764 fields missing.");
                writer.WriteBoolean(fields.Reset);
                WriteAdvancementMapping(ref writer, fields.AdvancementMapping, includeCriteria: false, includeTelemetry: true,
                    protocolVersion);
                WriteStringArray(ref writer, fields.Identifiers);
                WriteProgressMapping(ref writer, fields.ProgressMapping);
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V765_Last ?? throw new InvalidOperationException("Advancements V765_Last fields missing.");
                writer.WriteBoolean(fields.Reset);
                WriteAdvancementMappingNbt(ref writer, fields.AdvancementMapping, includeTelemetry: true, protocolVersion);
                WriteStringArray(ref writer, fields.Identifiers);
                WriteProgressMapping(ref writer, fields.ProgressMapping);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.Advancements), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 762:
            {
                VFirst_762 = new VFirst_762Fields
                {
                    Reset = reader.ReadBoolean(),
                    AdvancementMapping = ReadAdvancementMapping(ref reader, includeCriteria: true, includeTelemetry: false,
                        protocolVersion),
                    Identifiers = ReadStringArray(ref reader),
                    ProgressMapping = ReadProgressMapping(ref reader)
                };
                return;
            }
            case 763:
            {
                V763 = new V763Fields
                {
                    Reset = reader.ReadBoolean(),
                    AdvancementMapping = ReadAdvancementMapping(ref reader, includeCriteria: true, includeTelemetry: true,
                        protocolVersion),
                    Identifiers = ReadStringArray(ref reader),
                    ProgressMapping = ReadProgressMapping(ref reader)
                };
                return;
            }
            case 764:
            {
                V764 = new V764Fields
                {
                    Reset = reader.ReadBoolean(),
                    AdvancementMapping = ReadAdvancementMapping(ref reader, includeCriteria: false, includeTelemetry: true,
                        protocolVersion),
                    Identifiers = ReadStringArray(ref reader),
                    ProgressMapping = ReadProgressMapping(ref reader)
                };
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                V765_Last = new V765_LastFields
                {
                    Reset = reader.ReadBoolean(),
                    AdvancementMapping = ReadAdvancementMappingNbt(ref reader, includeTelemetry: true, protocolVersion),
                    Identifiers = ReadStringArray(ref reader),
                    ProgressMapping = ReadProgressMapping(ref reader)
                };
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.Advancements), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    private static AdvancementMappingEntry[] ReadAdvancementMapping(ref MinecraftPrimitiveReader reader,
        bool includeCriteria, bool includeTelemetry, int protocolVersion)
    {
        int count = reader.ReadVarInt();
        if (count == 0)
        {
            return Array.Empty<AdvancementMappingEntry>();
        }

        var entries = new AdvancementMappingEntry[count];
        for (int i = 0; i < entries.Length; i++)
        {
            string key = reader.ReadString();
            entries[i] = new AdvancementMappingEntry
            {
                Key = key,
                Value = ReadAdvancementDefinition(ref reader, includeCriteria, includeTelemetry, protocolVersion)
            };
        }

        return entries;
    }

    private static AdvancementMappingEntryNbt[] ReadAdvancementMappingNbt(ref MinecraftPrimitiveReader reader,
        bool includeTelemetry, int protocolVersion)
    {
        int count = reader.ReadVarInt();
        if (count == 0)
        {
            return Array.Empty<AdvancementMappingEntryNbt>();
        }

        var entries = new AdvancementMappingEntryNbt[count];
        for (int i = 0; i < entries.Length; i++)
        {
            string key = reader.ReadString();
            entries[i] = new AdvancementMappingEntryNbt
            {
                Key = key,
                Value = ReadAdvancementDefinitionNbt(ref reader, includeTelemetry, protocolVersion)
            };
        }

        return entries;
    }

    private static AdvancementDefinition ReadAdvancementDefinition(ref MinecraftPrimitiveReader reader,
        bool includeCriteria, bool includeTelemetry, int protocolVersion)
    {
        string? parentId = reader.ReadOptional(ReadDelegates.String);
        AdvancementDisplayData? displayData = reader.ReadBoolean()
            ? ReadAdvancementDisplayData(ref reader, protocolVersion)
            : null;
        string[] criteria = includeCriteria ? ReadCriteria(ref reader) : Array.Empty<string>();
        string[][] requirements = ReadRequirements(ref reader);
        bool sendsTelemetryData = includeTelemetry && reader.ReadBoolean();

        return new AdvancementDefinition
        {
            ParentId = parentId,
            DisplayData = displayData,
            Criteria = criteria,
            Requirements = requirements,
            SendsTelemetryData = sendsTelemetryData
        };
    }

    private static AdvancementDefinitionNbt ReadAdvancementDefinitionNbt(ref MinecraftPrimitiveReader reader,
        bool includeTelemetry, int protocolVersion)
    {
        string? parentId = reader.ReadOptional(ReadDelegates.String);
        AdvancementDisplayDataNbt? displayData = reader.ReadBoolean()
            ? ReadAdvancementDisplayDataNbt(ref reader, protocolVersion)
            : null;
        string[][] requirements = ReadRequirements(ref reader);
        bool sendsTelemetryData = includeTelemetry && reader.ReadBoolean();

        return new AdvancementDefinitionNbt
        {
            ParentId = parentId,
            DisplayData = displayData,
            Requirements = requirements,
            SendsTelemetryData = sendsTelemetryData
        };
    }

    private static AdvancementDisplayData ReadAdvancementDisplayData(ref MinecraftPrimitiveReader reader,
        int protocolVersion)
    {
        string title = reader.ReadString();
        string description = reader.ReadString();
        Slot icon = reader.ReadSlot(protocolVersion);
        int frameType = reader.ReadVarInt();
        int flagsRaw = reader.ReadSignedInt();
        var flags = new AdvancementDisplayFlags
        {
            Hidden = (flagsRaw & 0x04) != 0,
            ShowToast = (flagsRaw & 0x02) != 0,
            HasBackgroundTexture = (flagsRaw & 0x01) != 0
        };
        string? backgroundTexture = flags.HasBackgroundTexture ? reader.ReadString() : null;
        float xCord = reader.ReadFloat();
        float yCord = reader.ReadFloat();

        return new AdvancementDisplayData
        {
            Title = title,
            Description = description,
            Icon = icon,
            FrameType = frameType,
            Flags = flags,
            BackgroundTexture = backgroundTexture,
            X = xCord,
            Y = yCord
        };
    }

    private static AdvancementDisplayDataNbt ReadAdvancementDisplayDataNbt(ref MinecraftPrimitiveReader reader,
        int protocolVersion)
    {
        NbtTag title = reader.ReadAnonymousNbtTag(protocolVersion) ?? throw new InvalidOperationException("title missing");
        NbtTag description = reader.ReadAnonymousNbtTag(protocolVersion) ?? throw new InvalidOperationException("description missing");
        Slot icon = reader.ReadSlot(protocolVersion);
        int frameType = reader.ReadVarInt();
        int flagsRaw = reader.ReadSignedInt();
        var flags = new AdvancementDisplayFlags
        {
            Hidden = (flagsRaw & 0x04) != 0,
            ShowToast = (flagsRaw & 0x02) != 0,
            HasBackgroundTexture = (flagsRaw & 0x01) != 0
        };
        string? backgroundTexture = flags.HasBackgroundTexture ? reader.ReadString() : null;
        float xCord = reader.ReadFloat();
        float yCord = reader.ReadFloat();

        return new AdvancementDisplayDataNbt
        {
            Title = title,
            Description = description,
            Icon = icon,
            FrameType = frameType,
            Flags = flags,
            BackgroundTexture = backgroundTexture,
            X = xCord,
            Y = yCord
        };
    }

    private static string[] ReadCriteria(ref MinecraftPrimitiveReader reader)
    {
        int count = reader.ReadVarInt();
        if (count == 0)
        {
            return Array.Empty<string>();
        }

        var criteria = new string[count];
        for (int i = 0; i < criteria.Length; i++)
        {
            criteria[i] = reader.ReadString();
        }

        return criteria;
    }

    private static string[][] ReadRequirements(ref MinecraftPrimitiveReader reader)
    {
        int count = reader.ReadVarInt();
        if (count == 0)
        {
            return Array.Empty<string[]>();
        }

        var requirements = new string[count][];
        for (int i = 0; i < requirements.Length; i++)
        {
            int innerCount = reader.ReadVarInt();
            var requirement = new string[innerCount];
            for (int j = 0; j < requirement.Length; j++)
            {
                requirement[j] = reader.ReadString();
            }
            requirements[i] = requirement;
        }

        return requirements;
    }

    private static ProgressMappingEntry[] ReadProgressMapping(ref MinecraftPrimitiveReader reader)
    {
        int count = reader.ReadVarInt();
        if (count == 0)
        {
            return Array.Empty<ProgressMappingEntry>();
        }

        var entries = new ProgressMappingEntry[count];
        for (int i = 0; i < entries.Length; i++)
        {
            string key = reader.ReadString();
            int progressCount = reader.ReadVarInt();
            var progressEntries = new CriterionProgressEntry[progressCount];
            for (int j = 0; j < progressEntries.Length; j++)
            {
                string criterionIdentifier = reader.ReadString();
                long? criterionProgress = reader.ReadOptional(ReadDelegates.Int64);
                progressEntries[j] = new CriterionProgressEntry
                {
                    CriterionIdentifier = criterionIdentifier,
                    CriterionProgress = criterionProgress
                };
            }

            entries[i] = new ProgressMappingEntry
            {
                Key = key,
                Value = progressEntries
            };
        }

        return entries;
    }

    private static void WriteAdvancementMapping(ref MinecraftPrimitiveWriter writer, AdvancementMappingEntry[] entries,
        bool includeCriteria, bool includeTelemetry, int protocolVersion)
    {
        writer.WriteVarInt(entries.Length);
        for (int i = 0; i < entries.Length; i++)
        {
            writer.WriteString(entries[i].Key);
            WriteAdvancementDefinition(ref writer, entries[i].Value, includeCriteria, includeTelemetry,
                protocolVersion);
        }
    }

    private static void WriteAdvancementMappingNbt(ref MinecraftPrimitiveWriter writer, AdvancementMappingEntryNbt[] entries,
        bool includeTelemetry, int protocolVersion)
    {
        writer.WriteVarInt(entries.Length);
        for (int i = 0; i < entries.Length; i++)
        {
            writer.WriteString(entries[i].Key);
            WriteAdvancementDefinitionNbt(ref writer, entries[i].Value, includeTelemetry, protocolVersion);
        }
    }

    private static void WriteAdvancementDefinition(ref MinecraftPrimitiveWriter writer, AdvancementDefinition value,
        bool includeCriteria, bool includeTelemetry, int protocolVersion)
    {
        if (value.ParentId is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            writer.WriteString(value.ParentId);
        }

        if (value.DisplayData is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            WriteAdvancementDisplayData(ref writer, value.DisplayData.Value, protocolVersion);
        }

        if (includeCriteria)
        {
            WriteStringArray(ref writer, value.Criteria);
        }

        WriteRequirements(ref writer, value.Requirements);
        if (includeTelemetry)
        {
            writer.WriteBoolean(value.SendsTelemetryData);
        }
    }

    private static void WriteAdvancementDefinitionNbt(ref MinecraftPrimitiveWriter writer, AdvancementDefinitionNbt value,
        bool includeTelemetry, int protocolVersion)
    {
        if (value.ParentId is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            writer.WriteString(value.ParentId);
        }

        if (value.DisplayData is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            WriteAdvancementDisplayDataNbt(ref writer, value.DisplayData.Value, protocolVersion);
        }

        WriteRequirements(ref writer, value.Requirements);
        if (includeTelemetry)
        {
            writer.WriteBoolean(value.SendsTelemetryData);
        }
    }

    private static void WriteAdvancementDisplayData(ref MinecraftPrimitiveWriter writer, AdvancementDisplayData value,
        int protocolVersion)
    {
        writer.WriteString(value.Title);
        writer.WriteString(value.Description);
        writer.WriteSlot(value.Icon, protocolVersion);
        writer.WriteVarInt(value.FrameType);
        int flagsRaw = 0;
        if (value.Flags.Hidden) flagsRaw |= 0x04;
        if (value.Flags.ShowToast) flagsRaw |= 0x02;
        if (value.Flags.HasBackgroundTexture) flagsRaw |= 0x01;
        writer.WriteSignedInt(flagsRaw);
        if (value.Flags.HasBackgroundTexture)
        {
            writer.WriteString(value.BackgroundTexture ?? string.Empty);
        }
        writer.WriteFloat(value.X);
        writer.WriteFloat(value.Y);
    }

    private static void WriteAdvancementDisplayDataNbt(ref MinecraftPrimitiveWriter writer, AdvancementDisplayDataNbt value,
        int protocolVersion)
    {
        writer.WriteAnonymousNbtTag(value.Title, protocolVersion);
        writer.WriteAnonymousNbtTag(value.Description, protocolVersion);
        writer.WriteSlot(value.Icon, protocolVersion);
        writer.WriteVarInt(value.FrameType);
        int flagsRaw = 0;
        if (value.Flags.Hidden) flagsRaw |= 0x04;
        if (value.Flags.ShowToast) flagsRaw |= 0x02;
        if (value.Flags.HasBackgroundTexture) flagsRaw |= 0x01;
        writer.WriteSignedInt(flagsRaw);
        if (value.Flags.HasBackgroundTexture)
        {
            writer.WriteString(value.BackgroundTexture ?? string.Empty);
        }
        writer.WriteFloat(value.X);
        writer.WriteFloat(value.Y);
    }

    private static void WriteStringArray(ref MinecraftPrimitiveWriter writer, string[] values)
    {
        writer.WriteVarInt(values.Length);
        for (int i = 0; i < values.Length; i++)
        {
            writer.WriteString(values[i]);
        }
    }

    private static string[] ReadStringArray(ref MinecraftPrimitiveReader reader)
    {
        int count = reader.ReadVarInt();
        if (count == 0)
        {
            return Array.Empty<string>();
        }

        var values = new string[count];
        for (int i = 0; i < values.Length; i++)
        {
            values[i] = reader.ReadString();
        }

        return values;
    }

    private static void WriteRequirements(ref MinecraftPrimitiveWriter writer, string[][] requirements)
    {
        writer.WriteVarInt(requirements.Length);
        for (int i = 0; i < requirements.Length; i++)
        {
            writer.WriteVarInt(requirements[i].Length);
            for (int j = 0; j < requirements[i].Length; j++)
            {
                writer.WriteString(requirements[i][j]);
            }
        }
    }

    private static void WriteProgressMapping(ref MinecraftPrimitiveWriter writer, ProgressMappingEntry[] entries)
    {
        writer.WriteVarInt(entries.Length);
        for (int i = 0; i < entries.Length; i++)
        {
            writer.WriteString(entries[i].Key);
            writer.WriteVarInt(entries[i].Value.Length);
            for (int j = 0; j < entries[i].Value.Length; j++)
            {
                writer.WriteString(entries[i].Value[j].CriterionIdentifier);
                if (entries[i].Value[j].CriterionProgress is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteSignedLong(entries[i].Value[j].CriterionProgress.Value);
                }
            }
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_762Fields
    {
        public bool Reset { get; set; }
        public AdvancementMappingEntry[] AdvancementMapping { get; set; }
        public string[] Identifiers { get; set; }
        public ProgressMappingEntry[] ProgressMapping { get; set; }
    }

    public struct V763Fields
    {
        public bool Reset { get; set; }
        public AdvancementMappingEntry[] AdvancementMapping { get; set; }
        public string[] Identifiers { get; set; }
        public ProgressMappingEntry[] ProgressMapping { get; set; }
    }

    public struct V764Fields
    {
        public bool Reset { get; set; }
        public AdvancementMappingEntry[] AdvancementMapping { get; set; }
        public string[] Identifiers { get; set; }
        public ProgressMappingEntry[] ProgressMapping { get; set; }
    }

    public struct V765_LastFields
    {
        public bool Reset { get; set; }
        public AdvancementMappingEntryNbt[] AdvancementMapping { get; set; }
        public string[] Identifiers { get; set; }
        public ProgressMappingEntry[] ProgressMapping { get; set; }
    }

    public struct AdvancementMappingEntry
    {
        public string Key { get; set; }
        public AdvancementDefinition Value { get; set; }
    }

    public struct AdvancementMappingEntryNbt
    {
        public string Key { get; set; }
        public AdvancementDefinitionNbt Value { get; set; }
    }

    public struct AdvancementDefinition
    {
        public string? ParentId { get; set; }
        public AdvancementDisplayData? DisplayData { get; set; }
        public string[] Criteria { get; set; }
        public string[][] Requirements { get; set; }
        public bool SendsTelemetryData { get; set; }
    }

    public struct AdvancementDefinitionNbt
    {
        public string? ParentId { get; set; }
        public AdvancementDisplayDataNbt? DisplayData { get; set; }
        public string[][] Requirements { get; set; }
        public bool SendsTelemetryData { get; set; }
    }

    public struct AdvancementDisplayData
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Slot Icon { get; set; }
        public int FrameType { get; set; }
        public AdvancementDisplayFlags Flags { get; set; }
        public string? BackgroundTexture { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }

    public struct AdvancementDisplayDataNbt
    {
        public NbtTag Title { get; set; }
        public NbtTag Description { get; set; }
        public Slot Icon { get; set; }
        public int FrameType { get; set; }
        public AdvancementDisplayFlags Flags { get; set; }
        public string? BackgroundTexture { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }

    public struct AdvancementDisplayFlags
    {
        public bool HasBackgroundTexture { get; set; }
        public bool ShowToast { get; set; }
        public bool Hidden { get; set; }
    }

    public struct ProgressMappingEntry
    {
        public string Key { get; set; }
        public CriterionProgressEntry[] Value { get; set; }
    }

    public struct CriterionProgressEntry
    {
        public string CriterionIdentifier { get; set; }
        public long? CriterionProgress { get; set; }
    }
}
