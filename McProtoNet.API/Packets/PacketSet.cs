

using McProtoNet.API.Protocol;

namespace McProtoNet.API.Packets
{
    public class PacketSet : IPacketSet
    {
        private readonly Dictionary<Type, int> OutPackets;
        private readonly Dictionary<int, Packet> InPackets;

        public PacketSet(Dictionary<Type, int> outPackets, Dictionary<int, Packet> inPackets)
        {
            OutPackets = outPackets;
            InPackets = inPackets;
        }

        public bool TryGetInputPacket(int id, out Packet packet)
        {
            if (InPackets.TryGetValue(id, out packet))
            {
                return true;
            }
            packet = null;
            return false;
        }

        public bool TryGetOutputId(Type Tpacket, out int id)
        {
            if (OutPackets.TryGetValue(Tpacket, out id))
            {
                return true;
            }
            id = -1;
            return false;
        }
    }
}
