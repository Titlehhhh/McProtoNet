using System.Buffers.Binary;
using System.Text;

namespace McProtoNet;

/// <summary>Holds the header bits and the SRV records of a parsed DNS answer.</summary>
internal readonly struct DnsResponse
{
    public DnsResponse(bool truncated, int responseCode, IReadOnlyList<SrvResult> records)
    {
        Truncated = truncated;
        ResponseCode = responseCode;
        Records = records;
    }

    /// <summary>Gets the TC bit: the answer did not fit a datagram and must be asked again over TCP.</summary>
    public bool Truncated { get; }

    /// <summary>Gets the RCODE of the answer.</summary>
    public int ResponseCode { get; }

    public IReadOnlyList<SrvResult> Records { get; }
}

/// <summary>Reads and writes the part of the DNS wire format an SRV lookup needs (RFC 1035, RFC 2782).</summary>
internal static class DnsMessage
{
    public const int HeaderLength = 12;

    /// <summary>The longest query this type can produce: header, a 255-byte name, type and class.</summary>
    public const int MaxQueryLength = HeaderLength + MaxNameLength + 4;

    /// <summary>RCODE 0: no error.</summary>
    public const int CodeNoError = 0;

    /// <summary>RCODE 3: the name does not exist.</summary>
    public const int CodeNameError = 3;

    private const int TypeSrv = 33;
    private const int ClassInternet = 1;
    private const int MaxNameLength = 255;
    private const int MaxLabelLength = 63;
    private const int PointerMask = 0xC0;
    private const int MaxPointerJumps = 64;

    /// <summary>Writes a recursion-desired SRV question and returns the number of bytes written.</summary>
    public static int WriteQuery(Span<byte> destination, ushort id, string name)
    {
        BinaryPrimitives.WriteUInt16BigEndian(destination, id);
        BinaryPrimitives.WriteUInt16BigEndian(destination[2..], 0x0100);
        BinaryPrimitives.WriteUInt16BigEndian(destination[4..], 1);
        destination.Slice(6, 6).Clear();

        var offset = HeaderLength + WriteName(destination[HeaderLength..], name);
        BinaryPrimitives.WriteUInt16BigEndian(destination[offset..], TypeSrv);
        BinaryPrimitives.WriteUInt16BigEndian(destination[(offset + 2)..], ClassInternet);
        return offset + 4;
    }

    /// <summary>Attempts to read the answer to the question that carried the specified transaction id.</summary>
    public static bool TryRead(ReadOnlySpan<byte> message, ushort expectedId, out DnsResponse response)
    {
        return TryRead(message, expectedId, default, out response);
    }

    /// <summary>Attempts to read the answer, also requiring the echoed question to match; an empty question skips that check.</summary>
    public static bool TryRead(ReadOnlySpan<byte> message, ushort expectedId, ReadOnlySpan<byte> expectedQuestion,
        out DnsResponse response)
    {
        response = default;
        if (message.Length < HeaderLength) return false;
        if (BinaryPrimitives.ReadUInt16BigEndian(message) != expectedId) return false;

        var flags = BinaryPrimitives.ReadUInt16BigEndian(message[2..]);
        if ((flags & 0x8000) == 0) return false;

        int questions = BinaryPrimitives.ReadUInt16BigEndian(message[4..]);
        int answers = BinaryPrimitives.ReadUInt16BigEndian(message[6..]);

        if (!expectedQuestion.IsEmpty)
        {
            if (questions != 1) return false;
            if (message.Length < HeaderLength + expectedQuestion.Length) return false;
            if (!EchoesQuestion(message.Slice(HeaderLength, expectedQuestion.Length), expectedQuestion)) return false;
        }

        var offset = HeaderLength;
        for (var i = 0; i < questions; i++)
        {
            if (!TrySkipName(message, ref offset)) return false;
            offset += 4;
            if (offset > message.Length) return false;
        }

        List<SrvResult>? records = null;
        for (var i = 0; i < answers; i++)
        {
            if (!TrySkipName(message, ref offset)) return false;
            if (offset + 10 > message.Length) return false;

            int type = BinaryPrimitives.ReadUInt16BigEndian(message[offset..]);
            int recordClass = BinaryPrimitives.ReadUInt16BigEndian(message[(offset + 2)..]);
            int dataLength = BinaryPrimitives.ReadUInt16BigEndian(message[(offset + 8)..]);
            offset += 10;
            if (offset + dataLength > message.Length) return false;

            if (type == TypeSrv && recordClass == ClassInternet && dataLength >= 7)
            {
                var priority = BinaryPrimitives.ReadUInt16BigEndian(message[offset..]);
                var weight = BinaryPrimitives.ReadUInt16BigEndian(message[(offset + 2)..]);
                var port = BinaryPrimitives.ReadUInt16BigEndian(message[(offset + 4)..]);

                // A root target (a bare "." on the wire, an empty name here) means "no service": RFC 2782.
                if (TryReadName(message, offset + 6, offset + dataLength, out var target)
                    && target.Length > 0 && port != 0)
                    (records ??= []).Add(new SrvResult(target, port, priority, weight));
            }

            offset += dataLength;
        }

        response = new DnsResponse((flags & 0x0200) != 0, flags & 0x000F,
            records ?? (IReadOnlyList<SrvResult>)Array.Empty<SrvResult>());
        return true;
    }

    // Measured before a byte is written, so a name that cannot be encoded never leaves a half-written buffer.
    private static int WriteName(Span<byte> destination, string name)
    {
        var required = 1;
        foreach (var label in Labels(name))
        {
            if (label.Length > MaxLabelLength)
                throw new ArgumentException(
                    $"DNS name '{name}' has a label longer than {MaxLabelLength} bytes.", nameof(name));

            foreach (var c in label)
                if (c > 0x7F)
                    throw new ArgumentException($"DNS name '{name}' is not ASCII.", nameof(name));

            required += label.Length + 1;
        }

        if (required > MaxNameLength)
            throw new ArgumentException(
                $"DNS name '{name}' takes {required} bytes on the wire, over the {MaxNameLength} limit.", nameof(name));

        var written = 0;
        foreach (var label in Labels(name))
        {
            destination[written++] = (byte)label.Length;
            foreach (var c in label) destination[written++] = (byte)c;
        }

        destination[written++] = 0;
        return written;
    }

    // Splits a name on dots, dropping the empty labels a leading, doubled or trailing dot leaves.
    private static LabelEnumerator Labels(string name)
    {
        return new LabelEnumerator(name.AsSpan());
    }

    private ref struct LabelEnumerator
    {
        private ReadOnlySpan<char> _remaining;

        public LabelEnumerator(ReadOnlySpan<char> name)
        {
            _remaining = name;
            Current = default;
        }

        public ReadOnlySpan<char> Current { get; private set; }

        public readonly LabelEnumerator GetEnumerator()
        {
            return this;
        }

        public bool MoveNext()
        {
            while (!_remaining.IsEmpty)
            {
                var dot = _remaining.IndexOf('.');
                if (dot < 0)
                {
                    Current = _remaining;
                    _remaining = default;
                    return true;
                }

                Current = _remaining[..dot];
                _remaining = _remaining[(dot + 1)..];
                if (!Current.IsEmpty) return true;
            }

            return false;
        }
    }

    // Letters are folded because DNS names are case-insensitive on the wire and some resolvers echo a
    // different case; the type and class bytes lie outside the letter range and so compare exactly.
    private static bool EchoesQuestion(ReadOnlySpan<byte> echoed, ReadOnlySpan<byte> asked)
    {
        if (echoed.Length != asked.Length) return false;

        for (var i = 0; i < asked.Length; i++)
            if (Fold(echoed[i]) != Fold(asked[i]))
                return false;

        return true;

        static byte Fold(byte value)
        {
            return value is >= (byte)'A' and <= (byte)'Z' ? (byte)(value + 32) : value;
        }
    }

    // Steps over a name without expanding it: a pointer ends the name where it stands.
    private static bool TrySkipName(ReadOnlySpan<byte> message, ref int offset)
    {
        while (true)
        {
            if ((uint)offset >= (uint)message.Length) return false;
            int length = message[offset];

            if (length == 0)
            {
                offset++;
                return true;
            }

            if ((length & PointerMask) == PointerMask)
            {
                if (offset + 1 >= message.Length) return false;
                offset += 2;
                return true;
            }

            if ((length & PointerMask) != 0) return false;
            offset += 1 + length;
        }
    }

    // Expands a name, following compression pointers. Labels written in place must stay inside `limit` (a
    // record's own data section); a pointer may reach anywhere earlier in the message. A pointer that does
    // not go backwards is malformed, which rules out a loop.
    private static bool TryReadName(ReadOnlySpan<byte> message, int offset, int limit, out string name)
    {
        name = string.Empty;
        var builder = new StringBuilder(MaxNameLength);
        var jumps = 0;
        var total = 0;

        while (true)
        {
            if ((uint)offset >= (uint)limit) return false;
            int length = message[offset];

            if (length == 0)
            {
                name = builder.ToString();
                return true;
            }

            if ((length & PointerMask) == PointerMask)
            {
                if (offset + 1 >= limit) return false;
                var pointer = ((length & 0x3F) << 8) | message[offset + 1];

                // RFC 1035: a pointer names a prior occurrence, so it must move backwards.
                if (++jumps > MaxPointerJumps || pointer >= offset) return false;

                offset = pointer;
                limit = message.Length;
                continue;
            }

            if ((length & PointerMask) != 0) return false;
            if (offset + 1 + length > limit) return false;

            total += length + 1;
            if (total > MaxNameLength) return false;

            if (builder.Length > 0) builder.Append('.');
            for (var i = 0; i < length; i++) builder.Append((char)message[offset + 1 + i]);
            offset += 1 + length;
        }
    }
}
