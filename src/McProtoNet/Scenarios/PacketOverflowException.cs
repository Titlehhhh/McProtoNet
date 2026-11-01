namespace McProtoNet.Scenarios;

/// <summary>
///     An <see cref="Expectation{T}" /> opened with <see cref="OverflowPolicy.Fault" /> received more packets than
///     its capacity before the scenario read them.
/// </summary>
public sealed class PacketOverflowException : Exception
{
    public PacketOverflowException(Type packetType, int capacity)
        : base($"More than {capacity} {packetType.Name} arrived before the scenario read them.")
    {
        PacketType = packetType;
        Capacity = capacity;
    }

    public Type PacketType { get; }
    public int Capacity { get; }
}
