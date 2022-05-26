using McProtoNet.API.Protocol;

namespace McProtoNet.API.Packets
{
    public interface IPacketSet
    {
        bool TryGetInputPacket(int id, out Packet packet);
        bool TryGetOutputId(Type Tpacket, out int id);
    }
}
