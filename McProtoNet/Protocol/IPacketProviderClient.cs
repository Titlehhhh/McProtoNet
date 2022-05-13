namespace McProtoNet.Protocol
{
    public interface IPacketProviderClient
    {
        Dictionary<int, Type> HandShakePackets { get; }
        Dictionary<int, Type> StatusPackets { get; }
        Dictionary<int, Type> LoginPackets { get; }
        Dictionary<int, Type> GamePackets { get; }
    }
}
