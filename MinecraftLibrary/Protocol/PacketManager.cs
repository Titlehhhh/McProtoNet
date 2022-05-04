using ProtoLib.API.Networking;
using ProtoLib.API.Protocol;

namespace ProtoLib.Protocol
{
    public sealed class PacketManager : IPacketProducer, IPacketManager
    {
        private Dictionary<int, Lazy<IPacket>> packets = new Dictionary<int, Lazy<IPacket>>();

        public Dictionary<int, Lazy<IPacket>> InputPackets => packets ??= new Dictionary<int, Lazy<IPacket>>();

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

        public void RegisterInputPacket<TPacket>(int id) where TPacket : IPacket, new()
        {
            Lazy<IPacket> packet = new Lazy<IPacket>(() => new TPacket());
            packets.Add(id, packet);
            PacketsRegistered?.Invoke(this, new RegisterPacketsEventArgs(new List<Type>() { typeof(TPacket) }));
        }

        public void RegisterInputPacket(Type Tpacket, int id)
        {
            try
            {
                Lazy<IPacket> packet = new Lazy<IPacket>(() => (IPacket)Activator.CreateInstance(Tpacket));
                InputPackets.Add(id, packet);
            }
            catch (InvalidCastException)
            {
                throw new InvalidOperationException($"Параметр {nameof(Tpacket)} не является производным от {nameof(IPacket)}");
            }
            catch
            {
                throw;
            }
        }

        public void RegisterOutputPacket(Type Tpacket, int id)
        {
            if (!Tpacket.IsAssignableTo(typeof(IPacket)))
                throw new InvalidOperationException($"Параметр {nameof(Tpacket)} не является производным от {nameof(IPacket)}");
            OutputPackets.Add(Tpacket, id);
        }

        public void RegisterOutputPacket<TPacket>(int id) where TPacket : IPacket
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

        public bool TryGetInputPacket(int id, out Lazy<IPacket> packet)
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

        public void UnRegisterInputPacket<TPacket>() where TPacket : IPacket
        {
            throw new NotImplementedException();
        }

        public void UnRegisterOutputPacket(Type t)
        {
            OutputPackets.Remove(t);
        }

        public void UnRegisterOutputPacket<TPacket>() where TPacket : IPacket
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
                if (!item.Value.IsAssignableTo(typeof(IPacket)))
                {
                    throw new InvalidOperationException($"Элемент {nameof(item)} не является производным от {nameof(IPacket)}");
                }
                Lazy<IPacket> packet = new Lazy<IPacket>(() => (IPacket)Activator.CreateInstance(item.Value));
                InputPackets.Add(item.Key, packet);
            }
        }

        public void LoadOutputPackets(Dictionary<int, Type> packets)
        {
            foreach (var item in packets)
            {
                if (!item.Value.IsAssignableTo(typeof(IPacket)))
                {
                    throw new InvalidOperationException($"Элемент {nameof(item)} не является производным от {nameof(IPacket)}");
                }
                OutputPackets.Add(item.Value, item.Key);
            }
        }
    }
}
