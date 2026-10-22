using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace McProtoNet.Tests.Net;

/// <summary>
///     The connect path around the SRV lookup: an address the user spelled out is used as it stands,
///     with no lookup in the way.
/// </summary>
public class MinecraftClientConnectTests
{
    [Fact]
    public async Task ConnectAsync_IpLiteral_SkipsTheLookupAndConnects()
    {
        var token = TestContext.Current.CancellationToken;

        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        try
        {
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            await using var client = new MinecraftClient(new MinecraftClientOptions
            {
                Host = "127.0.0.1",
                Port = port,
                ConnectTimeout = TimeSpan.FromSeconds(10)
            });

            var accepting = listener.AcceptTcpClientAsync(token);
            await client.ConnectAsync(token);
            using var accepted = await accepting;

            Assert.True(accepted.Connected);
            Assert.True(client.IsConnected);
        }
        finally
        {
            listener.Dispose();
        }
    }

    /// <summary>
    ///     The SRV lookup must never eat the connect budget: whatever the resolver does, the connect goes
    ///     ahead with the host as typed and fails as a connect failure, not as a cancellation.
    /// </summary>
    [Fact]
    public async Task ConnectAsync_UnknownHost_FailsAsAConnectFailureInsideTheSrvBudget()
    {
        var token = TestContext.Current.CancellationToken;

        var client = new MinecraftClient(new MinecraftClientOptions
        {
            Host = "mcprotonet-no-such-host.invalid",
            ConnectTimeout = TimeSpan.FromSeconds(60),
            SrvTimeout = TimeSpan.FromSeconds(2)
        });

        var started = Stopwatch.StartNew();
        await Assert.ThrowsAsync<SocketException>(async () => await client.ConnectAsync(token));
        started.Stop();

        // The 2 s lookup budget plus the name lookup the connect itself does — nowhere near 60 s.
        Assert.True(started.Elapsed < TimeSpan.FromSeconds(30), $"Connect took {started.Elapsed}.");
    }

    [Theory]
    [InlineData("", 25565, 30, 5)]
    [InlineData("   ", 25565, 30, 5)]
    [InlineData("host", 0, 30, 5)]
    [InlineData("host", 70000, 30, 5)]
    [InlineData("host", 25565, 0, 5)]
    [InlineData("host", 25565, 30, 0)]
    public void Constructor_RefusesOptionsThatCannotWork(string host, int port, int connectSeconds, int srvSeconds)
    {
        Assert.ThrowsAny<ArgumentException>(() => new MinecraftClient(new MinecraftClientOptions
        {
            Host = host,
            Port = port,
            ConnectTimeout = TimeSpan.FromSeconds(connectSeconds),
            SrvTimeout = TimeSpan.FromSeconds(srvSeconds)
        }));
    }
}
