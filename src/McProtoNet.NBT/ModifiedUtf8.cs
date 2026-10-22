using System.Buffers;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Unicode;

namespace McProtoNet.NBT;

/// <summary>
/// Provides methods that convert between UTF-16 text and modified UTF-8.
/// </summary>
/// <remarks>
/// Modified UTF-8 is the string encoding that Java uses in <c>DataOutput.writeUTF</c> and
/// <c>DataInput.readUTF</c>, and through them in every NBT string. It differs from standard UTF-8 in two
/// places: U+0000 is written as <c>C0 80</c>, so that no encoded byte is zero, and a character outside the
/// Basic Multilingual Plane is written as two three-byte sequences, one per UTF-16 surrogate, instead of
/// one four-byte sequence.
/// <para>
/// All methods operate on spans. Only <see cref="GetString"/> allocates.
/// </para>
/// </remarks>
public static class ModifiedUtf8
{
    private const int StackAllocCharThreshold = 512;

    /// <summary>
    /// Returns the exact number of bytes that <see cref="FromUtf16"/> writes for the specified UTF-16 text.
    /// </summary>
    /// <param name="chars">The text to measure.</param>
    /// <returns>The number of modified UTF-8 bytes.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The encoded form does not fit in an
    /// <see cref="int"/>.</exception>
    /// <remarks>
    /// A lone surrogate is counted like any other character of the Basic Multilingual Plane, as three bytes.
    /// </remarks>
    public static int GetByteCount(ReadOnlySpan<char> chars)
    {
        long count = 0;
        var rest = chars;
        while (!rest.IsEmpty)
        {
            var nonAscii = rest.IndexOfAnyExceptInRange('\u0001', '\u007F');
            if (nonAscii < 0)
            {
                count += rest.Length;
                break;
            }

            count += nonAscii;
            rest = rest.Slice(nonAscii);

            var ascii = rest.IndexOfAnyInRange('\u0001', '\u007F');
            var run = ascii < 0 ? rest : rest.Slice(0, ascii);
            count += 2L * run.Length + CountAbove07FF(run);

            if (ascii < 0) break;
            rest = rest.Slice(ascii);
        }

        if (count > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(chars), "Encoded text does not fit in an Int32.");
        return (int)count;
    }

    /// <summary>
    /// Returns the largest number of bytes that <see cref="FromUtf16"/> can produce for the specified number
    /// of UTF-16 units.
    /// </summary>
    /// <param name="charCount">The number of UTF-16 units. This value cannot be negative.</param>
    /// <returns>The largest number of modified UTF-8 bytes, three per UTF-16 unit.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="charCount"/> is less than zero.
    /// -or-
    /// The encoded form does not fit in an <see cref="int"/>.
    /// </exception>
    public static int GetMaxByteCount(int charCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(charCount);
        var max = 3L * charCount;
        if (max > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(charCount), "Encoded text does not fit in an Int32.");
        return (int)max;
    }

    /// <summary>
    /// Returns the largest number of UTF-16 units that <see cref="ToUtf16"/> can produce for the specified
    /// number of bytes.
    /// </summary>
    /// <param name="byteCount">The number of modified UTF-8 bytes. This value cannot be negative.</param>
    /// <returns>The largest number of UTF-16 units, one per byte, because the shortest encoded sequence is
    /// one byte.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="byteCount"/> is less than
    /// zero.</exception>
    public static int GetMaxCharCount(int byteCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(byteCount);
        return byteCount;
    }

    /// <summary>
    /// Encodes UTF-16 text as modified UTF-8.
    /// </summary>
    /// <param name="source">The text to encode.</param>
    /// <param name="destination">The buffer that receives the bytes.</param>
    /// <param name="charsRead">When this method returns, contains the number of UTF-16 units that were fully
    /// encoded.</param>
    /// <param name="bytesWritten">When this method returns, contains the number of bytes written to
    /// <paramref name="destination"/>.</param>
    /// <returns><see cref="OperationStatus.Done"/> if all of <paramref name="source"/> was encoded; otherwise,
    /// <see cref="OperationStatus.DestinationTooSmall"/>.</returns>
    /// <remarks>
    /// Each UTF-16 unit is encoded on its own, so a call can be split at any character boundary and the
    /// pieces concatenated. Encoding never fails on the input; a lone surrogate is valid.
    /// </remarks>
    public static OperationStatus FromUtf16(ReadOnlySpan<char> source, Span<byte> destination,
        out int charsRead, out int bytesWritten)
    {
        charsRead = 0;
        bytesWritten = 0;

        while (!source.IsEmpty)
        {
            var nonAscii = source.IndexOfAnyExceptInRange('\u0001', '\u007F');
            var asciiRun = nonAscii < 0 ? source.Length : nonAscii;
            if (asciiRun > 0)
            {
                var free = destination.Slice(bytesWritten);
                var take = Math.Min(asciiRun, free.Length);
                Ascii.FromUtf16(source.Slice(0, take), free, out var asciiWritten);
                bytesWritten += asciiWritten;
                charsRead += asciiWritten;
                if (asciiWritten < asciiRun) return OperationStatus.DestinationTooSmall;
                source = source.Slice(asciiRun);
                if (source.IsEmpty) break;
                continue;
            }

            var ascii = source.IndexOfAnyInRange('\u0001', '\u007F');
            var runLength = ascii < 0 ? source.Length : ascii;
            for (var i = 0; i < runLength; i++)
            {
                var c = source[i];
                if (c <= 0x07FF)
                {
                    if (destination.Length - bytesWritten < 2) return OperationStatus.DestinationTooSmall;

                    destination[bytesWritten++] = (byte)(0xC0 | (c >> 6));
                    destination[bytesWritten++] = (byte)(0x80 | (c & 0x3F));
                }
                else
                {
                    if (destination.Length - bytesWritten < 3) return OperationStatus.DestinationTooSmall;

                    destination[bytesWritten++] = (byte)(0xE0 | (c >> 12));
                    destination[bytesWritten++] = (byte)(0x80 | ((c >> 6) & 0x3F));
                    destination[bytesWritten++] = (byte)(0x80 | (c & 0x3F));
                }

                charsRead++;
            }

            source = source.Slice(runLength);
        }

        return OperationStatus.Done;
    }

    /// <summary>
    /// Decodes modified UTF-8 into UTF-16.
    /// </summary>
    /// <param name="source">The bytes to decode.</param>
    /// <param name="destination">The buffer that receives the UTF-16 units.</param>
    /// <param name="bytesRead">When this method returns, contains the number of bytes consumed.</param>
    /// <param name="charsWritten">When this method returns, contains the number of UTF-16 units
    /// written.</param>
    /// <param name="allowFourByteSequences"><see langword="true"/> to accept a valid four-byte UTF-8 sequence
    /// and decode it to a surrogate pair, which reads data written by an encoder that used plain UTF-8;
    /// <see langword="false"/> to accept strict modified UTF-8 only. The default is
    /// <see langword="true"/>.</param>
    /// <returns><see cref="OperationStatus.Done"/> if all of <paramref name="source"/> was decoded;
    /// <see cref="OperationStatus.DestinationTooSmall"/> if <paramref name="destination"/> ran out; otherwise,
    /// <see cref="OperationStatus.InvalidData"/> for malformed or truncated input.</returns>
    /// <remarks>
    /// Decoding follows the byte switch of Java's <c>DataInput.readUTF</c>: a raw <c>00</c> byte decodes to
    /// U+0000, an overlong form is accepted as written, and a three-byte surrogate encoding passes through
    /// unchanged.
    /// </remarks>
    public static OperationStatus ToUtf16(ReadOnlySpan<byte> source, Span<char> destination,
        out int bytesRead, out int charsWritten, bool allowFourByteSequences = true)
    {
        bytesRead = 0;
        charsWritten = 0;

        var limit = source.Length;
        if (!allowFourByteSequences)
        {
            var fourByteLead = source.IndexOfAnyInRange((byte)0xF0, (byte)0xFF);
            if (fourByteLead >= 0) limit = fourByteLead;
        }

        while (true)
        {
            var status = Utf8.ToUtf16(source.Slice(bytesRead, limit - bytesRead), destination.Slice(charsWritten),
                out var consumed, out var produced, false);
            bytesRead += consumed;
            charsWritten += produced;

            switch (status)
            {
                case OperationStatus.Done when bytesRead == source.Length:
                    return OperationStatus.Done;
                case OperationStatus.Done:
                    return OperationStatus.InvalidData;
                case OperationStatus.DestinationTooSmall:
                    return OperationStatus.DestinationTooSmall;
                case OperationStatus.NeedMoreData:
                    return OperationStatus.InvalidData;
            }

            if (!TryDecodeAnomaly(source.Slice(bytesRead), out var anomalyBytes, out var anomalyChar))
                return OperationStatus.InvalidData;
            if (charsWritten >= destination.Length) return OperationStatus.DestinationTooSmall;

            destination[charsWritten++] = anomalyChar;
            bytesRead += anomalyBytes;
        }
    }

    /// <summary>
    /// Encodes UTF-16 text as modified UTF-8 into a buffer that is large enough to hold all of it.
    /// </summary>
    /// <param name="source">The text to encode.</param>
    /// <param name="destination">The buffer that receives the bytes.</param>
    /// <returns>The number of bytes written.</returns>
    /// <exception cref="ArgumentException"><paramref name="destination"/> is too small for the encoded
    /// text.</exception>
    public static int GetBytes(ReadOnlySpan<char> source, Span<byte> destination)
    {
        if (FromUtf16(source, destination, out _, out var bytesWritten) != OperationStatus.Done)
            throw new ArgumentException("Destination buffer is too small for the encoded text.", nameof(destination));
        return bytesWritten;
    }

    /// <summary>
    /// Decodes modified UTF-8 into a buffer that is large enough to hold all of it.
    /// </summary>
    /// <param name="source">The bytes to decode.</param>
    /// <param name="destination">The buffer that receives the UTF-16 units.</param>
    /// <returns>The number of UTF-16 units written.</returns>
    /// <exception cref="ArgumentException"><paramref name="destination"/> is too small for the decoded
    /// text.</exception>
    /// <exception cref="NbtFormatException"><paramref name="source"/> is malformed.</exception>
    public static int GetChars(ReadOnlySpan<byte> source, Span<char> destination)
    {
        var status = ToUtf16(source, destination, out _, out var charsWritten);
        if (status == OperationStatus.Done) return charsWritten;
        if (status == OperationStatus.DestinationTooSmall)
            throw new ArgumentException("Destination buffer is too small for the decoded text.", nameof(destination));
        throw MalformedInput();
    }

    /// <summary>
    /// Attempts to decode modified UTF-8 into UTF-16.
    /// </summary>
    /// <param name="source">The bytes to decode.</param>
    /// <param name="destination">The buffer that receives the UTF-16 units.</param>
    /// <param name="charsWritten">When this method returns, contains the number of UTF-16 units
    /// written.</param>
    /// <returns><see langword="true"/> if all of <paramref name="source"/> was decoded into
    /// <paramref name="destination"/>; otherwise, <see langword="false"/>.</returns>
    public static bool TryGetChars(ReadOnlySpan<byte> source, Span<char> destination, out int charsWritten)
    {
        return ToUtf16(source, destination, out _, out charsWritten) == OperationStatus.Done;
    }

    /// <summary>
    /// Returns the number of UTF-16 units that <see cref="ToUtf16"/> produces for the specified bytes.
    /// </summary>
    /// <param name="source">The bytes to measure.</param>
    /// <returns>The number of UTF-16 units.</returns>
    /// <remarks>
    /// The count is exact for well-formed input. For malformed input it is an upper bound, never below the
    /// number of units that decoding writes.
    /// </remarks>
    public static int GetCharCount(ReadOnlySpan<byte> source)
    {
        var count = source.Length - CountInRangeInclusive(source, 0x80, 0xBF);
        if (source.IndexOfAnyInRange((byte)0xF0, (byte)0xF4) >= 0)
            count += CountInRangeInclusive(source, 0xF0, 0xF4);
        return count;
    }

    /// <summary>
    /// Returns a value that indicates whether the specified bytes decode as modified UTF-8.
    /// </summary>
    /// <param name="source">The bytes to check.</param>
    /// <returns><see langword="true"/> if <paramref name="source"/> decodes as modified UTF-8, four-byte
    /// UTF-8 sequences included; otherwise, <see langword="false"/>.</returns>
    public static bool IsValid(ReadOnlySpan<byte> source)
    {
        if (Utf8.IsValid(source)) return true;

        var i = 0;
        while (i < source.Length)
        {
            var rest = source.Slice(i);
            var nonAscii = rest.IndexOfAnyExceptInRange((byte)0x00, (byte)0x7F);
            if (nonAscii < 0) return true;
            i += nonAscii;

            int b = source[i];
            switch (b >> 4)
            {
                case 12:
                case 13:
                    if (i + 1 >= source.Length || (source[i + 1] & 0xC0) != 0x80) return false;
                    i += 2;
                    break;
                case 14:
                    if (i + 2 >= source.Length ||
                        (source[i + 1] & 0xC0) != 0x80 || (source[i + 2] & 0xC0) != 0x80) return false;
                    i += 3;
                    break;
                case 15:
                    if (!TryMeasureFourByte(source.Slice(i))) return false;
                    i += 4;
                    break;
                default:
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Decodes modified UTF-8 into a new string.
    /// </summary>
    /// <param name="source">The bytes to decode.</param>
    /// <returns>A string that contains the decoded text.</returns>
    /// <exception cref="NbtFormatException"><paramref name="source"/> is malformed.</exception>
    public static string GetString(ReadOnlySpan<byte> source)
    {
        if (source.IsEmpty) return string.Empty;

        var charCount = GetCharCount(source);
        if (charCount <= StackAllocCharThreshold)
        {
            Span<char> buffer = stackalloc char[StackAllocCharThreshold];
            if (!TryGetChars(source, buffer, out var written)) throw MalformedInput();
            return new string(buffer.Slice(0, written));
        }

        var rented = ArrayPool<char>.Shared.Rent(charCount);
        try
        {
            if (!TryGetChars(source, rented, out var written)) throw MalformedInput();
            return new string(rented, 0, written);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(rented);
        }
    }

    private static bool TryDecodeAnomaly(ReadOnlySpan<byte> source, out int bytesConsumed, out char value)
    {
        bytesConsumed = 0;
        value = '\0';
        if (source.IsEmpty) return false;

        int b = source[0];
        switch (b >> 4)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
                value = (char)b;
                bytesConsumed = 1;
                return true;
            case 12:
            case 13:
            {
                if (source.Length < 2) return false;
                int second = source[1];
                if ((second & 0xC0) != 0x80) return false;
                value = (char)(((b & 0x1F) << 6) | (second & 0x3F));
                bytesConsumed = 2;
                return true;
            }
            case 14:
            {
                if (source.Length < 3) return false;
                int second = source[1];
                int third = source[2];
                if ((second & 0xC0) != 0x80 || (third & 0xC0) != 0x80) return false;
                value = (char)(((b & 0x0F) << 12) | ((second & 0x3F) << 6) | (third & 0x3F));
                bytesConsumed = 3;
                return true;
            }
            default:
                return false;
        }
    }

    private static bool TryMeasureFourByte(ReadOnlySpan<byte> source)
    {
        if (source.Length < 4) return false;
        int b = source[0];
        if (b > 0xF4) return false;
        if ((source[1] & 0xC0) != 0x80 || (source[2] & 0xC0) != 0x80 || (source[3] & 0xC0) != 0x80) return false;
        var scalar = ((b & 0x07) << 18) | ((source[1] & 0x3F) << 12) | ((source[2] & 0x3F) << 6) | (source[3] & 0x3F);
        return scalar is >= 0x10000 and <= 0x10FFFF;
    }

    private static int CountAbove07FF(ReadOnlySpan<char> chars)
    {
        var values = MemoryMarshal.Cast<char, ushort>(chars);
        var count = 0;
        var i = 0;

        if (Vector.IsHardwareAccelerated && values.Length >= Vector<ushort>.Count)
        {
            var width = Vector<ushort>.Count;
            var threshold = new Vector<ushort>(0x07FF);
            var one = Vector<ushort>.One;
            var zero = Vector<ushort>.Zero;
            var limit = values.Length - width;
            while (i <= limit)
            {
                var accumulator = Vector<ushort>.Zero;
                var batch = 0;
                while (i <= limit && batch < ushort.MaxValue)
                {
                    var block = new Vector<ushort>(values.Slice(i, width));
                    accumulator += Vector.ConditionalSelect(Vector.GreaterThan(block, threshold), one, zero);
                    i += width;
                    batch++;
                }

                for (var lane = 0; lane < width; lane++) count += accumulator[lane];
            }
        }

        for (; i < values.Length; i++)
            if (values[i] > 0x07FF)
                count++;
        return count;
    }

    private static int CountInRangeInclusive(ReadOnlySpan<byte> data, byte low, byte high)
    {
        var count = 0;
        var i = 0;

        if (Vector.IsHardwareAccelerated && data.Length >= Vector<byte>.Count)
        {
            var width = Vector<byte>.Count;
            var offset = new Vector<byte>(low);
            var window = new Vector<byte>((byte)(high - low));
            var one = Vector<byte>.One;
            var zero = Vector<byte>.Zero;
            var limit = data.Length - width;
            while (i <= limit)
            {
                var accumulator = Vector<byte>.Zero;
                var batch = 0;
                while (i <= limit && batch < byte.MaxValue)
                {
                    var block = new Vector<byte>(data.Slice(i, width)) - offset;
                    accumulator += Vector.ConditionalSelect(Vector.LessThanOrEqual(block, window), one, zero);
                    i += width;
                    batch++;
                }

                Vector.Widen(accumulator, out Vector<ushort> lower, out Vector<ushort> upper);
                for (var lane = 0; lane < Vector<ushort>.Count; lane++) count += lower[lane] + upper[lane];
            }
        }

        var spread = (byte)(high - low);
        for (; i < data.Length; i++)
            if ((byte)(data[i] - low) <= spread)
                count++;
        return count;
    }

    private static NbtFormatException MalformedInput()
    {
        return new NbtFormatException("Malformed modified UTF-8 sequence.");
    }
}
