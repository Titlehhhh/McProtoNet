using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace McProtoNet.SourceGenerator;

[Generator]
public class PacketGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Поиск классов с PacketInfoAttribute
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => IsClassWithPacketInfoAttribute(node),
                static (context, _) => GetPacketInfo(context))
            .Where(static info => info is not null)
            .Collect();

        // Генерация кода для найденных классов
        context.RegisterSourceOutput(classDeclarations, (ctx, classInfos) =>
        {
            foreach (var classInfo in classInfos!)
            {
                var result = GeneratePacketCode(classInfo!.Value.parentClassSymbol, classInfo.Value.nestedClasses);
                ctx.AddSource(result.FileName,
                    SourceText.From(result.SourceCode, Encoding.UTF8));
            }
        });
    }

    private static bool IsClassWithPacketInfoAttribute(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax cls && cls.AttributeLists.Count > 0;
    }

    private static (INamedTypeSymbol parentClassSymbol,
        ImmutableArray<(INamedTypeSymbol nestedClass, AttributeData subInfoAttribute)> nestedClasses)? GetPacketInfo(
            GeneratorSyntaxContext context)
    {
        var classSymbol =
            context.SemanticModel.GetDeclaredSymbol((ClassDeclarationSyntax)context.Node) as INamedTypeSymbol;
        if (classSymbol == null ||
            !classSymbol.GetAttributes().Any(a => a.AttributeClass?.Name == "PacketInfoAttribute"))
            return null;

        // Находим вложенные классы с PacketSubInfoAttribute
        var nestedClasses = classSymbol.GetTypeMembers()
            .Select(nestedClass => (nestedClass, subInfo: nestedClass.GetAttributes()
                .FirstOrDefault(attr => attr.AttributeClass?.Name == "PacketSubInfoAttribute")))
            .Where(tuple => tuple.subInfo is not null)
            .ToImmutableArray();

        return (classSymbol, nestedClasses);
    }
    enum PacketState
    {
        Status,
        Handshaking,
        Configuration,
        Login,
        Play
    }
    private static string GetState(string state)
    {
        switch (state)
        {
            case "0": return "Status";
            case "1": return "Handshaking";
            case "2": return "Configuration";
            case "3": return "Login";
            case "4": return "Play";
            default: return "Unknown";
        }
    }

    private static string GetDirection(string direction)
    {
        if(direction == "0") return "Server";
        if(direction == "1") return "Client";
        return "Unknown";
    }

    class GenerationResult
    {
        public string SourceCode { get; set; }
        public string FileName { get; set; }
    }
    private static GenerationResult GeneratePacketCode(INamedTypeSymbol parentClass,
        ImmutableArray<(INamedTypeSymbol nestedClass, AttributeData subInfoAttribute)> nestedClasses)
    {
        var packetInfo = parentClass.GetAttributes()
            .First(attr => attr.AttributeClass?.Name == "PacketInfoAttribute");
        var packetName = packetInfo.ConstructorArguments[0].Value?.ToString();
        var stage = packetInfo.ConstructorArguments[1].Value?.ToString();
        stage = GetState(stage);
        var direction = packetInfo.ConstructorArguments[2].Value?.ToString();
        direction = GetDirection(direction);
        

        var sb = new StringBuilder();
        sb.AppendLine($"namespace {parentClass.ContainingNamespace.ToDisplayString()}");
        sb.AppendLine("{");
        sb.AppendLine($"    public partial class {parentClass.Name}");
        sb.AppendLine("    {");

        // Генерация методов для вложенных классов
        foreach (var (nestedClass, subInfoAttribute) in nestedClasses)
        {
            var minVersion = subInfoAttribute.ConstructorArguments[0].Value;
            var maxVersion = subInfoAttribute.ConstructorArguments[1].Value;
            var access = nestedClass.DeclaredAccessibility.ToString().ToLower();
            sb.AppendLine($"       {access} sealed partial class {nestedClass.Name}");
            sb.AppendLine("        {");
            sb.AppendLine($"            public new static bool IsSupportedVersionStatic(int protocolVersion)");
            sb.AppendLine("            {");
            sb.AppendLine($"                return protocolVersion is >= {minVersion} and <= {maxVersion};");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine($"            public override bool IsSupportedVersion(int protocolVersion)");
            sb.AppendLine("            {");
            sb.AppendLine($"                return IsSupportedVersionStatic(protocolVersion);");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
        }
 
        // Генерация общих методов
        sb.AppendLine($"        public static bool IsSupportedVersionStatic(int protocolVersion)");
        sb.AppendLine("        {");

        List<string> equalities = [];
        foreach (var (nestedClass, _) in nestedClasses)
        {
            var gg = $"{nestedClass.Name}.IsSupportedVersionStatic(protocolVersion)";
            equalities.Add(gg);
        }
        string condition = string.Join("||", equalities);

        sb.AppendLine($"            return {condition};");
        sb.AppendLine("        }");
        sb.AppendLine();

        if (parentClass.IsAbstract)
        {
            sb.AppendLine("        public abstract bool IsSupportedVersion(int protocolVersion);");
        }
        else
        {
            sb.AppendLine("        public virtual bool IsSupportedVersion(int protocolVersion) => IsSupportedVersionStatic(protocolVersion);");
        }

        sb.AppendLine();
        sb.AppendLine($"        public static PacketIdentifier PacketId => {direction}{stage}Packet.{packetName};");
        sb.AppendLine();
        sb.AppendLine($"        public PacketIdentifier GetPacketId() => PacketId;");

        sb.AppendLine("    }");
        sb.AppendLine("}");
        return new GenerationResult()
        {

            SourceCode = sb.ToString(),
            FileName = $"{parentClass.Name}_{stage}_{direction}_Generated.g.cs"
        };
    }
}