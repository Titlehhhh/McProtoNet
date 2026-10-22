using System.Net;
using System.Net.Sockets;
using McProtoNet.Transport;

namespace McProtoNet.Tests.Client;

/// <summary>
///     What the client leaves behind. The socket it opens belongs to the stream the connection closes,
///     so after DisposeAsync the server end must see the connection go.
/// </summary>
public class MinecraftClientDisposeTests
{
    /// <summary>The socket the client dialled is closed, and the peer sees it.</summary>
    [Fact]
    public async Task DisposeAsync_ClosesTheSocketItOpened()
    {
        var token = TestContext.Current.CancellationToken;

        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        try
        {
            var client = new MinecraftClient(new MinecraftClientOptions
            {
                Host = "127.0.0.1",
                Port = ((IPEndPoint)listener.LocalEndpoint).Port,
                UseSrv = false,
                ConnectTimeout = TimeSpan.FromSeconds(15)
            });

            var accepting = listener.AcceptTcpClientAsync(token);
            await client.ConnectAsync(token);
            using var accepted = await accepting;

            await client.DisposeAsync();

            var gone = false;
            try
            {
                var read = await accepted.GetStream().ReadAsync(new byte[1], token).AsTask()
                    .WaitAsync(TimeSpan.FromSeconds(10), token);
                gone = read == 0;
            }
            catch (IOException)
            {
                // a reset instead of a clean end: the socket is gone either way
                gone = true;
            }

            Assert.True(gone, "the peer still sees the connection open after DisposeAsync");
        }
        finally
        {
            listener.Stop();
        }
    }

    /// <summary>The second DisposeAsync does nothing and does not throw.</summary>
    [Fact]
    public async Task DisposeAsync_Twice_DoesNotThrow()
    {
        var token = TestContext.Current.CancellationToken;

        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        try
        {
            var client = new MinecraftClient(new MinecraftClientOptions
            {
                Host = "127.0.0.1",
                Port = ((IPEndPoint)listener.LocalEndpoint).Port,
                UseSrv = false,
                ConnectTimeout = TimeSpan.FromSeconds(15)
            });

            var accepting = listener.AcceptTcpClientAsync(token);
            await client.ConnectAsync(token);
            using var accepted = await accepting;

            await client.DisposeAsync();
            await client.DisposeAsync();

            Assert.False(client.IsConnected);
        }
        finally
        {
            listener.Stop();
        }
    }

    /// <summary>
    ///     After ToStreaming the socket belongs to the streaming connection, not to the client. This is
    ///     why DisposeAsync leaves the TcpClient it opened alone: closing it here would take the stream
    ///     down under its new owner.
    /// </summary>
    [Fact]
    public async Task DisposeAsync_AfterToStreaming_LeavesTheConnectionAlive()
    {
        var token = TestContext.Current.CancellationToken;

        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        try
        {
            var client = new MinecraftClient(new MinecraftClientOptions
            {
                Host = "127.0.0.1",
                Port = ((IPEndPoint)listener.LocalEndpoint).Port,
                UseSrv = false,
                ConnectTimeout = TimeSpan.FromSeconds(15)
            });

            var accepting = listener.AcceptTcpClientAsync(token);
            await client.ConnectAsync(token);
            using var accepted = await accepting;
            await using var server = new MinecraftConnection(accepted.GetStream(), leaveOpen: true);

            await using var game = client.Connection.ToStreaming();
            await client.DisposeAsync();

            game.WritePacket(7, [1, 2, 3]);
            await game.FlushAsync(token);

            var frame = await server.ReadPacketAsync(token).AsTask().WaitAsync(TimeSpan.FromSeconds(10), token);

            Assert.Equal(7, frame.Id);
            Assert.Equal([1, 2, 3], frame.Body.ToArray());
        }
        finally
        {
            listener.Stop();
        }
    }
}
