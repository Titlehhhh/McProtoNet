using System.Diagnostics.CodeAnalysis;

namespace McProtoNet.Protocol;

internal static partial class ThrowHelper
{
    [DoesNotReturn]
    public static void ThrowProtocolNotSupported(Type type, int protocol, ProtocolRange[] ranges)
        => throw new ProtocolNotSupportedException(type.Name, protocol, ranges);


    [DoesNotReturn]
    private static void ThrowUnknownType(Type type)
        => throw new InvalidOperationException($"Unknown protocol type {type}");
}