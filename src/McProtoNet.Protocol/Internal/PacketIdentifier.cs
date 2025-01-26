namespace McProtoNet.Protocol;

public sealed class PacketIdentifier
{
    public readonly string Name;
    public readonly int Order;
    public readonly PacketState State;
    public readonly PacketDirection Direction;

    public PacketIdentifier(int order, string name, PacketState state, PacketDirection direction)
    {
        Order = order;
        Name = name;
        State = state;
        Direction = direction;
    }
}