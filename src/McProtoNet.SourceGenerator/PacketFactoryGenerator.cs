using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace McProtoNet.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public class PacketFactoryGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var packets = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (n, _) => n is ClassDeclarationSyntax c && c.AttributeLists.Count > 0,
                transform: static (ctx, _) => GetPacketData(ctx))
            .Where(static p => p is not null)
            .Collect();

        context.RegisterSourceOutput(packets, Generate);
    }

    private static PacketData? GetPacketData(GeneratorSyntaxContext ctx)
    {
        var decl = (ClassDeclarationSyntax)ctx.Node;
        var symbol = ctx.SemanticModel.GetDeclaredSymbol(decl) as INamedTypeSymbol;
        if (symbol is null) return null;

        AttributeData? packetInfoAttr = null;
        foreach (var attr in symbol.GetAttributes())
        {
            if (attr.AttributeClass?.Name == "PacketInfoAttribute")
            {
                packetInfoAttr = attr;
                break;
            }
        }
        if (packetInfoAttr is null) return null;
        if (packetInfoAttr.ConstructorArguments.Length < 3) return null;

        var packetName = packetInfoAttr.ConstructorArguments[0].Value as string;
        if (packetName is null) return null;
        var state     = (int)packetInfoAttr.ConstructorArguments[1].Value!;
        var direction = (int)packetInfoAttr.ConstructorArguments[2].Value!;

        var idsBuilder = ImmutableArray.CreateBuilder<PacketIdRange>();
        foreach (var attr in symbol.GetAttributes())
        {
            if (attr.AttributeClass?.Name != "PacketIdAttribute") continue;
            if (attr.ConstructorArguments.Length < 3) continue;
            var from  = (int)attr.ConstructorArguments[0].Value!;
            var to    = (int)attr.ConstructorArguments[1].Value!;
            var hexId = (int)attr.ConstructorArguments[2].Value!;
            idsBuilder.Add(new PacketIdRange(from, to, hexId));
        }

        bool hasGetPacketId = false;
        foreach (var m in symbol.GetMembers("GetPacketId"))
        {
            hasGetPacketId = true;
            break;
        }

        var ns = symbol.ContainingNamespace.IsGlobalNamespace
            ? string.Empty
            : symbol.ContainingNamespace.ToDisplayString();

        var fqn = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        return new PacketData(ns, symbol.Name, fqn, packetName, state, direction,
            idsBuilder.ToImmutable(), hasGetPacketId);
    }

    private static void Generate(SourceProductionContext ctx, ImmutableArray<PacketData?> raw)
    {
        var sorted = new List<PacketData>(raw.Length);
        foreach (var p in raw)
            if (p is not null)
                sorted.Add(p);

        sorted.Sort(static (a, b) =>
        {
            int c = a.State.CompareTo(b.State);
            if (c != 0) return c;
            c = a.Direction.CompareTo(b.Direction);
            if (c != 0) return c;
            return string.Compare(a.PacketName, b.PacketName, StringComparison.Ordinal);
        });

        var packets = new List<(PacketData Data, int Order)>(sorted.Count);
        for (int i = 0; i < sorted.Count; i++)
            packets.Add((sorted[i], i));

        foreach (var (data, order) in packets)
            GeneratePacketPartial(ctx, data, order);

        GeneratePacketIdHelper(ctx, packets);
        GenerateCatalogs(ctx, packets);
    }

    private static void GeneratePacketPartial(SourceProductionContext ctx, PacketData data, int order)
    {
        var sb = new StringBuilder();

        if (!string.IsNullOrEmpty(data.Namespace))
        {
            sb.AppendLine($"namespace {data.Namespace};");
            sb.AppendLine();
        }

        sb.AppendLine($"public sealed partial class {data.ClassName}");
        sb.AppendLine("{");

        sb.Append("    public static readonly global::McProtoNet.Protocol.PacketIdentifier Identifier = ");
        sb.AppendLine($"new({order}, \"{data.PacketName}\", global::McProtoNet.Protocol.PacketState.{StateName(data.State)}, global::McProtoNet.Protocol.PacketDirection.{DirectionName(data.Direction)});");

        if (!data.HasGetPacketId)
        {
            sb.AppendLine();
            sb.AppendLine("    global::McProtoNet.Protocol.PacketIdentifier global::McProtoNet.Protocol.IPacket.GetPacketId() => Identifier;");
        }

        if (data.PacketIds.Length > 0)
        {
            sb.AppendLine();
            sb.AppendLine("    public static int GetHexId(int version)");
            sb.AppendLine("    {");
            foreach (var r in data.PacketIds)
            {
                if (r.From == r.To)
                    sb.AppendLine($"        if (version == {r.From}) return 0x{r.HexId:X2};");
                else
                    sb.AppendLine($"        if ((uint)(version - {r.From}) <= (uint)({r.To} - {r.From})) return 0x{r.HexId:X2};");
            }
            sb.AppendLine("        return -1;");
            sb.AppendLine("    }");
        }

        sb.AppendLine("}");

        ctx.AddSource($"{data.ClassName}.PacketId.g.cs", sb.ToString());
    }

    private static void GeneratePacketIdHelper(SourceProductionContext ctx, List<(PacketData Data, int Order)> packets)
    {
        var groups = new Dictionary<(int State, int Direction), List<(PacketData Data, int Order)>>();
        foreach (var p in packets)
        {
            var key = (p.Data.State, p.Data.Direction);
            if (!groups.TryGetValue(key, out var list))
                groups[key] = list = new List<(PacketData, int)>();
            list.Add(p);
        }

        var sb = new StringBuilder();
        sb.AppendLine("using System.Collections.Frozen;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using System.Runtime.CompilerServices;");
        sb.AppendLine();
        sb.AppendLine("namespace McProtoNet.Protocol;");
        sb.AppendLine();
        sb.AppendLine("public static partial class PacketIdHelper");
        sb.AppendLine("{");

        foreach (var kv in groups)
        {
            var fn = FieldName(kv.Key.State, kv.Key.Direction);
            sb.AppendLine($"    private static readonly FrozenDictionary<long, global::System.Func<IPacket>> _fwd{fn};");
            sb.AppendLine($"    private static readonly FrozenDictionary<long, PacketIdentifier> _id{fn};");
            sb.AppendLine($"    private static readonly FrozenDictionary<long, int> _rev{fn};");
        }

        sb.AppendLine();
        sb.AppendLine("    static PacketIdHelper()");
        sb.AppendLine("    {");

        foreach (var kv in groups)
        {
            var fn = FieldName(kv.Key.State, kv.Key.Direction);
            sb.AppendLine("        {");
            sb.AppendLine("            var fwd = new Dictionary<long, global::System.Func<IPacket>>();");
            sb.AppendLine("            var id  = new Dictionary<long, PacketIdentifier>();");
            sb.AppendLine("            var rev = new Dictionary<long, int>();");

            foreach (var (data, order) in kv.Value)
            {
                foreach (var r in data.PacketIds)
                {
                    sb.AppendLine($"            for (int v = {r.From}; v <= {r.To}; v++)");
                    sb.AppendLine("            {");
                    sb.AppendLine($"                long k = Combine(0x{r.HexId:X2}, v);");
                    sb.AppendLine($"                fwd[k] = static () => new {data.FullyQualifiedName}();");
                    sb.AppendLine($"                id[k]  = {data.FullyQualifiedName}.Identifier;");
                    sb.AppendLine($"                rev[Combine({order}, v)] = 0x{r.HexId:X2};");
                    sb.AppendLine("            }");
                }
            }

            sb.AppendLine($"            _fwd{fn} = fwd.ToFrozenDictionary();");
            sb.AppendLine($"            _id{fn}  = id.ToFrozenDictionary();");
            sb.AppendLine($"            _rev{fn} = rev.ToFrozenDictionary();");
            sb.AppendLine("        }");
        }

        sb.AppendLine("    }");
        sb.AppendLine();

        // TryGetPacketIdentifier
        sb.AppendLine("    [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine("    public static bool TryGetPacketIdentifier(int packetId, int protocolVersion, PacketState state, PacketDirection direction, out PacketIdentifier identifier)");
        sb.AppendLine("    {");
        sb.AppendLine("        var dict = GetIdDict(state, direction);");
        sb.AppendLine("        if (dict is null) { identifier = PacketIdentifier.Undefined; return false; }");
        sb.AppendLine("        return dict.TryGetValue(Combine(packetId, protocolVersion), out identifier);");
        sb.AppendLine("    }");
        sb.AppendLine();

        // TryGetPacketId
        sb.AppendLine("    public static bool TryGetPacketId(PacketIdentifier identifier, int protocolVersion, out int packetId)");
        sb.AppendLine("    {");
        sb.AppendLine("        var dict = GetRevDict(identifier.State, identifier.Direction);");
        sb.AppendLine("        if (dict is null) { packetId = -1; return false; }");
        sb.AppendLine("        return dict.TryGetValue(Combine(identifier.Order, protocolVersion), out packetId);");
        sb.AppendLine("    }");
        sb.AppendLine();

        // TryGetFactory
        sb.AppendLine("    [MethodImpl(MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine("    public static bool TryGetFactory(int hexId, int protocolVersion, PacketState state, PacketDirection direction, out global::System.Func<IPacket>? factory)");
        sb.AppendLine("    {");
        sb.AppendLine("        var dict = GetFwdDict(state, direction);");
        sb.AppendLine("        if (dict is null) { factory = null; return false; }");
        sb.AppendLine("        return dict.TryGetValue(Combine(hexId, protocolVersion), out factory);");
        sb.AppendLine("    }");
        sb.AppendLine();

        AppendSwitchDict(sb, groups, "FrozenDictionary<long, global::System.Func<IPacket>>", "GetFwdDict", "_fwd");
        sb.AppendLine();
        AppendSwitchDict(sb, groups, "FrozenDictionary<long, PacketIdentifier>", "GetIdDict", "_id");
        sb.AppendLine();
        AppendSwitchDict(sb, groups, "FrozenDictionary<long, int>", "GetRevDict", "_rev");

        sb.AppendLine("}");
        ctx.AddSource("PacketIdHelper.g.cs", sb.ToString());
    }

    private static void AppendSwitchDict(
        StringBuilder sb,
        Dictionary<(int State, int Direction), List<(PacketData Data, int Order)>> groups,
        string returnType,
        string methodName,
        string fieldPrefix)
    {
        sb.AppendLine($"    private static {returnType}? {methodName}(PacketState state, PacketDirection direction)");
        sb.AppendLine("        => (state, direction) switch");
        sb.AppendLine("        {");
        foreach (var kv in groups)
        {
            var fn = FieldName(kv.Key.State, kv.Key.Direction);
            sb.AppendLine($"            (PacketState.{StateName(kv.Key.State)}, PacketDirection.{DirectionName(kv.Key.Direction)}) => {fieldPrefix}{fn},");
        }
        sb.AppendLine("            _ => null");
        sb.AppendLine("        };");
    }

    private static void GenerateCatalogs(SourceProductionContext ctx, List<(PacketData Data, int Order)> packets)
    {
        var groups = new Dictionary<(int State, int Direction), List<PacketData>>();
        foreach (var (data, _) in packets)
        {
            var key = (data.State, data.Direction);
            if (!groups.TryGetValue(key, out var list))
                groups[key] = list = new List<PacketData>();
            list.Add(data);
        }

        foreach (var kv in groups)
        {
            var catalogName = CatalogName(kv.Key.State, kv.Key.Direction);
            var sb = new StringBuilder();
            sb.AppendLine("namespace McProtoNet.Protocol;");
            sb.AppendLine();
            sb.AppendLine($"public static class {catalogName}");
            sb.AppendLine("{");

            foreach (var data in kv.Value)
                sb.AppendLine($"    public static readonly PacketIdentifier {data.PacketName} = {data.FullyQualifiedName}.Identifier;");

            sb.AppendLine();
            sb.AppendLine("    public static readonly PacketIdentifier[] All =");
            sb.AppendLine("    {");
            foreach (var data in kv.Value)
                sb.AppendLine($"        {data.PacketName},");
            sb.AppendLine("    };");
            sb.AppendLine("}");

            ctx.AddSource($"{catalogName}.g.cs", sb.ToString());
        }
    }

    private static string StateName(int state) => state switch
    {
        0 => "Status",
        1 => "Handshaking",
        2 => "Configuration",
        3 => "Login",
        4 => "Play",
        _ => throw new ArgumentException($"Unknown state {state}")
    };

    private static string DirectionName(int direction) => direction switch
    {
        0 => "Clientbound",
        1 => "Serverbound",
        _ => throw new ArgumentException($"Unknown direction {direction}")
    };

    private static string FieldName(int state, int direction) =>
        $"{StateName(state)}_{DirectionName(direction)}";

    private static string CatalogName(int state, int direction)
    {
        var prefix = direction == 0 ? "Server" : "Client";
        return $"{prefix}{StateName(state)}Packet";
    }
}

internal sealed class PacketIdRange
{
    public int From  { get; }
    public int To    { get; }
    public int HexId { get; }

    public PacketIdRange(int from, int to, int hexId)
    {
        From  = from;
        To    = to;
        HexId = hexId;
    }
}

internal sealed class PacketData
{
    public string Namespace          { get; }
    public string ClassName          { get; }
    public string FullyQualifiedName { get; }
    public string PacketName         { get; }
    public int    State              { get; }
    public int    Direction          { get; }
    public ImmutableArray<PacketIdRange> PacketIds { get; }
    public bool   HasGetPacketId     { get; }

    public PacketData(string ns, string className, string fqn, string packetName,
        int state, int direction, ImmutableArray<PacketIdRange> packetIds, bool hasGetPacketId)
    {
        Namespace          = ns;
        ClassName          = className;
        FullyQualifiedName = fqn;
        PacketName         = packetName;
        State              = state;
        Direction          = direction;
        PacketIds          = packetIds;
        HasGetPacketId     = hasGetPacketId;
    }
}
