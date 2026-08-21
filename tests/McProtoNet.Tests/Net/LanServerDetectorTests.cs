using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace McProtoNet.Tests.Net;

/// <summary>
///     The LAN announce format, one real datagram over the loopback, and — the part that matters most —
///     that the announce port always comes back. Parsing is exact; the socket tests skip when the machine
///     will not carry multicast to itself, but the teardown tests never skip.
/// </summary>
public class LanServerDetectorTests
{
    private const int OurPort = 25599;
    private const int RepeatPort = 25598;

    [Theory]
    [InlineData("[MOTD]A Minecraft World[/MOTD][AD]25565[/AD]", "A Minecraft World", 25565)]
    [InlineData("[MOTD]§aGreen§r name[/MOTD][AD]49152[/AD]", "§aGreen§r name", 49152)]
    [InlineData("[MOTD][/MOTD][AD]25565[/AD]", "", 25565)]
    [InlineData("[MOTD]No closing tag[AD]25565[/AD]", "No closing tag", 25565)]
    [InlineData("[MOTD]Open ad[/MOTD][AD]25565", "Open ad", 25565)]
    [InlineData("[MOTD]Spaced[/MOTD][AD] 25565 [/AD]", "Spaced", 25565)]
    [InlineData("[MOTD]Full address[/MOTD][AD]192.168.1.5:25565[/AD]", "Full address", 25565)]
    [InlineData("[AD]25565[/AD]", "", 25565)]
    [InlineData("noise [MOTD]Wrapped[/MOTD][AD]25565[/AD] noise\n", "Wrapped", 25565)]
    [InlineData("[MOTD]Highest[/MOTD][AD]65535[/AD]", "Highest", 65535)]
    public void TryParse_ReadsAnAnnouncement(string payload, string motd, int port)
    {
        Assert.True(LanServerDetector.TryParse(payload, out var parsedMotd, out var parsedPort));
        Assert.Equal(motd, parsedMotd);
        Assert.Equal(port, parsedPort);
    }

    [Theory]
    [InlineData("")]
    [InlineData("[MOTD]No port at all[/MOTD]")]
    [InlineData("[MOTD]x[/MOTD][AD]not a number[/AD]")]
    [InlineData("[MOTD]x[/MOTD][AD]0[/AD]")]
    [InlineData("[MOTD]x[/MOTD][AD]65536[/AD]")]
    [InlineData("[MOTD]x[/MOTD][AD]-1[/AD]")]
    [InlineData("[MOTD]x[/MOTD][AD][/AD]")]
    [InlineData("random udp noise")]
    public void TryParse_RefusesWhatIsNotAnAnnouncement(string payload)
    {
        Assert.False(LanServerDetector.TryParse(payload, out _, out var port));
        Assert.Equal(0, port);
    }

    [Fact]
    public void Constructor_RefusesAnIPv6Interface()
    {
        Assert.Throws<ArgumentException>(() => new LanServerDetector(IPAddress.IPv6Loopback));
    }

    [Fact]
    public async Task DiscoverAsync_RefusesAnEmptyWindow()
    {
        var token = TestContext.Current.CancellationToken;

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            async () => await LanServerDetector.DiscoverAsync(TimeSpan.Zero, token));
    }

    [Fact(Timeout = 30_000)]
    public async Task ListenAsync_RefusesASecondListenOnTheSameSocket()
    {
        var token = TestContext.Current.CancellationToken;
        await using var detector = new LanServerDetector();

        var first = detector.ListenAsync(token).GetAsyncEnumerator(token);
        _ = first.MoveNextAsync();

        var second = detector.ListenAsync(token).GetAsyncEnumerator(token);
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await second.MoveNextAsync());
    }

    /// <summary>
    ///     Checks the instrument before trusting it. If an exclusive bind cannot see a port that a live
    ///     detector is holding, then the leak tests below would pass no matter what, and they are worthless.
    /// </summary>
    [Fact(Timeout = 30_000)]
    public async Task PortProbe_SeesThePortWhileADetectorHoldsIt()
    {
        Assert.SkipUnless(PortIsFree(), "Something else on this machine already holds port 4445.");

        var detector = new LanServerDetector();
        await using (detector.ConfigureAwait(false))
        {
            Assert.False(TryBindExclusively(),
                "The probe cannot see a held port, so every leak test in this class proves nothing.");
        }

        Assert.True(PortIsFree());
    }

    /// <summary>
    ///     The scenario that used to strand a socket on port 4445 forever: dispose the enumerator, or the
    ///     detector, while a receive is in flight. The socket belongs to the detector now, so disposing it
    ///     ends the read and frees the port — and doing it ten times over frees it ten times.
    /// </summary>
    [Fact(Timeout = 60_000)]
    public async Task DisposeAsync_WithAReceiveInFlight_EndsTheListenAndFreesThePort()
    {
        var token = TestContext.Current.CancellationToken;

        for (var round = 0; round < 10; round++)
        {
            var detector = new LanServerDetector();
            var enumerator = detector.ListenAsync(token).GetAsyncEnumerator(token);
            var pending = enumerator.MoveNextAsync().AsTask();

            await Task.Delay(TimeSpan.FromMilliseconds(20), token);
            await detector.DisposeAsync();

            // A real world announcing on this network can complete the step before the dispose does, so
            // the value is not the point — that the step ends at all, and that the port comes back, is.
            await Drain(pending);
            await DisposeQuietly(enumerator);
        }

        Assert.True(PortIsFree(), "Port 4445 is still held after ten listens were disposed mid-receive.");
    }

    /// <summary>
    ///     The measured leak, reproduced exactly: dispose the *enumerator* while its receive is in flight,
    ///     ten times over. The async-iterator contract forbids that and the runtime may refuse it, but the
    ///     socket is the detector's, so refusing costs a call — not a port that never comes back.
    /// </summary>
    [Fact(Timeout = 60_000)]
    public async Task DisposingTheEnumeratorMidReceive_DoesNotStrandTheSocket()
    {
        var token = TestContext.Current.CancellationToken;

        for (var round = 0; round < 10; round++)
        {
            var detector = new LanServerDetector();
            var enumerator = detector.ListenAsync(token).GetAsyncEnumerator(token);
            var pending = enumerator.MoveNextAsync().AsTask();

            await Task.Delay(TimeSpan.FromMilliseconds(20), token);

            // "Cannot dispose while a MoveNextAsync is pending" — the contract talking, not a leak.
            await DisposeQuietly(enumerator);
            await detector.DisposeAsync();
            await Drain(pending);
        }

        Assert.True(PortIsFree(), "Port 4445 is still held after ten enumerators were disposed mid-receive.");
    }

    /// <summary>Cancelling the token ends a pending receive too, and the detector still frees the port.</summary>
    [Fact(Timeout = 30_000)]
    public async Task ListenAsync_CancelledWithAReceiveInFlight_Ends()
    {
        var token = TestContext.Current.CancellationToken;
        using var stop = CancellationTokenSource.CreateLinkedTokenSource(token);

        var detector = new LanServerDetector();
        await using (detector.ConfigureAwait(false))
        {
            var enumerator = detector.ListenAsync(stop.Token).GetAsyncEnumerator(stop.Token);
            var pending = enumerator.MoveNextAsync().AsTask();

            await Task.Delay(TimeSpan.FromMilliseconds(20), token);
            await stop.CancelAsync();

            // An announcement may beat the cancel and complete the step in flight; the enumeration still
            // has to end, so keep asking until the cancelled token says no.
            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            {
                if (await pending.WaitAsync(TimeSpan.FromSeconds(5), token))
                    while (await enumerator.MoveNextAsync())
                    {
                    }
            });
        }

        Assert.True(PortIsFree(), "Port 4445 is still held after a cancelled listen.");
    }

    /// <summary>A consumer that walks away mid-stream must not strand the port either.</summary>
    [Fact(Timeout = 30_000)]
    public async Task ListenAsync_ConsumerBreaksOut_FreesThePort()
    {
        var token = TestContext.Current.CancellationToken;
        using var stop = CancellationTokenSource.CreateLinkedTokenSource(token);
        stop.CancelAfter(TimeSpan.FromSeconds(3));

        var payload = Encoding.UTF8.GetBytes($"[MOTD]McProtoNet break[/MOTD][AD]{OurPort}[/AD]");
        var detector = new LanServerDetector();
        await using (detector.ConfigureAwait(false))
        {
            using var beacon = Beacon.Start(payload);
            try
            {
                await foreach (var _ in detector.ListenAsync(stop.Token).ConfigureAwait(false)) break;
            }
            catch (OperationCanceledException)
            {
                // Nothing was announced back to us in three seconds; the break path is untested but the
                // teardown below is the point of this test.
            }
        }

        Assert.True(PortIsFree(), "Port 4445 is still held after the consumer broke out of the loop.");
    }

    [Fact(Timeout = 60_000)]
    public async Task ListenAsync_ReceivesAnAnnouncementFromThisMachine()
    {
        var token = TestContext.Current.CancellationToken;
        var payload = Encoding.UTF8.GetBytes($"[MOTD]McProtoNet loopback[/MOTD][AD]{OurPort}[/AD]");

        using var stop = CancellationTokenSource.CreateLinkedTokenSource(token);
        stop.CancelAfter(TimeSpan.FromSeconds(6));

        LanServer? ours = null;
        var detector = new LanServerDetector();
        await using (detector.ConfigureAwait(false))
        {
            using var beacon = Beacon.Start(payload);
            try
            {
                // A real world open to LAN on this network announces here too, so keep pulling until the
                // announcement that arrives is the one this test sent.
                await foreach (var server in detector.ListenAsync(stop.Token).ConfigureAwait(false))
                    if (server.Port == OurPort)
                    {
                        ours = server;
                        break;
                    }
            }
            catch (OperationCanceledException)
            {
            }
        }

        Assert.SkipWhen(ours is null, "This machine does not carry multicast back to itself.");

        var found = ours!.Value;
        Assert.Equal("McProtoNet loopback", found.Motd);
        Assert.Equal(OurPort, found.Port);
        Assert.Equal(OurPort, found.EndPoint.Port);
        Assert.Equal(found.Source.Address, found.EndPoint.Address);
    }

    /// <summary>
    ///     Many datagrams from one announcer collapse to one entry. The beacon fires several times a second
    ///     for two seconds, so without the de-dup this comes back with a dozen copies of the same world.
    /// </summary>
    [Fact(Timeout = 60_000)]
    public async Task DiscoverAsync_ReportsEachWorldOnce()
    {
        var token = TestContext.Current.CancellationToken;
        var payload = Encoding.UTF8.GetBytes($"[MOTD]McProtoNet repeat[/MOTD][AD]{RepeatPort}[/AD]");

        using var beacon = Beacon.Start(payload);
        var found = await LanServerDetector.DiscoverAsync(TimeSpan.FromSeconds(2), token);
        var rounds = beacon.Rounds;

        var ours = found.Where(server => server.Port == RepeatPort).ToList();
        Assert.SkipWhen(ours.Count == 0, "This machine does not carry multicast back to itself.");

        // The test only means something if the same world really did announce more than once.
        Assert.True(rounds >= 3, $"The beacon only fired {rounds} times; the de-dup was never exercised.");

        foreach (var group in ours.GroupBy(server => server.EndPoint))
            Assert.Single(group);
    }

    /// <summary>Waits out the step in flight however it ends: a value, an end, or a torn-down socket.</summary>
    private static async Task Drain(Task<bool> pending)
    {
        try
        {
            await pending.WaitAsync(TimeSpan.FromSeconds(5));
        }
        catch (Exception e) when (e is OperationCanceledException or SocketException or ObjectDisposedException)
        {
        }
    }

    private static async Task DisposeQuietly(IAsyncEnumerator<LanServer> enumerator)
    {
        try
        {
            await enumerator.DisposeAsync();
        }
        catch (Exception e) when (e is NotSupportedException or OperationCanceledException or ObjectDisposedException)
        {
        }
    }

    /// <summary>
    ///     Binds the announce port exclusively. On Windows that fails while anybody else holds it, which is
    ///     exactly the leak these tests are guarding against. A leaked socket never lets go, so a short
    ///     retry costs nothing and absorbs the moment the OS takes to release a socket under load.
    /// </summary>
    private static bool PortIsFree()
    {
        for (var attempt = 0; ; attempt++)
        {
            if (TryBindExclusively()) return true;
            if (attempt >= 20) return false;
            Thread.Sleep(100);
        }
    }

    private static bool TryBindExclusively()
    {
        using var probe = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        try
        {
            probe.ExclusiveAddressUse = true;
        }
        catch (Exception e) when (e is SocketException or PlatformNotSupportedException)
        {
            return true;
        }

        try
        {
            probe.Bind(new IPEndPoint(IPAddress.Any, LanServerDetector.MulticastPort));
            return true;
        }
        catch (SocketException)
        {
            return false;
        }
    }

    /// <summary>Shouts an announcement out of every interface that might loop it back to us.</summary>
    private sealed class Beacon : IDisposable
    {
        private readonly CancellationTokenSource _stop = new();
        private readonly Task _loop;
        private int _rounds;

        private Beacon(byte[] payload)
        {
            _loop = Task.Run(async () =>
            {
                while (!_stop.IsCancellationRequested)
                {
                    Announce(payload);
                    Interlocked.Increment(ref _rounds);
                    try
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(150), _stop.Token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                }
            }, CancellationToken.None);
        }

        public int Rounds => Volatile.Read(ref _rounds);

        public void Dispose()
        {
            _stop.Cancel();
            try
            {
                _loop.GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
            }

            _stop.Dispose();
        }

        public static Beacon Start(byte[] payload)
        {
            return new Beacon(payload);
        }

        private static void Announce(byte[] payload)
        {
            var destination = new IPEndPoint(LanServerDetector.MulticastGroup, LanServerDetector.MulticastPort);

            foreach (var local in LocalAddresses())
                try
                {
                    using var sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    sender.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, true);
                    sender.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);
                    if (local is not null)
                        sender.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface,
                            local.GetAddressBytes());

                    sender.SendTo(payload, destination);
                }
                catch (SocketException)
                {
                }
        }

        private static IEnumerable<IPAddress?> LocalAddresses()
        {
            // null first: whatever interface the stack picks by itself.
            yield return null;
            yield return IPAddress.Loopback;

            foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (adapter.OperationalStatus != OperationalStatus.Up || !adapter.SupportsMulticast) continue;

                foreach (var address in adapter.GetIPProperties().UnicastAddresses)
                    if (address.Address.AddressFamily == AddressFamily.InterNetwork)
                        yield return address.Address;
            }
        }
    }
}
