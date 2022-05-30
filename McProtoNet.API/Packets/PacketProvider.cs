

using McProtoNet.API.Protocol;

namespace McProtoNet.API.Packets
{
    public class PacketProvider : IPacketProvider
    {
        private readonly Dictionary<Type, int> _outPackets;
        private readonly Dictionary<int, Packet> _inPackets;

        public PacketProvider(Dictionary<Type, int> outPackets, Dictionary<int, Packet> inPackets)
        {
            _outPackets = outPackets;
            _inPackets = inPackets;
        }
        public PacketProvider(Dictionary<int, Type> outPackets, Dictionary<int, Type> inPackets)
        {
            this._outPackets = outPackets.ToDictionary(k => k.Value, v => v.Key);


            this._inPackets = inPackets.ToDictionary(k => k.Key, v => (Packet)Activator.CreateInstance(v.Value));
        }

        public bool TryGetInputPacket(int id, out Packet packet)
        {
            if (_inPackets.TryGetValue(id, out packet))
            {
                return true;
            }
            packet = null;
            return false;
        }

        public bool TryGetOutputId(Type Tpacket, out int id)
        {
            if (_outPackets.TryGetValue(Tpacket, out id))
            {
                return true;
            }
            id = -1;
            return false;
        }
    }
}
