using McProtoNet.Core.Protocol;

namespace McProtoNet.Core.Packets
{
    public interface IPacketProvider
    {
        bool TryGetInputPacket<TPack>(int id, out MinecraftPacket<TPack> packet) where TPack : IProtocol, new();
        bool TryGetOutputId(IOutputPacket Tpacket, out int id);
    }
}
