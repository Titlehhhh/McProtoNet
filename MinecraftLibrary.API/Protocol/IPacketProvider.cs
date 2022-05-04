namespace ProtoLib.API.Protocol
{
    public interface IPacketProvider
    {
        int TargetVersion { get; }

        IPacketProviderClient ClientPackets { get; }
        IPacketProviderServer ServerPackets { get; }
    }
}
