namespace McProtoNet.Utils;

public struct SrvRecord(string host, ushort port)
{
    public string Host { get; } = host;

    public ushort Port { get; } = port;
}