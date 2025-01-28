namespace McProtoNet.Protocol;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class PacketSubInfoAttribute : Attribute
{
    public int MinVersion { get; }
    public int MaxVersion { get; }

    public PacketSubInfoAttribute(int minVersion, int maxVersion)
    {
        MinVersion = minVersion;
        MaxVersion = maxVersion;
    }

    public PacketSubInfoAttribute(int version)
    {
        MinVersion = version;
        MaxVersion = version;
    }
}