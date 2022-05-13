using McProtoNet.Networking;

namespace McProtoNet.Protocol
{
    /// <summary>
    /// Предоставляет методы для работы с пакетами
    /// </summary>
    public interface IPacketManager
    {
        Dictionary<int, Lazy<IPacket>> InputPackets { get; }
        Dictionary<Type, int> OutputPackets { get; }

        void RegisterOutputPacket<TPacket>(int id) where TPacket : IPacket;
        void RegisterOutputPacket(Type Tpacket, int id);

        void UnRegisterOutputPacket(Type t);
        void UnRegisterOutputPacket<TPacket>() where TPacket : IPacket;

        void RegisterInputPacket<TPacket>(int id) where TPacket : IPacket, new();
        void RegisterInputPacket(Type Tpacket, int id);
        void UnRegisterInputPacket<TPacket>() where TPacket : IPacket;
        void UnRegisterInputPacket(int id);

        void LoadInputPackets(Dictionary<int, Type> packets);
        void LoadOutputPackets(Dictionary<int, Type> packets);

        void ClearAll();
        void ClearOutputPackets();
        void ClearInputPackets();
        event EventHandler<RegisterPacketsEventArgs> PacketsRegistered;
        event EventHandler<UnRegisterPacketEventArgs> PacketsUnregister;
    }
}
