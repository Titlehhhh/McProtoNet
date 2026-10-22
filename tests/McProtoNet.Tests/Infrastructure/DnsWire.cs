using System.Buffers.Binary;

namespace McProtoNet.Tests.Infrastructure;

/// <summary>
///     Hand-built DNS messages: the only way to make a resolver answer exactly what a test needs —
///     a compression pointer into the middle of the question, a pointer that eats itself, a TC bit.
/// </summary>
public sealed class DnsWire
{
    public const ushort FlagResponse = 0x8000;
    public const ushort FlagRecursionDesired = 0x0100;
    public const ushort FlagRecursionAvailable = 0x0080;
    public const ushort FlagTruncated = 0x0200;

    public const ushort TypeSrv = 33;
    public const ushort TypeCName = 5;
    public const ushort ClassInternet = 1;

    private readonly List<byte> _bytes = [];

    /// <summary>Offset the next byte will land on — the value a compression pointer needs.</summary>
    public int Offset => _bytes.Count;

    public DnsWire Header(ushort id, ushort flags, ushort questions, ushort answers)
    {
        return U16(id).U16(flags).U16(questions).U16(answers).U16(0).U16(0);
    }

    /// <summary>Writes a name label by label, ending at the root.</summary>
    public DnsWire Name(string name)
    {
        foreach (var label in name.Split('.', StringSplitOptions.RemoveEmptyEntries))
        {
            _bytes.Add((byte)label.Length);
            foreach (var c in label) _bytes.Add((byte)c);
        }

        _bytes.Add(0);
        return this;
    }

    /// <summary>Writes labels without the root terminator, so a pointer can finish the name.</summary>
    public DnsWire Labels(string name)
    {
        foreach (var label in name.Split('.', StringSplitOptions.RemoveEmptyEntries))
        {
            _bytes.Add((byte)label.Length);
            foreach (var c in label) _bytes.Add((byte)c);
        }

        return this;
    }

    public DnsWire Pointer(int target)
    {
        return U16((ushort)(0xC000 | target));
    }

    public DnsWire Question(string name, ushort type = TypeSrv)
    {
        return Name(name).U16(type).U16(ClassInternet);
    }

    /// <summary>Writes a record header whose data section is produced by <paramref name="data" />.</summary>
    public DnsWire Record(Action<DnsWire> name, ushort type, Action<DnsWire> data)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(data);

        name(this);
        U16(type).U16(ClassInternet).U16(0).U16(300);

        var lengthAt = _bytes.Count;
        U16(0);
        var start = _bytes.Count;
        data(this);

        var length = (ushort)(_bytes.Count - start);
        _bytes[lengthAt] = (byte)(length >> 8);
        _bytes[lengthAt + 1] = (byte)length;
        return this;
    }

    public DnsWire U16(ushort value)
    {
        Span<byte> scratch = stackalloc byte[2];
        BinaryPrimitives.WriteUInt16BigEndian(scratch, value);
        _bytes.Add(scratch[0]);
        _bytes.Add(scratch[1]);
        return this;
    }

    public DnsWire Byte(byte value)
    {
        _bytes.Add(value);
        return this;
    }

    public byte[] ToArray()
    {
        return [.. _bytes];
    }
}
