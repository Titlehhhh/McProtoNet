namespace McProtoNet.Scenarios;

/// <summary>What an <see cref="Expectation{T}" /> does when its queue is full and one more packet arrives.</summary>
public enum OverflowPolicy : byte
{
    /// <summary>Evict the oldest queued packet. Right for "latest state" waits.</summary>
    DropOldest,

    /// <summary>Discard the arriving packet. Right for "the first one matters" waits.</summary>
    DropNewest,

    /// <summary>Fail the expectation: the next await throws <see cref="PacketOverflowException" />.</summary>
    Fault
}
