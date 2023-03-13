
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace QuickProxy
{
    public interface IProxyClient
    {
        string ProxyHost { get; set; }

        ushort ProxyPort { get; set; }

        string ProxyName { get; }

        TcpClient TcpClient { get; set; }

        TcpClient CreateConnection(string destinationHost, ushort destinationPort);

        ValueTask<TcpClient> CreateConnectionAsync(string destinationHost, ushort destinationPort, CancellationToken cancellation = default);

    }
}
