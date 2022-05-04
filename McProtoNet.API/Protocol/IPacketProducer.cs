using McProtoNet.API.Networking;

namespace McProtoNet.API.Protocol
{
    public interface IPacketProducer
    {
        bool TryGetInputPacket(int id, out Lazy<IPacket> packet);
        bool TryGetOutputId(Type Tpacket, out int id);
    }
}
