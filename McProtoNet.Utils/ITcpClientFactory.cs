using System.Net.Sockets;

namespace McProtoNet.Utils
{
    public interface ITcpClientFactory
    {
        public TcpClient CreateTcpClient(string host, ushort port);
    }
}
