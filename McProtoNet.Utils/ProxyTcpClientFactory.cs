using Starksoft.Net.Proxy;
using System.Net.Sockets;

namespace McProtoNet.Utils
{
    public class ProxyTcpClientFactory : ITcpClientFactory
    {
        private readonly string proxyHost;
        private readonly ushort proxyPort;
        private readonly ProxyType proxyType;

        private readonly static ProxyClientFactory factory = new();
        public ProxyTcpClientFactory(string proxyHost, ushort proxyPort, ProxyType proxyType)
        {
            this.proxyHost = proxyHost;
            this.proxyPort = proxyPort;
            this.proxyType = proxyType;
        }

        public TcpClient CreateTcpClient(string host, ushort port)
        {
            return factory.CreateProxyClient(proxyType, proxyHost, proxyPort)
                .CreateConnection(host, port);
        }
    }
}
