using System.Diagnostics.CodeAnalysis;

namespace McProtoNet.Protocol;

internal static partial class ThrowHelper
{
    [DoesNotReturn]
    public static void ThrowProtocolNotSupported(Type type, int protocol, ProtocolRange[] ranges)
        => throw new ProtocolNotSupportException(type.Name, protocol, ranges);

    [DoesNotReturn]
    public static void ThrowProtocolNotSupported(string typeName, int protocol, ProtocolRange[] ranges)
        => throw new ProtocolNotSupportException(typeName, protocol, ranges);

    [DoesNotReturn]
    public static void ThrowBufferLengthOutOfRange(int length, int actual)
        => throw new ArgumentOutOfRangeException(nameof(length), length,
            $"Buffer length {actual} is less than requested {length}.");


    [DoesNotReturn]
    private static void ThrowUnknownType(Type type)
        => throw new InvalidOperationException($"Unknown protocol type {type}");
}
