using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace McProtoNet.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public class PacketFactoryGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 1. Выбираем все синтаксические деревья из проекта
        var classes = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsClassDeclaration(node),
                transform: static (context, _) => GetClassSymbol(context)
            )
            .Where(static classSymbol => classSymbol is not null);

        // 2. Фильтруем классы, которые реализуют интерфейс IServerPacket
        var serverPacketClasses = classes
            .Where(static classSymbol => ImplementsIServerPacket(classSymbol!) && !IsAbstract(classSymbol!))
            .Collect();
        
        // 3. Регистрируем генерацию кода
        context.RegisterSourceOutput(serverPacketClasses, (context, classSymbols) =>
        {
            var sourceCode = GenerateCode(classSymbols!);
            context.AddSource("ServerPacketGenerated.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
        });
    }
    
    private static bool IsClassDeclaration(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax;
    }
    
    private static INamedTypeSymbol? GetClassSymbol(GeneratorSyntaxContext context)
    {
        // Получаем семантическую информацию о классе
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        var model = context.SemanticModel;
        return model.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;
    }
    
    private static bool ImplementsIServerPacket(INamedTypeSymbol classSymbol)
    {
        // Проверяем, реализует ли класс интерфейс IServerPacket
        return classSymbol.AllInterfaces.Any(i => i.Name == "IServerPacket");
    }
    
    private static bool IsAbstract(INamedTypeSymbol classSymbol)
    {
        // Проверяем, является ли класс абстрактным
        return classSymbol.IsAbstract;
    }
    
    private static string GenerateCode(ImmutableArray<INamedTypeSymbol> classSymbols)
    {
        // Генерация кода на основе списка классов
        var sb = new StringBuilder();
        sb.AppendLine("namespace McProtoNet.Protocol;");
        sb.AppendLine("internal static class ServerPacketRegistry");
        sb.AppendLine("{");
        sb.AppendLine("    internal static readonly Func<IServerPacket>[] Packets = new Func<IServerPacket>[]");
        sb.AppendLine("    {");
        foreach (var className in classSymbols.Select(classSymbol => classSymbol.ToDisplayString()))
        {
            sb.AppendLine($"        static () => new {className}(),");
        }
        sb.AppendLine("    };");
        sb.AppendLine("}");
        return sb.ToString();
    }
}