using ProtoLib.API.Networking;

namespace ProtoLib.API.Protocol
{
    public interface IPacketProducer
    {
        bool TryGetInputPacket(int id, out Lazy<IPacket> packet);
        bool TryGetOutputId(Type Tpacket, out int id);
    }
}
