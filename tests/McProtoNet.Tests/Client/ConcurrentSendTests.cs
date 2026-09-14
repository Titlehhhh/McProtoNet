using System.Net;
using System.Net.Sockets;
using McProtoNet.Tests.Infrastructure;
using McProtoNet.Transport;
using QuickProxyNet;

namespace McProtoNet.Tests.Client;

public class ConcurrentSendTests
{
    [Fact]
    public async Task ConcurrentConnectionSend_NamesTheRuleAndLeavesTheConnectionOpen()
    {
        var token = TestContext.Current.CancellationToken;
        var gate = new GateStream();
        await using var connection = new MinecraftConnection(gate);

        var first = connection.WritePacketAsync(0x00, new byte[] { 1, 2, 3 }, token).AsTask();
        await gate.WriteStarted;

        var error = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await connection.WritePacketAsync(0x01, new byte[] { 4 }, token));

        Assert.Contains("one writer", error.Message);
        Assert.Contains("serialize", error.Message);
        Assert.Null(connection.CloseReason);
        Assert.False(connection.Completion.IsCompleted);

        gate.ReleaseWrite();
        await first;
    }

    [Fact]
    public async Task ConcurrentClientSend_QueuesBehindTheGate()
    {
        var token = TestContext.Current.CancellationToken;
        var gate = new GateStream();

        await using var client = new MinecraftClient(new MinecraftClientOptions
        {
            Host = "127.0.0.1",
            Port = 25565,
            UseSrv = false,
            Proxy = new FixedStreamProxyClient(gate)
        });

        await client.ConnectAsync(token);

        var first = client.SendRawAsync(0x00, new byte[] { 1, 2, 3 }, token).AsTask();
        await gate.WriteStarted;

        var second = client.SendRawAsync(0x01, new byte[] { 4 }, token).AsTask();
        Assert.False(second.IsCompleted);

        gate.ReleaseWrite();
        await first;
        await second;
    }

    private sealed class FixedStreamProxyClient(Stream stream) : IProxyClient
    {
        public Uri ProxyUri { get; } = new("socks5://127.0.0.1:1080");
        public NetworkCredential? ProxyCredentials => null;
        public string ProxyHost => "127.0.0.1";
        public int ProxyPort => 1080;
        public ProxyType Type => ProxyType.Socks5;
        public IPEndPoint? LocalEndPoint { get; set; }
        public LingerOption? LingerState { get; set; }
        public bool NoDelay { get; set; }
        public int WriteTimeout { get; set; }
        public int ReadTimeout { get; set; }

        public ValueTask<Stream> ConnectAsync(string host, int port, CancellationToken cancellationToken) =>
            ValueTask.FromResult(stream);

        public ValueTask<Stream> ConnectAsync(Stream source, string host, int port,
            CancellationToken cancellationToken) => ValueTask.FromResult(stream);

        public ValueTask<Stream> ConnectAsync(string host, int port, TimeSpan timeout,
            CancellationToken cancellationToken) => ValueTask.FromResult(stream);
    }
}
