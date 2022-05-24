

using McProtoNet.API.Protocol;

namespace McProtoNet.API.Packets
{
    public interface IPacketDictionary
    {
        bool TryGetInputPacket(int id, out Lazy<Packet> packet);
        bool TryGetOutputId(Type Tpacket, out int id);
    }
}
