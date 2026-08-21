using System.Buffers.Binary;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using McProtoNet.Tests.Infrastructure;

namespace McProtoNet.Tests.Net;

/// <summary>
///     The lookup against a DNS server standing on the loopback, so the transport itself is under test:
///     the UDP question, the TC retry over TCP with its two-byte length prefix, how the budget is split
///     between servers, and what the resolver does when one lies, floods, or says nothing at all.
/// </summary>
public class SrvResolverTransportTests
{
    private const string Host = "example.com";
    private const string Question = "_minecraft._tcp.example.com";
    private const int SuffixOffset = 28;

    private static readonly TimeSpan Budget = TimeSpan.FromSeconds(6);

    [Fact(Timeout = 60_000)]
    public async Task ResolveAllAsync_ReadsAnAnswerOverUdp()
    {
        var token = TestContext.Current.CancellationToken;
        await using var server = new FakeDnsServer((id, _) => Answer(id, truncated: false));

        var records = await SrvResolver.ResolveAllAsync(Host, [server.EndPoint], Budget, token);

        SrvResult[] expected =
        [
            new("mc.example.com", 25566, 10, 5),
            new("backup.example.com", 25567, 20, 0)
        ];
        Assert.Equal(expected, records);
    }

    /// <summary>The answer that does not fit a datagram: TC over UDP, the whole thing over TCP.</summary>
    [Fact(Timeout = 60_000)]
    public async Task ResolveAllAsync_RetriesOverTcpWhenTheAnswerIsTruncated()
    {
        var token = TestContext.Current.CancellationToken;
        await using var server = new FakeDnsServer((id, overTcp) => Answer(id, truncated: !overTcp));

        var record = await SrvResolver.ResolveAsync(Host, [server.EndPoint], token);

        // Lowest priority wins, so the draw cannot make this flaky.
        Assert.Equal(new SrvResult("mc.example.com", 25566, 10, 5), record);
        Assert.Equal(1, server.UdpQueries);
        Assert.Equal(1, server.TcpQueries);
    }

    [Fact(Timeout = 60_000)]
    public async Task ResolveAsync_NameError_IsAnAnswerAndNotAFailure()
    {
        var token = TestContext.Current.CancellationToken;
        await using var server = new FakeDnsServer((id, _) => new DnsWire()
            .Header(id, (ushort)(DnsWire.FlagResponse | DnsMessage.CodeNameError), 1, 0)
            .Question(Question)
            .ToArray());

        Assert.Null(await SrvResolver.ResolveAsync(Host, [server.EndPoint], token));
        Assert.Empty(await SrvResolver.ResolveAllAsync(Host, [server.EndPoint], Budget, token));
    }

    /// <summary>A server that fails is skipped; the next one in the list still gets asked.</summary>
    [Fact(Timeout = 60_000)]
    public async Task ResolveAllAsync_MovesOnToTheNextServer()
    {
        var token = TestContext.Current.CancellationToken;
        await using var broken = new FakeDnsServer((id, _) => new DnsWire()
            .Header(id, (ushort)(DnsWire.FlagResponse | 2), 1, 0)
            .Question(Question)
            .ToArray());
        await using var working = new FakeDnsServer((id, _) => Answer(id, truncated: false));

        var records = await SrvResolver.ResolveAllAsync(Host, [broken.EndPoint, working.EndPoint], Budget, token);

        Assert.Equal(2, records.Count);
        Assert.Equal(1, broken.UdpQueries);
        Assert.Equal(1, working.UdpQueries);
    }

    /// <summary>
    ///     The budget is split between servers rather than spent on the first one. A resolver that has gone
    ///     quiet must not eat the time the second one needed — that is the difference between a client that
    ///     falls back and a client that hangs for its whole connect timeout.
    /// </summary>
    [Fact(Timeout = 60_000)]
    public async Task ResolveAllAsync_SilentFirstServer_StillReachesTheSecondInsideTheBudget()
    {
        var token = TestContext.Current.CancellationToken;
        await using var silent = new FakeDnsServer((_, _) => null);
        await using var working = new FakeDnsServer((id, _) => Answer(id, truncated: false));

        var budget = TimeSpan.FromSeconds(4);
        var elapsed = Stopwatch.StartNew();
        var records = await SrvResolver.ResolveAllAsync(Host, [silent.EndPoint, working.EndPoint], budget, token);
        elapsed.Stop();

        Assert.Equal(2, records.Count);
        Assert.Equal(1, silent.UdpQueries);
        Assert.Equal(1, working.UdpQueries);
        Assert.True(elapsed.Elapsed < budget, $"The silent server took the whole budget: {elapsed.Elapsed}.");
    }

    /// <summary>
    ///     A peer that answers with somebody else's transaction id forever must not hold the slice open:
    ///     patience runs out after a fixed count and the next server gets its turn.
    /// </summary>
    [Fact(Timeout = 60_000)]
    public async Task ResolveAllAsync_ServerThatOnlySendsStrangers_FailsOver()
    {
        var token = TestContext.Current.CancellationToken;
        await using var noisy = new FakeDnsServer((id, _) => Answer((ushort)(id ^ 0xFFFF), truncated: false),
            udpReplies: 64);
        await using var working = new FakeDnsServer((id, _) => Answer(id, truncated: false));

        var elapsed = Stopwatch.StartNew();
        var records = await SrvResolver.ResolveAllAsync(Host, [noisy.EndPoint, working.EndPoint], Budget, token);
        elapsed.Stop();

        Assert.Equal(2, records.Count);
        Assert.Equal(1, noisy.UdpQueries);

        // It gave up on the flood rather than waiting out its slice.
        Assert.True(elapsed.Elapsed < TimeSpan.FromSeconds(3), $"Gave up too slowly: {elapsed.Elapsed}.");
    }

    /// <summary>
    ///     Right transaction id, wrong question. A real answer echoes what was asked, so this is somebody
    ///     else's traffic — or a forgery that guessed the id — and it is not taken for an answer.
    /// </summary>
    [Fact(Timeout = 60_000)]
    public async Task ResolveAllAsync_AnswerToADifferentQuestion_IsRejected()
    {
        var token = TestContext.Current.CancellationToken;
        await using var liar = new FakeDnsServer((id, _) => new DnsWire()
            .Header(id, DnsWire.FlagResponse, 1, 1)
            .Question("_minecraft._tcp.attacker.example")
            .Record(w => w.Pointer(DnsMessage.HeaderLength), DnsWire.TypeSrv,
                w => w.U16(0).U16(0).U16(31337).Name("evil.example"))
            .ToArray(), udpReplies: 64);
        await using var working = new FakeDnsServer((id, _) => Answer(id, truncated: false));

        var records = await SrvResolver.ResolveAllAsync(Host, [liar.EndPoint, working.EndPoint], Budget, token);

        Assert.Equal(2, records.Count);
        Assert.DoesNotContain(records, record => record.Target.Contains("evil", StringComparison.Ordinal));
    }

    [Fact(Timeout = 60_000)]
    public async Task ResolveAllAsync_NobodyAnswers_Throws()
    {
        var token = TestContext.Current.CancellationToken;
        await using var silent = new FakeDnsServer((_, _) => null);

        var failure = await Assert.ThrowsAsync<IOException>(
            async () => await SrvResolver.ResolveAllAsync(Host, [silent.EndPoint], TimeSpan.FromSeconds(2), token));

        Assert.Contains(Question, failure.Message, StringComparison.Ordinal);
    }

    /// <summary>
    ///     Cancelled while the receive is genuinely in flight — not cancelled up front. The caller's
    ///     cancellation has to come back as cancellation, never dressed up as a lookup failure.
    /// </summary>
    [Fact(Timeout = 60_000)]
    public async Task ResolveAllAsync_CancelledMidReceive_ReportsCancellationNotFailure()
    {
        var token = TestContext.Current.CancellationToken;
        await using var silent = new FakeDnsServer((_, _) => null);
        using var cancel = CancellationTokenSource.CreateLinkedTokenSource(token);

        var lookup = SrvResolver
            .ResolveAllAsync(Host, [silent.EndPoint], TimeSpan.FromSeconds(30), cancel.Token)
            .AsTask();

        await Task.Delay(TimeSpan.FromMilliseconds(250), token);
        Assert.False(lookup.IsCompleted, "The silent server should still be keeping the receive in flight.");

        await cancel.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await lookup);
    }

    [Fact(Timeout = 60_000)]
    public async Task ResolveAllAsync_NoServers_IsAnEmptyAnswerOnBothPaths()
    {
        var token = TestContext.Current.CancellationToken;

        Assert.Empty(await SrvResolver.ResolveAllAsync(Host, [], Budget, token));
        Assert.Null(await SrvResolver.ResolveAsync(Host, [], token));
    }

    private static byte[] Answer(ushort id, bool truncated)
    {
        var flags = (ushort)(DnsWire.FlagResponse | DnsWire.FlagRecursionAvailable |
                             (truncated ? DnsWire.FlagTruncated : 0));

        var wire = new DnsWire()
            .Header(id, flags, 1, (ushort)(truncated ? 0 : 2))
            .Question(Question);

        if (truncated) return wire.ToArray();

        wire.Record(w => w.Pointer(DnsMessage.HeaderLength), DnsWire.TypeSrv,
            w => w.U16(10).U16(5).U16(25566).Labels("mc").Pointer(SuffixOffset));
        wire.Record(w => w.Pointer(DnsMessage.HeaderLength), DnsWire.TypeSrv,
            w => w.U16(20).U16(0).U16(25567).Labels("backup").Pointer(SuffixOffset));
        return wire.ToArray();
    }

    /// <summary>
    ///     A DNS server on the loopback that answers whatever the test tells it to. The callback sees the
    ///     query id and whether the question arrived over TCP; returning null means "say nothing".
    /// </summary>
    private sealed class FakeDnsServer : IAsyncDisposable
    {
        private readonly Func<ushort, bool, byte[]?> _answer;
        private readonly int _udpReplies;
        private readonly Socket _udp;
        private readonly Socket _tcp;
        private readonly CancellationTokenSource _stop = new();
        private readonly Task _udpLoop;
        private readonly Task _tcpLoop;
        private int _udpQueries;
        private int _tcpQueries;

        public FakeDnsServer(Func<ushort, bool, byte[]?> answer, int udpReplies = 1)
        {
            _answer = answer;
            _udpReplies = udpReplies;

            // The resolver retries over TCP on the same address and port, so this server needs both. The
            // port the UDP bind picked may already be taken on TCP by somebody else, so try again.
            (_udp, _tcp, EndPoint) = BindBothProtocols();

            _udpLoop = Task.Run(ServeUdpAsync, CancellationToken.None);
            _tcpLoop = Task.Run(ServeTcpAsync, CancellationToken.None);
        }

        public IPEndPoint EndPoint { get; }

        public int UdpQueries => Volatile.Read(ref _udpQueries);

        public int TcpQueries => Volatile.Read(ref _tcpQueries);

        public async ValueTask DisposeAsync()
        {
            await _stop.CancelAsync();
            _udp.Dispose();
            _tcp.Dispose();
            await Task.WhenAll(_udpLoop, _tcpLoop);
            _stop.Dispose();
        }

        private static (Socket Udp, Socket Tcp, IPEndPoint EndPoint) BindBothProtocols()
        {
            for (var attempt = 1; ; attempt++)
            {
                Socket? udp = null;
                Socket? tcp = null;
                try
                {
                    udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    udp.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                    var port = ((IPEndPoint)udp.LocalEndPoint!).Port;

                    tcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    tcp.Bind(new IPEndPoint(IPAddress.Loopback, port));
                    tcp.Listen(4);

                    return (udp, tcp, new IPEndPoint(IPAddress.Loopback, port));
                }
                catch (SocketException)
                {
                    // Never leak the half-built pair, on the last attempt as much as on the first.
                    udp?.Dispose();
                    tcp?.Dispose();
                    if (attempt >= 20) throw;
                }
            }
        }

        private async Task ServeUdpAsync()
        {
            var buffer = new byte[512];
            var from = new IPEndPoint(IPAddress.Loopback, 0);

            try
            {
                while (!_stop.IsCancellationRequested)
                {
                    var request = await _udp.ReceiveFromAsync(buffer, SocketFlags.None, from, _stop.Token);
                    if (request.ReceivedBytes < DnsMessage.HeaderLength) continue;

                    Interlocked.Increment(ref _udpQueries);
                    var id = BinaryPrimitives.ReadUInt16BigEndian(buffer);

                    for (var i = 0; i < _udpReplies; i++)
                    {
                        var reply = _answer(id, false);
                        if (reply is not null)
                            await _udp.SendToAsync(reply, SocketFlags.None, request.RemoteEndPoint, _stop.Token);
                    }
                }
            }
            catch (Exception e) when (e is OperationCanceledException or ObjectDisposedException or SocketException)
            {
            }
        }

        private async Task ServeTcpAsync()
        {
            try
            {
                while (!_stop.IsCancellationRequested)
                {
                    using var client = await _tcp.AcceptAsync(_stop.Token);

                    var header = new byte[2];
                    if (!await ReadExactlyAsync(client, header)) continue;

                    var body = new byte[BinaryPrimitives.ReadUInt16BigEndian(header)];
                    if (!await ReadExactlyAsync(client, body)) continue;
                    if (body.Length < DnsMessage.HeaderLength) continue;

                    Interlocked.Increment(ref _tcpQueries);
                    var reply = _answer(BinaryPrimitives.ReadUInt16BigEndian(body), true);
                    if (reply is null) continue;

                    var framed = new byte[reply.Length + 2];
                    BinaryPrimitives.WriteUInt16BigEndian(framed, (ushort)reply.Length);
                    reply.CopyTo(framed, 2);
                    await client.SendAsync(framed, SocketFlags.None, _stop.Token);
                }
            }
            catch (Exception e) when (e is OperationCanceledException or ObjectDisposedException or SocketException)
            {
            }
        }

        private async Task<bool> ReadExactlyAsync(Socket socket, Memory<byte> buffer)
        {
            while (!buffer.IsEmpty)
            {
                var read = await socket.ReceiveAsync(buffer, SocketFlags.None, _stop.Token);
                if (read == 0) return false;
                buffer = buffer[read..];
            }

            return true;
        }
    }
}
