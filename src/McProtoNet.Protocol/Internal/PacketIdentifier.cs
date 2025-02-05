namespace McProtoNet.Protocol;

public sealed class PacketIdentifier
{
    public static readonly PacketIdentifier Undefined = new(-1, "Undefined", PacketState.Status, PacketDirection.Clientbound);
    
    public readonly string Name;
    public readonly int Order;
    public readonly PacketState State;
    public readonly PacketDirection Direction;

    
    internal PacketIdentifier(int order, string name, PacketState state, PacketDirection direction)
    {
        Order = order;
        Name = name;
        State = state;
        Direction = direction;
        
    }

    public override string ToString() => Name;
}