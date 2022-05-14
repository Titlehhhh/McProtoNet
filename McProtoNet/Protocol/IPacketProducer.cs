using McProtoNet.Networking;

namespace McProtoNet.Protocol
{
    public interface IPacketProducer
    {
        bool TryGetInputPacket(int id, out Lazy<Packet> packet);
        bool TryGetOutputId(Type Tpacket, out int id);
    }
}
