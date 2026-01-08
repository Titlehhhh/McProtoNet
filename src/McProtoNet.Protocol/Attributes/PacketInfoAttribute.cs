namespace McProtoNet.Protocol;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class PacketInfoAttribute : Attribute
{
    public string Name { get; }
    public string Stage { get; }
    public string Direction { get; }


    public PacketInfoAttribute(string name, PacketState stage, PacketDirection direction)
    {
        Name = name;
        Stage = stage.ToString();
        Direction = direction.ToString();
    }
}