using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace McProtoNet.Primitives;

internal static class ThrowHelper
{
    [DoesNotReturn]
    [StackTraceHidden]
    public static void ThrowInvalidData(string message)
        => throw new InvalidDataException(message);

    [DoesNotReturn]
    [StackTraceHidden]
    public static void ThrowNotEnoughData()
        => throw new InvalidDataException("Not enough data left in the buffer");

    [DoesNotReturn]
    [StackTraceHidden]
    public static void ThrowEndOfStream(string message)
        => throw new EndOfStreamException(message);

    [DoesNotReturn]
    [StackTraceHidden]
    public static void ThrowArgumentOutOfRange(string paramName, string? message = null)
        => throw new ArgumentOutOfRangeException(paramName, message);

    [DoesNotReturn]
    [StackTraceHidden]
    public static void ThrowArgumentNull(string paramName)
        => throw new ArgumentNullException(paramName);

    [DoesNotReturn]
    [StackTraceHidden]
    public static void ThrowInvalidOperation(string message)
        => throw new InvalidOperationException(message);

    [DoesNotReturn]
    [StackTraceHidden]
    public static void ThrowVarIntTooLong()
        => throw new InvalidDataException("VarInt is longer than 5 bytes");
}
