using McProtoNet.Core.Protocol;

namespace McProtoNet.Core.Packets
{
    public interface IPacketProvider
    {
        bool TryGetInputPacket(int id, out Packet packet);
        bool TryGetOutputId(Type Tpacket, out int id);
    }
}
