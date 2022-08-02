using Starksoft.Net.Proxy;
using System.Net.Sockets;

namespace McProtoNet.Utils
{
    public class ProxyTcpClientFactory : ITcpClientFactory
    {
        private readonly string proxyHost;
        private readonly ushort proxyPort;
        private readonly ProxyType proxyType;

        private readonly bool IsAuth = false;
        private readonly string login;
        private readonly string pass;
        private readonly static ProxyClientFactory factory = new();
        public ProxyTcpClientFactory(string proxyHost, ushort proxyPort, ProxyType proxyType)
        {
            ArgumentNullException.ThrowIfNull(proxyHost, nameof(proxyHost));
            if (string.IsNullOrEmpty(proxyHost))
                throw new InvalidOperationException("proxyHost is empty");
            this.proxyHost = proxyHost;
            this.proxyPort = proxyPort;
            this.proxyType = proxyType;
        }
        public ProxyTcpClientFactory(string proxyHost, ushort proxyPort, ProxyType proxyType, string login, string pass) : this(proxyHost, proxyPort, proxyType)
        {
            IsAuth = true;
            this.login = login;
            this.pass = pass;
        }

        public TcpClient CreateTcpClient(string host, ushort port)
        {
            if (IsAuth)
                return factory.CreateProxyClient(proxyType, proxyHost, proxyPort, login, pass)
                    .CreateConnection(host, port);
            return factory.CreateProxyClient(proxyType, proxyHost, proxyPort)
                .CreateConnection(host, port);
        }
    }
}
