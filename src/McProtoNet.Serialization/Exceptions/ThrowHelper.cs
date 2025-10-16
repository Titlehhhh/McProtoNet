using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace McProtoNet.Serialization;

internal static class ThrowHelper
{
    [DoesNotReturn]
    public static void ThrowInvalidData(string message)
        => throw new InvalidDataException(message);

    [DoesNotReturn]
    public static void ThrowProtocolViolation(string message)
        => throw new ProtocolViolationException(message);

    [DoesNotReturn]
    public static void ThrowEndOfStream(string message)
        => throw new EndOfStreamException(message);

    [DoesNotReturn]
    public static void ThrowArgumentOutOfRange(string paramName, string? message = null)
        => throw new ArgumentOutOfRangeException(paramName, message);

    [DoesNotReturn]
    public static void ThrowArgumentNull(string paramName)
        => throw new ArgumentNullException(paramName);

    [DoesNotReturn]
    public static void ThrowInvalidOperation(string message)
        => throw new InvalidOperationException(message);
    [DoesNotReturn]
    public static void ThrowVarIntTooLong()
    {
        throw new ArithmeticException("VarInt too long");
    }
}