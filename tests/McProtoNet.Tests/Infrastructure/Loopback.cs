using System.Net;
using System.Net.Sockets;

namespace McProtoNet.Tests.Infrastructure;

/// <summary>A real TCP pair on loopback: the closest thing to a socket a transport test can hold.</summary>
public sealed class Loopback : IAsyncDisposable
{
    private readonly TcpListener _listener;
    private readonly TcpClient _clientSide;
    private readonly TcpClient _serverSide;

    private Loopback(TcpListener listener, TcpClient clientSide, TcpClient serverSide)
    {
        _listener = listener;
        _clientSide = clientSide;
        _serverSide = serverSide;
        Client = clientSide.GetStream();
        Server = serverSide.GetStream();
    }

    public Stream Client { get; }

    public Stream Server { get; }

    public static async Task<Loopback> CreateAsync(CancellationToken token)
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();

        var client = new TcpClient { NoDelay = true };
        var connect = client.ConnectAsync((IPEndPoint)listener.LocalEndpoint, token).AsTask();
        var accepted = await listener.AcceptTcpClientAsync(token).ConfigureAwait(false);
        accepted.NoDelay = true;
        await connect.ConfigureAwait(false);

        return new Loopback(listener, client, accepted);
    }

    public ValueTask DisposeAsync()
    {
        _clientSide.Dispose();
        _serverSide.Dispose();
        _listener.Dispose();
        return ValueTask.CompletedTask;
    }
}
