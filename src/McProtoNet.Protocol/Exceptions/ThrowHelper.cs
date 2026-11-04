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
    public static void ThrowInvalidArrayLength(int count, int maxCount, long remaining)
        => throw new InvalidDataException(
            $"Array length {count} is out of range: the frame has {remaining} bytes left and at most " +
            $"{maxCount} elements are allowed.");

    [DoesNotReturn]
    public static void ThrowInvalidLength(int length)
        => throw new InvalidDataException($"Array length {length} is negative.");

    [DoesNotReturn]
    public static void ThrowUnsupportedElementType(Type type)
        => throw new NotSupportedException($"Array element type {type.Name} has no fixed-width wire encoding.");

    [DoesNotReturn]
    public static void ThrowArrayExceedsFrame(int count, int bytesPerElement, long remaining)
        => throw new InvalidDataException(
            $"Array of {count} elements needs at least {(long)count * bytesPerElement} bytes, " +
            $"but the frame has {remaining} left.");

    [DoesNotReturn]
    private static void ThrowUnknownType(Type type)
        => throw new InvalidOperationException($"Unknown protocol type {type}");
}
