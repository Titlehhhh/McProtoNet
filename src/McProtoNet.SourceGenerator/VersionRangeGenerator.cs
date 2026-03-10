using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace McProtoNet.SourceGenerator;

[Generator]
public sealed class VersionRangeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var protocolTypes =
            context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (n, _) =>
                        n is TypeDeclarationSyntax t && t.AttributeLists.Count > 0,
                    transform: static (ctx, _) => GetProtocolType(ctx))
                .Where(static t => t is not null)!;

        var collected = protocolTypes.Collect();

        context.RegisterSourceOutput(collected, Generate);
    }

    static ProtocolTypeInfo? GetProtocolType(GeneratorSyntaxContext ctx)
    {
        var decl = (TypeDeclarationSyntax)ctx.Node;
        var symbol = ctx.SemanticModel.GetDeclaredSymbol(decl) as INamedTypeSymbol;
        if (symbol is null)
            return null;

        var ranges = ImmutableArray.CreateBuilder<(int, int)>();

        foreach (var attr in symbol.GetAttributes())
        {
            if (attr.AttributeClass?.Name != "ProtocolSupportAttribute")
                continue;

            var from = (int)attr.ConstructorArguments[0].Value!;
            var to = (int)attr.ConstructorArguments[1].Value!;
            ranges.Add((from, to));
        }

        return ranges.Count == 0
            ? null
            : new ProtocolTypeInfo(symbol, ranges.ToImmutable());
    }


    static void Generate(
        SourceProductionContext ctx,
        ImmutableArray<ProtocolTypeInfo> types)
    {
        foreach (var type in types)
            GenerateTypePartial(ctx, type);

        GenerateThrowHelper(ctx, types);
    }

    static string GetTypeDeclaration(INamedTypeSymbol symbol)
    {
        var accessibility = symbol.DeclaredAccessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Internal => "internal",
            _ => "internal"
        };

        var isRecord = symbol.IsRecord;
        var kind = symbol.TypeKind switch
        {
            TypeKind.Struct => "struct",
            TypeKind.Class => "class",
            _ => throw new NotSupportedException()
        };

        if (isRecord)
            return $"{accessibility} partial record {kind} {symbol.Name}";

        return $"{accessibility} partial {kind} {symbol.Name}";
    }

    static void GenerateTypePartial(
        SourceProductionContext ctx,
        ProtocolTypeInfo type)
    {
        var symbol = type.Symbol;
        var name = symbol.Name;

        var sb = new StringBuilder();
        sb.AppendLine("using McProtoNet.Protocol;");
        sb.AppendLine();

        if (!symbol.ContainingNamespace.IsGlobalNamespace)
        {
            sb.AppendLine($"namespace {symbol.ContainingNamespace.ToDisplayString()};");
            sb.AppendLine();
        }

        sb.AppendLine($"{GetTypeDeclaration(symbol)}");
        sb.AppendLine("{");

        // SupportedVersions (массив, но не используется в hot-path)
        sb.AppendLine("    public static readonly ProtocolRange[] SupportedVersions =");
        sb.AppendLine("    {");
        foreach (var (from, to) in type.Ranges)
            sb.AppendLine($"        new ProtocolRange({from}, {to}),");
        sb.AppendLine("    };");
        sb.AppendLine();

        // IsSupportedVersion
        sb.AppendLine(
            "    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]");
        sb.AppendLine("    public static bool IsSupportedVersion(int version)");
        sb.AppendLine("    {");
        sb.Append("        return ");

        for (int i = 0; i < type.Ranges.Length; i++)
        {
            var (from, to) = type.Ranges[i];
            if (i > 0)
                sb.Append(" || ");

            sb.Append($"((uint)(version - {from}) <= (uint)({to} - {from}))");
        }

        sb.AppendLine(";");
        sb.AppendLine("    }");

        sb.AppendLine("}");

        ctx.AddSource($"{name}.Protocol.g.cs", sb.ToString());
    }


    static void GenerateThrowHelper(
        SourceProductionContext ctx,
        ImmutableArray<ProtocolTypeInfo> types)
    {
        var sb = new StringBuilder();

        sb.AppendLine("using System;");
        sb.AppendLine("using System.Diagnostics.CodeAnalysis;");
        sb.AppendLine();
        sb.AppendLine("namespace McProtoNet.Protocol;");
        sb.AppendLine();
        sb.AppendLine("internal static partial class ThrowHelper");
        sb.AppendLine("{");

        sb.AppendLine("    public static void ThrowIfProtocolNotSupported<T>(int protocol)");
        sb.AppendLine("    {");

        foreach (var type in types)
        {
            var fullName = type.Symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var shortName = type.Symbol.Name;

            sb.AppendLine($"        if (typeof(T) == typeof({fullName}))");
            sb.AppendLine("        {");
            sb.AppendLine($"            if (!{shortName}.IsSupportedVersion(protocol))");
            sb.AppendLine(
                $"                ThrowProtocolNotSupported(typeof({shortName}), protocol, {shortName}.SupportedVersions);");
            sb.AppendLine("            return;");
            sb.AppendLine("        }");
            sb.AppendLine();
        }

        sb.AppendLine("        System.Diagnostics.Debug.Fail($\"Protocol type {typeof(T)} is not registered\");");
        sb.AppendLine("        ThrowUnknownType(typeof(T));");
        sb.AppendLine("    }");
        sb.AppendLine("}");


        ctx.AddSource("ThrowHelper.Protocol.g.cs", sb.ToString());
    }
}

public sealed class ProtocolTypeInfo
{
    public INamedTypeSymbol Symbol { get; }
    public ImmutableArray<(int From, int To)> Ranges { get; }

    public ProtocolTypeInfo(
        INamedTypeSymbol symbol,
        ImmutableArray<(int From, int To)> ranges)
    {
        Symbol = symbol;
        Ranges = ranges;
    }
}