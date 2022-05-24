namespace McProtoNet.API.Packets
{
    public interface IPacketProvider
    {
        int TargetVersion { get; }

        IPacketProviderClient ClientPackets { get; }
        IPacketProviderServer ServerPackets { get; }
    }
}
