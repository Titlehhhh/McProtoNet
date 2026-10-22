using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace McProtoNet.Transport.Framing;

/// <summary>Provides the throw sites of the framing code so the surrounding loops stay inlineable.</summary>
internal static class ThrowHelper
{
    [DoesNotReturn]
    [StackTraceHidden]
    public static void ThrowInvalidFrameLength(int length)
        => throw new InvalidDataException($"Invalid frame length {length}");

    [DoesNotReturn]
    [StackTraceHidden]
    public static void ThrowInvalidUncompressedSize(int size)
        => throw new InvalidDataException($"Invalid uncompressed size {size}");

    [DoesNotReturn]
    [StackTraceHidden]
    public static void ThrowEmptyEnvelope()
        => throw new InvalidDataException("Compression envelope carries no packet");

    [DoesNotReturn]
    [StackTraceHidden]
    public static void ThrowIdPastFrameEnd()
        => throw new InvalidDataException("Packet id runs past the end of the frame");

    [DoesNotReturn]
    [StackTraceHidden]
    public static void ThrowVarIntTooLong()
        => throw new InvalidDataException("Frame length VarInt is longer than 5 bytes");

    [DoesNotReturn]
    [StackTraceHidden]
    public static void ThrowDecompressFailed(OperationStatus status)
        => throw new InvalidDataException($"Decompress failed: {status}");

    [DoesNotReturn]
    [StackTraceHidden]
    public static void ThrowDecompressSizeMismatch(int written, int expected)
        => throw new InvalidDataException($"Decompress produced {written} bytes, frame header promised {expected}");

    [DoesNotReturn]
    [StackTraceHidden]
    public static void ThrowTruncatedFrame()
        => throw new EndOfStreamException("The stream ended in the middle of a frame");

    [DoesNotReturn]
    [StackTraceHidden]
    public static void ThrowConcurrentRead()
        => throw new InvalidOperationException("Concurrent packet reading is not allowed");

    [DoesNotReturn]
    [StackTraceHidden]
    public static void ThrowAborted(Exception? reason)
        => throw new ConnectionAbortedException(reason);

    /// <summary>Throws a new exception that carries the stored fault, instead of rethrowing it.</summary>
    [DoesNotReturn]
    [StackTraceHidden]
    public static void ThrowWriterDead(Exception? fault)
        => throw new InvalidOperationException(
            "The writer is dead: a flush failed part-way and a partial frame may be on the wire.", fault);

    [DoesNotReturn]
    [StackTraceHidden]
    public static void ThrowMovedToStreaming()
        => throw new InvalidOperationException(
            "This connection was moved to streaming mode; use the StreamingConnection instead.");

    [DoesNotReturn]
    [StackTraceHidden]
    public static void ThrowAlreadyMoved()
        => throw new InvalidOperationException("This connection was already moved to streaming mode.");

    [DoesNotReturn]
    [StackTraceHidden]
    public static void ThrowEncryptionAlreadyEnabled()
        => throw new InvalidOperationException("Encryption is already enabled.");
}
