using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("DeclareCommands", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class DeclareCommandsPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol),
    };

    public CommandNodeEntry[] Nodes { get; set; } = Array.Empty<CommandNodeEntry>();
    public int RootIndex { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(Nodes.Length);
                for (int i = 0; i < Nodes.Length; i++)
                {
                    WriteCommandNode(writer, Nodes[i], protocolVersion);
                }
                writer.WriteVarInt(RootIndex);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.DeclareCommands), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
            {
                int count = reader.ReadVarInt();
                var nodes = new CommandNodeEntry[count];
                for (int i = 0; i < nodes.Length; i++)
                {
                    nodes[i] = ReadCommandNode(ref reader, protocolVersion);
                }
                Nodes = nodes;
                RootIndex = reader.ReadVarInt();
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.DeclareCommands), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    private static CommandNodeEntry ReadCommandNode(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        byte flagsRaw = reader.ReadUnsignedByte();
        CommandNodeType nodeType = (CommandNodeType)(flagsRaw & 0x03);
        bool hasCommand = (flagsRaw & 0x04) != 0;
        bool hasRedirect = (flagsRaw & 0x08) != 0;
        bool hasCustomSuggestions = (flagsRaw & 0x10) != 0;

        int[] children = reader.ReadArray<int, VarIntArrayReader>(LengthFormat.VarInt);
        int? redirectNode = hasRedirect ? reader.ReadVarInt() : null;

        string? name = null;
        string? parser = null;
        object? properties = null;
        string? suggestionType = null;

        if (nodeType == CommandNodeType.Literal || nodeType == CommandNodeType.Argument)
        {
            name = reader.ReadString();
        }

        if (nodeType == CommandNodeType.Argument)
        {
            parser = ReadCommandParser(ref reader, protocolVersion);
            properties = ReadCommandNodeProperties(ref reader, parser, protocolVersion);
            if (hasCustomSuggestions)
            {
                suggestionType = reader.ReadString();
            }
        }

        return new CommandNodeEntry
        {
            NodeType = nodeType,
            HasCommand = hasCommand,
            Children = children,
            RedirectNode = redirectNode,
            Name = name,
            Parser = parser,
            Properties = properties,
            SuggestionType = suggestionType
        };
    }

    private static void WriteCommandNode(MinecraftPrimitiveWriter writer, CommandNodeEntry node, int protocolVersion)
    {
        byte flagsRaw = (byte)((int)node.NodeType & 0x03);
        if (node.HasCommand) flagsRaw |= 0x04;
        if (node.RedirectNode.HasValue) flagsRaw |= 0x08;
        if (node.SuggestionType is not null) flagsRaw |= 0x10;
        writer.WriteUnsignedByte(flagsRaw);

        writer.WriteVarInt(node.Children.Length);
        for (int i = 0; i < node.Children.Length; i++)
        {
            writer.WriteVarInt(node.Children[i]);
        }
        if (node.RedirectNode.HasValue)
        {
            writer.WriteVarInt(node.RedirectNode.Value);
        }

        if (node.NodeType == CommandNodeType.Literal || node.NodeType == CommandNodeType.Argument)
        {
            writer.WriteString(node.Name ?? string.Empty);
        }

        if (node.NodeType == CommandNodeType.Argument)
        {
            WriteCommandParser(writer, node.Parser ?? string.Empty, protocolVersion);
            WriteCommandNodeProperties(writer, node.Parser ?? string.Empty, node.Properties, protocolVersion);
            if (node.SuggestionType is not null)
            {
                writer.WriteString(node.SuggestionType);
            }
        }
    }

    private static string ReadCommandParser(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        if (protocolVersion <= 758)
        {
            return reader.ReadString();
        }

        int id = reader.ReadVarInt();
        string[] mapping = GetCommandParserMapping(protocolVersion);
        if ((uint)id >= (uint)mapping.Length)
        {
            throw new InvalidOperationException($"Unknown command parser id {id}.");
        }

        return mapping[id];
    }

    private static void WriteCommandParser(MinecraftPrimitiveWriter writer, string parser, int protocolVersion)
    {
        if (protocolVersion <= 758)
        {
            writer.WriteString(parser);
            return;
        }

        int id = GetCommandParserId(protocolVersion, parser);
        writer.WriteVarInt(id);
    }

    private static object? ReadCommandNodeProperties(ref MinecraftPrimitiveReader reader, string parser, int protocolVersion)
    {
        return parser switch
        {
            "brigadier:float" => ReadFloatProperties(ref reader),
            "brigadier:double" => ReadDoubleProperties(ref reader),
            "brigadier:integer" => ReadIntProperties(ref reader),
            "brigadier:long" => ReadLongProperties(ref reader),
            "brigadier:string" => new BrigadierStringProperties(ReadStringType(ref reader)),
            "minecraft:entity" => ReadEntityProperties(ref reader),
            "minecraft:score_holder" => ReadScoreHolderProperties(ref reader),
            "minecraft:range" when protocolVersion <= 758 => new RangeProperties(reader.ReadBoolean()),
            "minecraft:resource_or_tag" => new ResourceProperties(reader.ReadString()),
            "minecraft:resource" => new ResourceProperties(reader.ReadString()),
            "minecraft:resource_or_tag_key" => new ResourceProperties(reader.ReadString()),
            "minecraft:resource_key" => new ResourceProperties(reader.ReadString()),
            "minecraft:time" when protocolVersion >= 762 => new TimeProperties(reader.ReadSignedInt()),
            _ => null
        };
    }

    private static void WriteCommandNodeProperties(MinecraftPrimitiveWriter writer, string parser, object? value,
        int protocolVersion)
    {
        switch (parser)
        {
            case "brigadier:float":
                WriteFloatProperties(writer, Expect<BrigadierFloatProperties>(parser, value));
                return;
            case "brigadier:double":
                WriteDoubleProperties(writer, Expect<BrigadierDoubleProperties>(parser, value));
                return;
            case "brigadier:integer":
                WriteIntProperties(writer, Expect<BrigadierIntProperties>(parser, value));
                return;
            case "brigadier:long":
                WriteLongProperties(writer, Expect<BrigadierLongProperties>(parser, value));
                return;
            case "brigadier:string":
                writer.WriteVarInt((int)Expect<BrigadierStringProperties>(parser, value).Type);
                return;
            case "minecraft:entity":
                WriteEntityProperties(writer, Expect<EntityProperties>(parser, value));
                return;
            case "minecraft:score_holder":
                WriteScoreHolderProperties(writer, Expect<ScoreHolderProperties>(parser, value));
                return;
            case "minecraft:range" when protocolVersion <= 758:
                writer.WriteBoolean(Expect<RangeProperties>(parser, value).AllowDecimals);
                return;
            case "minecraft:resource_or_tag":
            case "minecraft:resource":
            case "minecraft:resource_or_tag_key":
            case "minecraft:resource_key":
                writer.WriteString(Expect<ResourceProperties>(parser, value).Registry);
                return;
            case "minecraft:time" when protocolVersion >= 762:
                writer.WriteSignedInt(Expect<TimeProperties>(parser, value).Min);
                return;
            default:
                return;
        }
    }

    private static BrigadierFloatProperties ReadFloatProperties(ref MinecraftPrimitiveReader reader)
    {
        byte flags = reader.ReadUnsignedByte();
        bool minPresent = (flags & 0x01) != 0;
        bool maxPresent = (flags & 0x02) != 0;
        float? min = minPresent ? reader.ReadFloat() : null;
        float? max = maxPresent ? reader.ReadFloat() : null;
        return new BrigadierFloatProperties(min, max);
    }

    private static BrigadierDoubleProperties ReadDoubleProperties(ref MinecraftPrimitiveReader reader)
    {
        byte flags = reader.ReadUnsignedByte();
        bool minPresent = (flags & 0x01) != 0;
        bool maxPresent = (flags & 0x02) != 0;
        double? min = minPresent ? reader.ReadDouble() : null;
        double? max = maxPresent ? reader.ReadDouble() : null;
        return new BrigadierDoubleProperties(min, max);
    }

    private static BrigadierIntProperties ReadIntProperties(ref MinecraftPrimitiveReader reader)
    {
        byte flags = reader.ReadUnsignedByte();
        bool minPresent = (flags & 0x01) != 0;
        bool maxPresent = (flags & 0x02) != 0;
        int? min = minPresent ? reader.ReadSignedInt() : null;
        int? max = maxPresent ? reader.ReadSignedInt() : null;
        return new BrigadierIntProperties(min, max);
    }

    private static BrigadierLongProperties ReadLongProperties(ref MinecraftPrimitiveReader reader)
    {
        byte flags = reader.ReadUnsignedByte();
        bool minPresent = (flags & 0x01) != 0;
        bool maxPresent = (flags & 0x02) != 0;
        long? min = minPresent ? reader.ReadSignedLong() : null;
        long? max = maxPresent ? reader.ReadSignedLong() : null;
        return new BrigadierLongProperties(min, max);
    }

    private static BrigadierStringType ReadStringType(ref MinecraftPrimitiveReader reader)
    {
        int value = reader.ReadVarInt();
        return value switch
        {
            0 => BrigadierStringType.SingleWord,
            1 => BrigadierStringType.QuotablePhrase,
            2 => BrigadierStringType.GreedyPhrase,
            _ => throw new InvalidOperationException($"Unknown brigadier string type {value}.")
        };
    }

    private static EntityProperties ReadEntityProperties(ref MinecraftPrimitiveReader reader)
    {
        byte flags = reader.ReadUnsignedByte();
        bool onlyAllowEntities = (flags & 0x01) != 0;
        bool onlyAllowPlayers = (flags & 0x02) != 0;
        return new EntityProperties(onlyAllowPlayers, onlyAllowEntities);
    }

    private static ScoreHolderProperties ReadScoreHolderProperties(ref MinecraftPrimitiveReader reader)
    {
        byte flags = reader.ReadUnsignedByte();
        bool allowMultiple = (flags & 0x01) != 0;
        return new ScoreHolderProperties(allowMultiple);
    }

    private static void WriteFloatProperties(MinecraftPrimitiveWriter writer, BrigadierFloatProperties props)
    {
        byte flags = 0;
        if (props.Min.HasValue) flags |= 0x01;
        if (props.Max.HasValue) flags |= 0x02;
        writer.WriteUnsignedByte(flags);
        if (props.Min.HasValue) writer.WriteFloat(props.Min.Value);
        if (props.Max.HasValue) writer.WriteFloat(props.Max.Value);
    }

    private static void WriteDoubleProperties(MinecraftPrimitiveWriter writer, BrigadierDoubleProperties props)
    {
        byte flags = 0;
        if (props.Min.HasValue) flags |= 0x01;
        if (props.Max.HasValue) flags |= 0x02;
        writer.WriteUnsignedByte(flags);
        if (props.Min.HasValue) writer.WriteDouble(props.Min.Value);
        if (props.Max.HasValue) writer.WriteDouble(props.Max.Value);
    }

    private static void WriteIntProperties(MinecraftPrimitiveWriter writer, BrigadierIntProperties props)
    {
        byte flags = 0;
        if (props.Min.HasValue) flags |= 0x01;
        if (props.Max.HasValue) flags |= 0x02;
        writer.WriteUnsignedByte(flags);
        if (props.Min.HasValue) writer.WriteSignedInt(props.Min.Value);
        if (props.Max.HasValue) writer.WriteSignedInt(props.Max.Value);
    }

    private static void WriteLongProperties(MinecraftPrimitiveWriter writer, BrigadierLongProperties props)
    {
        byte flags = 0;
        if (props.Min.HasValue) flags |= 0x01;
        if (props.Max.HasValue) flags |= 0x02;
        writer.WriteUnsignedByte(flags);
        if (props.Min.HasValue) writer.WriteSignedLong(props.Min.Value);
        if (props.Max.HasValue) writer.WriteSignedLong(props.Max.Value);
    }

    private static void WriteEntityProperties(MinecraftPrimitiveWriter writer, EntityProperties props)
    {
        byte flags = 0;
        if (props.OnlyAllowEntities) flags |= 0x01;
        if (props.OnlyAllowPlayers) flags |= 0x02;
        writer.WriteUnsignedByte(flags);
    }

    private static void WriteScoreHolderProperties(MinecraftPrimitiveWriter writer, ScoreHolderProperties props)
    {
        byte flags = 0;
        if (props.AllowMultiple) flags |= 0x01;
        writer.WriteUnsignedByte(flags);
    }

    private static T Expect<T>(string parser, object? value) where T : struct
    {
        if (value is T typed)
        {
            return typed;
        }

        throw new InvalidOperationException($"Missing properties for parser {parser}.");
    }

    private static string[] GetCommandParserMapping(int protocolVersion)
    {
        return protocolVersion switch
        {
            <= 760 => ParserMapping759_760,
            761 => ParserMapping761,
            >= 762 and <= 764 => ParserMapping762_764,
            765 => ParserMapping765,
            _ => ParserMapping766_769
        };
    }

    private static int GetCommandParserId(int protocolVersion, string name)
    {
        string[] mapping = GetCommandParserMapping(protocolVersion);
        int id = Array.IndexOf(mapping, name);
        if (id < 0)
        {
            throw new InvalidOperationException($"Unknown command parser name {name}.");
        }

        return id;
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct CommandNodeEntry
    {
        public CommandNodeType NodeType { get; set; }
        public bool HasCommand { get; set; }
        public int[] Children { get; set; }
        public int? RedirectNode { get; set; }
        public string? Name { get; set; }
        public string? Parser { get; set; }
        public object? Properties { get; set; }
        public string? SuggestionType { get; set; }
    }

    public enum CommandNodeType : byte
    {
        Root = 0,
        Literal = 1,
        Argument = 2
    }

    public enum BrigadierStringType
    {
        SingleWord = 0,
        QuotablePhrase = 1,
        GreedyPhrase = 2
    }

    public readonly record struct BrigadierFloatProperties(float? Min, float? Max);
    public readonly record struct BrigadierDoubleProperties(double? Min, double? Max);
    public readonly record struct BrigadierIntProperties(int? Min, int? Max);
    public readonly record struct BrigadierLongProperties(long? Min, long? Max);
    public readonly record struct BrigadierStringProperties(BrigadierStringType Type);
    public readonly record struct EntityProperties(bool OnlyAllowPlayers, bool OnlyAllowEntities);
    public readonly record struct ScoreHolderProperties(bool AllowMultiple);
    public readonly record struct RangeProperties(bool AllowDecimals);
    public readonly record struct ResourceProperties(string Registry);
    public readonly record struct TimeProperties(int Min);

    private static readonly string[] ParserMapping759_760 =
    {
        "brigadier:bool", "brigadier:float", "brigadier:double", "brigadier:integer", "brigadier:long", "brigadier:string",
        "minecraft:entity", "minecraft:game_profile", "minecraft:block_pos", "minecraft:column_pos", "minecraft:vec3",
        "minecraft:vec2", "minecraft:block_state", "minecraft:block_predicate", "minecraft:item_stack",
        "minecraft:item_predicate", "minecraft:color", "minecraft:component", "minecraft:message", "minecraft:nbt",
        "minecraft:nbt_tag", "minecraft:nbt_path", "minecraft:objective", "minecraft:objective_criteria",
        "minecraft:operation", "minecraft:particle", "minecraft:angle", "minecraft:rotation", "minecraft:scoreboard_slot",
        "minecraft:score_holder", "minecraft:swizzle", "minecraft:team", "minecraft:item_slot", "minecraft:resource_location",
        "minecraft:mob_effect", "minecraft:function", "minecraft:entity_anchor", "minecraft:int_range",
        "minecraft:float_range", "minecraft:item_enchantment", "minecraft:entity_summon", "minecraft:dimension",
        "minecraft:time", "minecraft:resource_or_tag", "minecraft:resource", "minecraft:template_mirror",
        "minecraft:template_rotation", "minecraft:uuid"
    };

    private static readonly string[] ParserMapping761 =
    {
        "brigadier:bool", "brigadier:float", "brigadier:double", "brigadier:integer", "brigadier:long", "brigadier:string",
        "minecraft:entity", "minecraft:game_profile", "minecraft:block_pos", "minecraft:column_pos", "minecraft:vec3",
        "minecraft:vec2", "minecraft:block_state", "minecraft:block_predicate", "minecraft:item_stack",
        "minecraft:item_predicate", "minecraft:color", "minecraft:component", "minecraft:message", "minecraft:nbt",
        "minecraft:nbt_tag", "minecraft:nbt_path", "minecraft:objective", "minecraft:objective_criteria",
        "minecraft:operation", "minecraft:particle", "minecraft:angle", "minecraft:rotation", "minecraft:scoreboard_slot",
        "minecraft:score_holder", "minecraft:swizzle", "minecraft:team", "minecraft:item_slot", "minecraft:resource_location",
        "minecraft:function", "minecraft:entity_anchor", "minecraft:int_range", "minecraft:float_range",
        "minecraft:dimension", "minecraft:gamemode", "minecraft:time", "minecraft:resource_or_tag",
        "minecraft:resource_or_tag_key", "minecraft:resource", "minecraft:resource_key", "minecraft:template_mirror",
        "minecraft:template_rotation", "minecraft:uuid"
    };

    private static readonly string[] ParserMapping762_764 =
    {
        "brigadier:bool", "brigadier:float", "brigadier:double", "brigadier:integer", "brigadier:long", "brigadier:string",
        "minecraft:entity", "minecraft:game_profile", "minecraft:block_pos", "minecraft:column_pos", "minecraft:vec3",
        "minecraft:vec2", "minecraft:block_state", "minecraft:block_predicate", "minecraft:item_stack",
        "minecraft:item_predicate", "minecraft:color", "minecraft:component", "minecraft:message", "minecraft:nbt",
        "minecraft:nbt_tag", "minecraft:nbt_path", "minecraft:objective", "minecraft:objective_criteria",
        "minecraft:operation", "minecraft:particle", "minecraft:angle", "minecraft:rotation", "minecraft:scoreboard_slot",
        "minecraft:score_holder", "minecraft:swizzle", "minecraft:team", "minecraft:item_slot", "minecraft:resource_location",
        "minecraft:function", "minecraft:entity_anchor", "minecraft:int_range", "minecraft:float_range",
        "minecraft:dimension", "minecraft:gamemode", "minecraft:time", "minecraft:resource_or_tag",
        "minecraft:resource_or_tag_key", "minecraft:resource", "minecraft:resource_key", "minecraft:template_mirror",
        "minecraft:template_rotation", "minecraft:heightmap", "minecraft:uuid"
    };

    private static readonly string[] ParserMapping765 =
    {
        "brigadier:bool", "brigadier:float", "brigadier:double", "brigadier:integer", "brigadier:long", "brigadier:string",
        "minecraft:entity", "minecraft:game_profile", "minecraft:block_pos", "minecraft:column_pos", "minecraft:vec3",
        "minecraft:vec2", "minecraft:block_state", "minecraft:block_predicate", "minecraft:item_stack",
        "minecraft:item_predicate", "minecraft:color", "minecraft:component", "minecraft:style", "minecraft:message",
        "minecraft:nbt", "minecraft:nbt_tag", "minecraft:nbt_path", "minecraft:objective", "minecraft:objective_criteria",
        "minecraft:operation", "minecraft:particle", "minecraft:angle", "minecraft:rotation", "minecraft:scoreboard_slot",
        "minecraft:score_holder", "minecraft:swizzle", "minecraft:team", "minecraft:item_slot", "minecraft:resource_location",
        "minecraft:function", "minecraft:entity_anchor", "minecraft:int_range", "minecraft:float_range",
        "minecraft:dimension", "minecraft:gamemode", "minecraft:time", "minecraft:resource_or_tag",
        "minecraft:resource_or_tag_key", "minecraft:resource", "minecraft:resource_key", "minecraft:template_mirror",
        "minecraft:template_rotation", "minecraft:heightmap", "minecraft:uuid"
    };

    private static readonly string[] ParserMapping766_769 =
    {
        "brigadier:bool", "brigadier:float", "brigadier:double", "brigadier:integer", "brigadier:long", "brigadier:string",
        "minecraft:entity", "minecraft:game_profile", "minecraft:block_pos", "minecraft:column_pos", "minecraft:vec3",
        "minecraft:vec2", "minecraft:block_state", "minecraft:block_predicate", "minecraft:item_stack",
        "minecraft:item_predicate", "minecraft:color", "minecraft:component", "minecraft:style", "minecraft:message",
        "minecraft:nbt", "minecraft:nbt_tag", "minecraft:nbt_path", "minecraft:objective", "minecraft:objective_criteria",
        "minecraft:operation", "minecraft:particle", "minecraft:angle", "minecraft:rotation", "minecraft:scoreboard_slot",
        "minecraft:score_holder", "minecraft:swizzle", "minecraft:team", "minecraft:item_slot", "minecraft:item_slots",
        "minecraft:resource_location", "minecraft:function", "minecraft:entity_anchor", "minecraft:int_range",
        "minecraft:float_range", "minecraft:dimension", "minecraft:gamemode", "minecraft:time", "minecraft:resource_or_tag",
        "minecraft:resource_or_tag_key", "minecraft:resource", "minecraft:resource_key", "minecraft:template_mirror",
        "minecraft:template_rotation", "minecraft:heightmap", "minecraft:loot_table", "minecraft:loot_predicate",
        "minecraft:loot_modifier", "minecraft:uuid"
    };
}
