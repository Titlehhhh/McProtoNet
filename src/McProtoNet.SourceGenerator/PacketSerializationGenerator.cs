using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace McProtoNet.SourceGenerator;

[Generator]
public sealed class PacketSerializationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var packets = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (n, _) => n is ClassDeclarationSyntax c && c.AttributeLists.Count > 0,
                transform: static (ctx, _) => GetPacketSymbol(ctx))
            .Where(static s => s is not null)!;

        context.RegisterSourceOutput(packets, Generate);
    }

    static INamedTypeSymbol? GetPacketSymbol(GeneratorSyntaxContext ctx)
    {
        var symbol = ctx.SemanticModel.GetDeclaredSymbol((ClassDeclarationSyntax)ctx.Node) as INamedTypeSymbol;
        if (symbol is null) return null;

        var attrs = symbol.GetAttributes();
        bool hasPacketInfo    = attrs.Any(a => a.AttributeClass?.Name == "PacketInfoAttribute");
        bool hasProtocolSupport = attrs.Any(a => a.AttributeClass?.Name == "ProtocolSupportAttribute");

        return hasPacketInfo && hasProtocolSupport ? symbol : null;
    }

    static void Generate(SourceProductionContext ctx, INamedTypeSymbol symbol)
    {
        var ns = symbol.ContainingNamespace.IsGlobalNamespace
            ? null
            : symbol.ContainingNamespace.ToDisplayString();

        var sb = new StringBuilder();
        sb.AppendLine("using McProtoNet.Serialization;");
        sb.AppendLine();

        if (ns is not null)
        {
            sb.AppendLine($"namespace {ns};");
            sb.AppendLine();
        }

        sb.AppendLine($"partial class {symbol.Name}");
        sb.AppendLine("{");

        sb.AppendLine($"    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)");
        sb.AppendLine("    {");
        sb.AppendLine("        if (!IsSupportedVersion(protocolVersion))");
        sb.AppendLine($"            ThrowHelper.ThrowProtocolNotSupported(nameof({symbol.Name}), protocolVersion, SupportedVersions);");
        sb.AppendLine("        Serialize(writer, protocolVersion);");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine($"    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)");
        sb.AppendLine("    {");
        sb.AppendLine("        if (!IsSupportedVersion(protocolVersion))");
        sb.AppendLine($"            ThrowHelper.ThrowProtocolNotSupported(nameof({symbol.Name}), protocolVersion, SupportedVersions);");
        sb.AppendLine("        Deserialize(ref reader, protocolVersion);");
        sb.AppendLine("    }");

        sb.AppendLine("}");

        var fileHint = ns is null ? symbol.Name : ns.Replace(".", "_") + "_" + symbol.Name;
        ctx.AddSource($"{fileHint}.Serialization.g.cs", sb.ToString());
    }
}
