using System.Net;
using System.Net.Sockets;

namespace McProtoNet.Tests.Client;

public class MinecraftClientLocalEndPointTests
{
    /// <summary>The socket leaves from the address and port the caller picked, and the server sees it.</summary>
    [Fact]
    public async Task LocalEndPoint_BindsTheOutgoingSocket()
    {
        var token = TestContext.Current.CancellationToken;

        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var serverPort = ((IPEndPoint)listener.LocalEndpoint).Port;

        var sourcePort = FreePort();

        try
        {
            await using var client = new MinecraftClient(new MinecraftClientOptions
            {
                Host = "127.0.0.1",
                Port = serverPort,
                UseSrv = false,
                LocalEndPoint = new IPEndPoint(IPAddress.Loopback, sourcePort)
            });

            await client.ConnectAsync(token);

            using var accepted = await listener.AcceptTcpClientAsync(token);
            var seen = (IPEndPoint)accepted.Client.RemoteEndPoint!;

            Assert.Equal(IPAddress.Loopback, seen.Address);
            Assert.Equal(sourcePort, seen.Port);
        }
        finally
        {
            listener.Stop();
        }
    }

    /// <summary>Port 0 still binds the address, and the OS picks the port.</summary>
    [Fact]
    public async Task LocalEndPoint_WithPortZero_LetsTheOsChoose()
    {
        var token = TestContext.Current.CancellationToken;

        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var serverPort = ((IPEndPoint)listener.LocalEndpoint).Port;

        try
        {
            await using var client = new MinecraftClient(new MinecraftClientOptions
            {
                Host = "127.0.0.1",
                Port = serverPort,
                UseSrv = false,
                LocalEndPoint = new IPEndPoint(IPAddress.Loopback, 0)
            });

            await client.ConnectAsync(token);

            using var accepted = await listener.AcceptTcpClientAsync(token);
            var seen = (IPEndPoint)accepted.Client.RemoteEndPoint!;

            Assert.Equal(IPAddress.Loopback, seen.Address);
            Assert.NotEqual(0, seen.Port);
        }
        finally
        {
            listener.Stop();
        }
    }

    /// <summary>No local endpoint keeps the old behaviour: the socket is unbound and the connect works.</summary>
    [Fact]
    public async Task NoLocalEndPoint_ConnectsAsBefore()
    {
        var token = TestContext.Current.CancellationToken;

        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var serverPort = ((IPEndPoint)listener.LocalEndpoint).Port;

        try
        {
            await using var client = new MinecraftClient(new MinecraftClientOptions
            {
                Host = "127.0.0.1",
                Port = serverPort,
                UseSrv = false
            });

            await client.ConnectAsync(token);

            using var accepted = await listener.AcceptTcpClientAsync(token);
            Assert.True(accepted.Connected);
        }
        finally
        {
            listener.Stop();
        }
    }

    private static int FreePort()
    {
        var probe = new TcpListener(IPAddress.Loopback, 0);
        probe.Start();
        var port = ((IPEndPoint)probe.LocalEndpoint).Port;
        probe.Stop();
        return port;
    }
}
