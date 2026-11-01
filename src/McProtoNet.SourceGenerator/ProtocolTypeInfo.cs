using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace McProtoNet.SourceGenerator;

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
