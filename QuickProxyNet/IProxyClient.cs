using System.Net;

namespace QuickProxyNet
{
    public interface IProxyClient
    {
        NetworkCredential ProxyCredentials { get; }

        string ProxyHost { get; }

        int ProxyPort { get; }

        IPEndPoint LocalEndPoint { get; set; }

        Stream Connect(string host, int port, CancellationToken cancellationToken = default(CancellationToken));

        Task<Stream> ConnectAsync(string host, int port, CancellationToken cancellationToken = default(CancellationToken));

        Stream Connect(string host, int port, int timeout, CancellationToken cancellationToken = default(CancellationToken));

        Task<Stream> ConnectAsync(string host, int port, int timeout, CancellationToken cancellationToken = default(CancellationToken));
    }
}
