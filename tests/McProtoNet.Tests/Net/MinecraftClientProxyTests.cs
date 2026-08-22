using System.Net;
using System.Net.Sockets;
using QuickProxyNet;

namespace McProtoNet.Tests.Net;

/// <summary>
///     <see cref="MinecraftClientOptions.Proxy" />: when it is set the client never opens a socket of
///     its own — it speaks over whatever stream the proxy hands back, and closes that stream with itself.
/// </summary>
public class MinecraftClientProxyTests
{
    [Fact]
    public async Task ConnectAsync_WithProxy_SendsTheFrameThroughTheProxyStream()
    {
        var token = TestContext.Current.CancellationToken;

        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var proxyPort = ((IPEndPoint)listener.LocalEndpoint).Port;

        var accepting = listener.AcceptTcpClientAsync(token);
        var proxy = new FakeProxyClient(proxyPort);

        await using (var client = new MinecraftClient(new MinecraftClientOptions
                     {
                         Host = "127.0.0.1",
                         Port = 30000,
                         Proxy = proxy,
                         ConnectTimeout = TimeSpan.FromSeconds(10)
                     }))
        {
            await client.ConnectAsync(token);
            using var accepted = await accepting;

            // the target the client asked for is the resolved target, not the proxy's own address
            Assert.Equal("127.0.0.1", proxy.RequestedHost);
            Assert.Equal(30000, proxy.RequestedPort);
            Assert.True(client.IsConnected);

            byte[] body = [0x2A, 0x2B, 0x2C];
            await client.SendRawAsync(0x00, body, token);

            var frame = await ReadExactlyAsync(accepted.GetStream(), 5, token);
            Assert.Equal([0x04, 0x00, 0x2A, 0x2B, 0x2C], frame);
        }

        Assert.True(proxy.Stream!.Disposed);
    }

    [Fact]
    public async Task ConnectAsync_WithProxy_OpensNoSocketOfItsOwn()
    {
        var token = TestContext.Current.CancellationToken;

        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var proxyPort = ((IPEndPoint)listener.LocalEndpoint).Port;

        var accepting = listener.AcceptTcpClientAsync(token);
        var proxy = new FakeProxyClient(proxyPort);

        // a port nothing listens on: a TCP connect here would fail, the proxy path never tries
        var deadPort = FreePort();

        await using var client = new MinecraftClient(new MinecraftClientOptions
        {
            Host = "127.0.0.1",
            Port = deadPort,
            Proxy = proxy,
            ConnectTimeout = TimeSpan.FromSeconds(10)
        });

        await client.ConnectAsync(token);
        using var accepted = await accepting;

        Assert.Equal(deadPort, proxy.RequestedPort);
        Assert.True(client.IsConnected);
    }

    private static int FreePort()
    {
        using var probe = new TcpListener(IPAddress.Loopback, 0);
        probe.Start();
        var port = ((IPEndPoint)probe.LocalEndpoint).Port;
        probe.Stop();
        return port;
    }

    private static async Task<byte[]> ReadExactlyAsync(Stream stream, int count, CancellationToken token)
    {
        var buffer = new byte[count];
        var read = 0;
        while (read < count)
        {
            var got = await stream.ReadAsync(buffer.AsMemory(read), token);
            if (got == 0) throw new EndOfStreamException();
            read += got;
        }

        return buffer;
    }

    /// <summary>A proxy that dials one loopback listener and reports the target it was asked for.</summary>
    private sealed class FakeProxyClient(int listenerPort) : IProxyClient
    {
        private TcpClient? _tcp;

        public string? RequestedHost { get; private set; }
        public int RequestedPort { get; private set; }
        public TrackingStream? Stream { get; private set; }

        public Uri ProxyUri { get; } = new($"socks5://127.0.0.1:{listenerPort}");
        public NetworkCredential? ProxyCredentials => null;
        public string ProxyHost => "127.0.0.1";
        public int ProxyPort => listenerPort;
        public ProxyType Type => ProxyType.Socks5;
        public IPEndPoint? LocalEndPoint { get; set; }
        public LingerOption? LingerState { get; set; }
        public bool NoDelay { get; set; }
        public int WriteTimeout { get; set; }
        public int ReadTimeout { get; set; }

        public async ValueTask<Stream> ConnectAsync(string host, int port, CancellationToken cancellationToken)
        {
            RequestedHost = host;
            RequestedPort = port;
            _tcp = new TcpClient { NoDelay = true };
            await _tcp.ConnectAsync(IPAddress.Loopback, listenerPort, cancellationToken);
            Stream = new TrackingStream(_tcp.GetStream());
            return Stream;
        }

        public ValueTask<Stream> ConnectAsync(Stream source, string host, int port,
            CancellationToken cancellationToken) => ConnectAsync(host, port, cancellationToken);

        public ValueTask<Stream> ConnectAsync(string host, int port, TimeSpan timeout,
            CancellationToken cancellationToken) => ConnectAsync(host, port, cancellationToken);
    }

    /// <summary>Passes everything through and remembers whether the client closed it.</summary>
    internal sealed class TrackingStream(Stream inner) : Stream
    {
        public bool Disposed { get; private set; }

        public override bool CanRead => inner.CanRead;
        public override bool CanSeek => false;
        public override bool CanWrite => inner.CanWrite;
        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Flush() => inner.Flush();
        public override Task FlushAsync(CancellationToken token) => inner.FlushAsync(token);

        public override int Read(byte[] buffer, int offset, int count) => inner.Read(buffer, offset, count);
        public override int Read(Span<byte> buffer) => inner.Read(buffer);

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken token = default) =>
            inner.ReadAsync(buffer, token);

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken token) =>
            inner.ReadAsync(buffer, offset, count, token);

        public override void Write(byte[] buffer, int offset, int count) => inner.Write(buffer, offset, count);
        public override void Write(ReadOnlySpan<byte> buffer) => inner.Write(buffer);

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken token = default) =>
            inner.WriteAsync(buffer, token);

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken token) =>
            inner.WriteAsync(buffer, offset, count, token);

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();

        public override async ValueTask DisposeAsync()
        {
            Disposed = true;
            await inner.DisposeAsync();
        }

        protected override void Dispose(bool disposing)
        {
            Disposed = true;
            if (disposing) inner.Dispose();
            base.Dispose(disposing);
        }
    }
}
