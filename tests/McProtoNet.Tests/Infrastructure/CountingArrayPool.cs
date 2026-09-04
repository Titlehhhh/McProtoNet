using System.Buffers;

namespace McProtoNet.Tests.Infrastructure;

/// <summary>
///     A pool that hands out a fresh array of the exact size for every rent and refuses a return of an
///     array it does not currently have on loan. A double return, or a return of a foreign array, is
///     recorded and then throws on the spot, so a leak shows up where it happens instead of corrupting a
///     later read.
/// </summary>
public sealed class CountingArrayPool : ArrayPool<byte>
{
    private readonly object _gate = new();
    private readonly HashSet<byte[]> _onLoan = [];
    private readonly List<string> _violations = [];

    /// <summary>Gets the number of rents served.</summary>
    public int Rents
    {
        get
        {
            lock (_gate) return _rents;
        }
    }

    /// <summary>Gets the number of returns accepted, refused ones included.</summary>
    public int Returns
    {
        get
        {
            lock (_gate) return _returns;
        }
    }

    /// <summary>Gets the number of arrays that are still out on loan.</summary>
    public int OnLoan
    {
        get
        {
            lock (_gate) return _onLoan.Count;
        }
    }

    /// <summary>Gets every refused return, in the order they happened.</summary>
    public IReadOnlyList<string> Violations
    {
        get
        {
            lock (_gate) return _violations.ToArray();
        }
    }

    private int _rents;
    private int _returns;

    public override byte[] Rent(int minimumLength)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(minimumLength);

        // exact size, never reused: two rents can never be the same array, so a double return is always
        // visible
        var array = new byte[Math.Max(minimumLength, 1)];
        lock (_gate)
        {
            _rents++;
            _onLoan.Add(array);
        }

        return array;
    }

    public override void Return(byte[] array, bool clearArray = false)
    {
        ArgumentNullException.ThrowIfNull(array);

        lock (_gate)
        {
            _returns++;
            if (_onLoan.Remove(array)) return;

            var message = $"Return of an array that is not on loan (length {array.Length})";
            _violations.Add(message);
            throw new InvalidOperationException(message);
        }
    }
}
