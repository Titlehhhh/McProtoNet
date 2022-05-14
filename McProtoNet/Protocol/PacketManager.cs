using McProtoNet.Networking;

namespace McProtoNet.Protocol
{
    public sealed class PacketManager : IPacketProducer, IPacketManager
    {
        private Dictionary<int, Lazy<Packet>> packets = new Dictionary<int, Lazy<Packet>>();

        public Dictionary<int, Lazy<Packet>> InputPackets => packets ??= new Dictionary<int, Lazy<Packet>>();

        private Dictionary<Type, int> outPackets;

        public Dictionary<Type, int> OutputPackets => outPackets ??= new Dictionary<Type, int>();


        public event EventHandler<RegisterPacketsEventArgs> PacketsRegistered;
        public event EventHandler<UnRegisterPacketEventArgs> PacketsUnregister;

        public void ClearAll()
        {
            ClearInputPackets();
            ClearOutputPackets();
        }

        public void ClearInputPackets()
        {
            InputPackets.Clear();
        }

        public void ClearOutputPackets()
        {
            OutputPackets.Clear();
        }

        public void RegisterInputPacket<TPacket>(int id) where TPacket : Packet, new()
        {
            Lazy<Packet> packet = new Lazy<Packet>(() => new TPacket());
            packets.Add(id, packet);
            PacketsRegistered?.Invoke(this, new RegisterPacketsEventArgs(new List<Type>() { typeof(TPacket) }));
        }

        public void RegisterInputPacket(Type Tpacket, int id)
        {
            try
            {
                Lazy<Packet> packet = new Lazy<Packet>(() => (Packet)Activator.CreateInstance(Tpacket));
                InputPackets.Add(id, packet);
            }
            catch (InvalidCastException)
            {
                throw new InvalidOperationException($"Параметр {nameof(Tpacket)} не является производным от {nameof(Packet)}");
            }
            catch
            {
                throw;
            }
        }

        public void RegisterOutputPacket(Type Tpacket, int id)
        {
            if (!Tpacket.IsAssignableTo(typeof(Packet)))
                throw new InvalidOperationException($"Параметр {nameof(Tpacket)} не является производным от {nameof(Packet)}");
            OutputPackets.Add(Tpacket, id);
        }

        public void RegisterOutputPacket<TPacket>(int id) where TPacket : Packet
        {
            OutputPackets.Add(typeof(TPacket), id);
        }

        public bool TryGetOutputId(Type Tpacket, out int id)
        {
            foreach (var item in OutputPackets)
            {
                if (item.Key == Tpacket)
                {
                    id = item.Value;
                    return true;
                }
            }
            id = 0;
            return false;
        }

        public bool TryGetInputPacket(int id, out Lazy<Packet> packet)
        {
            if (packets.ContainsKey(id))
            {
                packet = InputPackets[id];
                return true;
            }
            packet = null;
            return false;
        }

        public void UnRegisterInputPacket(int id)
        {
            InputPackets.Remove(id);
        }

        public void UnRegisterInputPacket<TPacket>() where TPacket : Packet
        {
            throw new NotImplementedException();
        }

        public void UnRegisterOutputPacket(Type t)
        {
            OutputPackets.Remove(t);
        }

        public void UnRegisterOutputPacket<TPacket>() where TPacket : Packet
        {
            this.UnRegisterOutputPacket(typeof(TPacket));
        }

        public void UnRegisterPacket(int id)
        {
            InputPackets.Remove(id);
        }

        public void LoadInputPackets(Dictionary<int, Type> packets)
        {
            foreach (var item in packets)
            {
                if (!item.Value.IsAssignableTo(typeof(Packet)))
                {
                    throw new InvalidOperationException($"Элемент {nameof(item)} не является производным от {nameof(Packet)}");
                }
                Lazy<Packet> packet = new Lazy<Packet>(() => (Packet)Activator.CreateInstance(item.Value));
                InputPackets.Add(item.Key, packet);
            }
        }

        public void LoadOutputPackets(Dictionary<int, Type> packets)
        {
            foreach (var item in packets)
            {
                if (!item.Value.IsAssignableTo(typeof(Packet)))
                {
                    throw new InvalidOperationException($"Элемент {nameof(item)} не является производным от {nameof(Packet)}");
                }
                OutputPackets.Add(item.Value, item.Key);
            }
        }
    }
}
