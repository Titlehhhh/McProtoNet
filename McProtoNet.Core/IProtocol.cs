using McProtoNet.Core.Packets;

namespace McProtoNet.Core
{
    public enum SubProtocol
    {
        Handshake,
        Login,
        Game
    }
    public interface IProtocol : IDisposable
    {
        public int NumberVersion { get; }

        public IPacketCollection PacketCollection { get; }
    }
}
