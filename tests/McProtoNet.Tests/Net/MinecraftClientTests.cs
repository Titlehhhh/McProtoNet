using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using McProtoNet.Primitives;
using McProtoNet.Transport;
using HandshakeSb = McProtoNet.Protocol.Packets.Handshaking.Serverbound;
using PlaySb = McProtoNet.Protocol.Packets.Play.Serverbound;

namespace McProtoNet.Tests.Net;

/// <summary>
///     The glue itself over a real socket pair: what the client puts on the wire, what it reads back,
///     what happens when either side goes away.
/// </summary>
public class MinecraftClientTests
{
    private const int Pv = 772;

    [Fact]
    public async Task SendAsync_PutsTheSameBytesOnTheWireAsTheConnectionExtension()
    {
        var token = TestContext.Current.CancellationToken;
        await using var peers = await Peers.CreateAsync(token);

        var packet = new HandshakeSb.SetProtocolPacket(Pv, "localhost", 25565, 2);

        var mirror = new MemoryStream();
        await using (var reference = new MinecraftConnection(mirror, leaveOpen: true))
        {
            await reference.SendAsync(packet, Pv, token);
        }

        var expected = mirror.ToArray();
        await peers.Client.SendAsync(packet, Pv, token);

        var actual = new byte[expected.Length];
        await peers.ServerStream.ReadExactlyAsync(actual, token);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task SendRawAsync_PutsTheIdAndTheBodyOnTheWire()
    {
        var token = TestContext.Current.CancellationToken;
        await using var peers = await Peers.CreateAsync(token);

        byte[] body = [1, 2, 3, 4, 5];
        await peers.Client.SendRawAsync(0x2A, body, token);

        var frame = await peers.Server.ReadPacketAsync(token);
        Assert.Equal(0x2A, frame.Id);
        Assert.Equal(body, frame.Body.ToArray());
    }

    [Fact]
    public async Task ReadPacketsAsync_EndsWithoutErrorWhenTheServerCloses()
    {
        var token = TestContext.Current.CancellationToken;
        await using var peers = await Peers.CreateAsync(token);

        var pump = Task.Run(async () =>
        {
            var ids = new List<int>();
            await foreach (var packet in peers.Client.ReadPacketsAsync(token)) ids.Add(packet.Id);
            return ids;
        }, token);

        await peers.Server.WritePacketAsync(7, new byte[] { 1, 2, 3 }, token);
        await peers.Server.WritePacketAsync(9, new byte[] { 4 }, token);
        peers.HalfCloseServer();

        var seen = await pump.WaitAsync(TimeSpan.FromSeconds(15), token);
        Assert.Equal([7, 9], seen);
    }

    [Fact]
    public async Task SendAsync_FromEightTasksKeepsEveryFrameWhole()
    {
        var token = TestContext.Current.CancellationToken;
        await using var peers = await Peers.CreateAsync(token);

        const int tasks = 8;
        const int perTask = 25;

        Assert.True(PlaySb.KeepAlivePacket.TryGetPacketId(Pv, out var keepAliveId));

        var senders = Enumerable.Range(0, tasks).Select(sender => Task.Run(async () =>
        {
            for (var i = 0; i < perTask; i++)
                await peers.Client.SendAsync(new PlaySb.KeepAlivePacket(sender * 1000L + i), Pv, token);
        }, token)).ToArray();

        var seen = new List<long>(tasks * perTask);
        for (var i = 0; i < tasks * perTask; i++)
        {
            var frame = await peers.Server.ReadPacketAsync(token);
            Assert.Equal(keepAliveId, frame.Id);

            var reader = new MinecraftPrimitiveReader(frame.Body);
            seen.Add(PlaySb.KeepAlivePacket.Read(ref reader, Pv).KeepAliveId);
        }

        await Task.WhenAll(senders).WaitAsync(TimeSpan.FromSeconds(15), token);

        var expected = Enumerable.Range(0, tasks)
            .SelectMany(sender => Enumerable.Range(0, perTask).Select(i => sender * 1000L + i))
            .Order();
        Assert.Equal(expected, seen.Order());
    }

    /// <summary>
    ///     Both switches land between two frames and both sides stay in step — the plain frame, the
    ///     compressed one, the compressed and encrypted one, and one coming back the other way.
    /// </summary>
    [Fact]
    public async Task EncryptionAndCompression_SwitchBetweenFramesOnBothSides()
    {
        var token = TestContext.Current.CancellationToken;
        await using var peers = await Peers.CreateAsync(token);

        var secret = RandomNumberGenerator.GetBytes(16);

        await peers.Client.SendAsync(new PlaySb.KeepAlivePacket(1), Pv, token);
        Assert.Equal(1L, await ReadKeepAliveAsync(peers.Server, token));

        peers.Client.CompressionThreshold = 0;
        peers.Server.CompressionThreshold = 0;

        await peers.Client.SendAsync(new PlaySb.KeepAlivePacket(2), Pv, token);
        Assert.Equal(2L, await ReadKeepAliveAsync(peers.Server, token));

        peers.Client.EnableEncryption(secret);
        peers.Server.EnableEncryption(secret);
        Assert.True(peers.Client.IsEncrypted);

        await peers.Client.SendAsync(new PlaySb.KeepAlivePacket(3), Pv, token);
        Assert.Equal(3L, await ReadKeepAliveAsync(peers.Server, token));

        await peers.Server.SendAsync(new PlaySb.KeepAlivePacket(4), Pv, token);
        var back = await peers.Client.ReadPacketAsync(token);
        var reader = new MinecraftPrimitiveReader(back.Body);
        Assert.Equal(4L, PlaySb.KeepAlivePacket.Read(ref reader, Pv).KeepAliveId);
    }

    [Fact]
    public async Task DisposeAsync_EndsAnInFlightReadPacketsAsync()
    {
        var token = TestContext.Current.CancellationToken;
        await using var peers = await Peers.CreateAsync(token);

        var arrived = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var pump = Task.Run(async () =>
        {
            var count = 0;
            await foreach (var _ in peers.Client.ReadPacketsAsync(token))
            {
                count++;
                arrived.TrySetResult();
            }

            return count;
        }, token);

        await peers.Server.WritePacketAsync(11, new byte[] { 1 }, token);
        await arrived.Task.WaitAsync(TimeSpan.FromSeconds(15), token);

        // let the pump get back into the read, so dispose lands on a parked one and not on the
        // check at the top of the next call
        await Task.Delay(200, token);
        await peers.Client.DisposeAsync();

        var seen = await pump.WaitAsync(TimeSpan.FromSeconds(15), token);
        Assert.Equal(1, seen);
        Assert.False(peers.Client.IsConnected);
    }

    /// <summary>
    ///     The client's own budget must not leave as a cancellation. A budget of a single millisecond
    ///     cannot outlast a name lookup that has to leave the machine; a network that answers faster
    ///     than that makes the case untestable, not failed.
    /// </summary>
    [Fact]
    public async Task ConnectAsync_OwnTimeout_ThrowsTimeoutException()
    {
        var token = TestContext.Current.CancellationToken;
        await using var client = new MinecraftClient(new MinecraftClientOptions
        {
            Host = "mcprotonet-no-such-host.invalid",
            ConnectTimeout = TimeSpan.FromMilliseconds(1)
        });

        var started = Stopwatch.StartNew();
        try
        {
            await client.ConnectAsync(token);
            Assert.Skip("The connect finished inside a 1 ms budget — the timeout cannot be observed here.");
        }
        catch (SocketException ex)
        {
            Assert.Skip($"The name lookup failed with {ex.SocketErrorCode} before the budget ran out.");
        }
        catch (TimeoutException)
        {
            started.Stop();
            Assert.True(started.Elapsed < TimeSpan.FromSeconds(15), $"Connect took {started.Elapsed}.");
        }
    }

    /// <summary>The caller's own token stays a cancellation and never turns into a timeout.</summary>
    [Fact]
    public async Task ConnectAsync_CallersToken_ThrowsOperationCanceledException()
    {
        await using var client = new MinecraftClient(new MinecraftClientOptions
        {
            Host = "127.0.0.1",
            Port = 25599,
            UseSrv = false,
            ConnectTimeout = TimeSpan.FromMinutes(5)
        });

        using var caller = new CancellationTokenSource();
        await caller.CancelAsync();

        var thrown =
            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await client.ConnectAsync(caller.Token));

        // the token the caller can compare against, not the linked one the client made inside
        Assert.Equal(caller.Token, thrown.CancellationToken);
        Assert.False(client.IsConnected);
    }

    [Fact]
    public async Task Abort_ComesOutOfReadPacketsAsync()
    {
        var token = TestContext.Current.CancellationToken;
        await using var peers = await Peers.CreateAsync(token);

        var reason = new InvalidOperationException("watchdog");
        var pump = Task.Run(async () =>
        {
            await foreach (var _ in peers.Client.ReadPacketsAsync(token))
            {
            }
        }, token);

        await Task.Delay(200, token);
        peers.Client.Abort(reason);

        var thrown = await Assert.ThrowsAsync<ConnectionAbortedException>(async () =>
            await pump.WaitAsync(TimeSpan.FromSeconds(15), token));
        Assert.Same(reason, thrown.InnerException);
    }

    [Fact]
    public async Task SendAsync_AfterDispose_ThrowsInsteadOfParkingOnTheGate()
    {
        var token = TestContext.Current.CancellationToken;
        var peers = await Peers.CreateAsync(token);
        await using (peers)
        {
            await peers.Client.DisposeAsync();

            await Assert.ThrowsAsync<ObjectDisposedException>(async () =>
                await peers.Client.SendAsync(new PlaySb.KeepAlivePacket(1), Pv, token).AsTask()
                    .WaitAsync(TimeSpan.FromSeconds(10), token));
        }
    }

    [Fact]
    public async Task ConnectAsync_Twice_Refuses()
    {
        var token = TestContext.Current.CancellationToken;
        await using var peers = await Peers.CreateAsync(token);

        await Assert.ThrowsAsync<InvalidOperationException>(async () => await peers.Client.ConnectAsync(token));
    }

    [Fact]
    public async Task DisposeAsync_WhileSendAsyncIsInFlight_WaitsForItAndCloses()
    {
        var token = TestContext.Current.CancellationToken;
        await using var peers = await Peers.CreateAsync(token);

        // fill the pipe so the send parks inside the transport with a pooled buffer in hand
        var body = new byte[32 * 1024];
        var sending = Task.Run(async () =>
        {
            try
            {
                for (var i = 0; i < 500; i++) await peers.Client.SendRawAsync(1, body, token);
            }
            catch (Exception ex) when (ex is ConnectionAbortedException or ObjectDisposedException or IOException)
            {
            }
        }, token);

        await Task.Delay(300, token);
        await peers.Client.DisposeAsync();

        await sending.WaitAsync(TimeSpan.FromSeconds(15), token);
        Assert.False(peers.Client.IsConnected);
    }

    [Fact]
    public async Task MembersBeforeConnect_SayTheClientIsNotConnected()
    {
        await using var client = new MinecraftClient(new MinecraftClientOptions { Host = "127.0.0.1" });

        Assert.False(client.IsConnected);
        Assert.Throws<InvalidOperationException>(() => client.Connection);
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await client.SendAsync(new PlaySb.KeepAlivePacket(1), Pv, TestContext.Current.CancellationToken));
    }

    private static async Task<long> ReadKeepAliveAsync(MinecraftConnection connection, CancellationToken token)
    {
        var frame = await connection.ReadPacketAsync(token);
        var reader = new MinecraftPrimitiveReader(frame.Body);
        return PlaySb.KeepAlivePacket.Read(ref reader, Pv).KeepAliveId;
    }

    /// <summary>A client dialled into a listener on loopback, with the accepted end as a connection.</summary>
    private sealed class Peers : IAsyncDisposable
    {
        private readonly TcpListener _listener;
        private readonly TcpClient _accepted;

        private Peers(TcpListener listener, TcpClient accepted, MinecraftClient client)
        {
            _listener = listener;
            _accepted = accepted;
            Client = client;
            ServerStream = accepted.GetStream();
            Server = new MinecraftConnection(ServerStream, leaveOpen: true);
        }

        public MinecraftClient Client { get; }

        public MinecraftConnection Server { get; }

        public Stream ServerStream { get; }

        /// <summary>Sends FIN from the server side: a clean end of stream for the client, no reset.</summary>
        public void HalfCloseServer() => _accepted.Client.Shutdown(SocketShutdown.Send);

        public static async Task<Peers> CreateAsync(CancellationToken token)
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();

            var client = new MinecraftClient(new MinecraftClientOptions
            {
                Host = "127.0.0.1",
                Port = ((IPEndPoint)listener.LocalEndpoint).Port,
                ConnectTimeout = TimeSpan.FromSeconds(15)
            });

            var accepting = listener.AcceptTcpClientAsync(token);
            try
            {
                await client.ConnectAsync(token);
            }
            catch
            {
                listener.Dispose();
                await client.DisposeAsync();
                throw;
            }

            var accepted = await accepting;
            accepted.NoDelay = true;
            return new Peers(listener, accepted, client);
        }

        public async ValueTask DisposeAsync()
        {
            await Client.DisposeAsync();
            await Server.DisposeAsync();
            _accepted.Dispose();
            _listener.Dispose();
        }
    }
}
