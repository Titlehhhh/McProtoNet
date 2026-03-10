namespace McProtoNet.Protocol;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class PacketIdAttribute(int from, int to, int id) : Attribute
{
    public int From { get; } = from;
    public int To   { get; } = to;
    public int Id   { get; } = id;
}