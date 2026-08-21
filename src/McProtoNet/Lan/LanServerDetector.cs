using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace McProtoNet;

/// <summary>
///     Listens for the "open to LAN" announcements a Minecraft client shouts every 1.5 seconds:
///     <c>[MOTD]…[/MOTD][AD]…[/AD]</c> in UDP datagrams to the multicast group 224.0.2.60:4445.
///     <para>
///     The detector owns the socket, not the enumerator. Constructing one binds port 4445 and joins the
///     group; <see cref="DisposeAsync" /> closes it and ends any listen in flight. That ownership is the
///     whole point: an async iterator cannot be torn down while a read is pending, so a socket that
///     belonged to the iterator would stay bound forever when a caller disposed the enumerator mid-read.
///     Here <c>await using</c> on the detector always frees the port.
///     </para>
///     <para>
///     <see cref="ListenAsync" /> yields every announcement as it arrives, so the same world shows up
///     again and again; that is the raw feed. <see cref="DiscoverAsync(TimeSpan, CancellationToken)" /> is
///     the one-shot form and removes the repeats itself.
///     </para>
/// </summary>
public sealed class LanServerDetector : IAsyncDisposable
{
    /// <summary>The UDP port worlds announce to.</summary>
    public const int MulticastPort = 4445;

    private const int DatagramLimit = 2048;

    private readonly Socket _socket;
    private readonly byte[] _buffer = new byte[DatagramLimit];
    private readonly char[] _text = new char[DatagramLimit];
    private int _listening;
    private int _disposed;

    /// <summary>Binds the announce port and joins the group on every interface that carries multicast.</summary>
    /// <exception cref="SocketException">The port could not be bound or no group could be joined.</exception>
    public LanServerDetector() : this(null)
    {
    }

    /// <summary>Listens on one interface, named by a local IPv4 address; <see langword="null" /> means all.</summary>
    /// <exception cref="SocketException">The port could not be bound or the group could not be joined.</exception>
    public LanServerDetector(IPAddress? localInterface)
    {
        if (localInterface is not null && localInterface.AddressFamily != AddressFamily.InterNetwork)
            throw new ArgumentException("The LAN announcement group is IPv4; the interface address must be too.",
                nameof(localInterface));

        _socket = CreateSocket(localInterface);
    }

    /// <summary>The multicast group worlds announce to.</summary>
    public static IPAddress MulticastGroup { get; } = IPAddress.Parse("224.0.2.60");

    /// <summary>Closes the socket and frees the port. Any listen in flight ends without an error.</summary>
    public ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 0) _socket.Dispose();
        return ValueTask.CompletedTask;
    }

    /// <summary>
    ///     Yields announcements as they arrive until the token is cancelled, the caller stops enumerating,
    ///     or the detector is disposed. Malformed datagrams are dropped without a sound.
    ///     <para>
    ///     The token is required, not optional: it is how a listen that is otherwise endless comes to a
    ///     stop from the inside.
    ///     </para>
    /// </summary>
    /// <exception cref="InvalidOperationException">Another listen is already running on this detector.</exception>
    public async IAsyncEnumerable<LanServer> ListenAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) == 1, this);

        if (Interlocked.Exchange(ref _listening, 1) == 1)
            throw new InvalidOperationException(
                "This detector is already listening; two listens on one socket would steal each other's datagrams.");

        try
        {
            while (true)
            {
                var received = await ReceiveAsync(cancellationToken).ConfigureAwait(false);

                // Null means the detector was disposed under the read: the listen is over, not broken.
                if (received is not { } datagram) yield break;

                var (length, source) = datagram;
                if (length == 0) continue;

                // Decoding into a field keeps the noise on the group allocation-free; only a real
                // announcement pays for a string, and that string is the MOTD the caller asked for.
                var count = Encoding.UTF8.GetChars(_buffer, 0, length, _text, 0);
                if (TryParse(_text.AsSpan(0, count), out var motd, out var port))
                    yield return new LanServer(source, motd, port);
            }
        }
        finally
        {
            Volatile.Write(ref _listening, 0);
        }
    }

    /// <summary>
    ///     Collects everything announced during <paramref name="window" />, each world once, then returns.
    ///     Worlds announce every 1.5 seconds, so a window under two seconds can come back empty.
    ///     <para>
    ///     Two announcements are the same world when <see cref="LanServer.EndPoint" /> matches. The source
    ///     port cannot take part: an announcer that opens a fresh socket each time is the same world.
    ///     </para>
    /// </summary>
    public static Task<IReadOnlyList<LanServer>> DiscoverAsync(TimeSpan window,
        CancellationToken cancellationToken = default)
    {
        return DiscoverAsync(window, null, cancellationToken);
    }

    /// <inheritdoc cref="DiscoverAsync(TimeSpan, CancellationToken)" />
    /// <param name="window">How long to listen.</param>
    /// <param name="localInterface">Local IPv4 address of the interface to listen on, or all when null.</param>
    /// <param name="cancellationToken">Cancels the collection.</param>
    public static async Task<IReadOnlyList<LanServer>> DiscoverAsync(TimeSpan window, IPAddress? localInterface,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(window, TimeSpan.Zero);

        var detector = new LanServerDetector(localInterface);
        await using (detector.ConfigureAwait(false))
        {
            return await CollectAsync(detector, window, cancellationToken).ConfigureAwait(false);
        }
    }

    private static async Task<IReadOnlyList<LanServer>> CollectAsync(LanServerDetector detector, TimeSpan window,
        CancellationToken cancellationToken)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(window);

        var found = new List<LanServer>();
        var seen = new HashSet<IPEndPoint>();
        try
        {
            await foreach (var server in detector.ListenAsync(timeout.Token).ConfigureAwait(false))
                if (seen.Add(server.EndPoint))
                    found.Add(server);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            // The window closed, which is how this method is meant to end.
        }

        return found;
    }

    /// <summary>
    ///     Reads <c>[MOTD]…[/MOTD][AD]…[/AD]</c>. Lenient on purpose: a missing closing tag, a missing MOTD,
    ///     surrounding noise, and an <c>address:port</c> in place of a bare port are all accepted. A payload
    ///     without a usable port is not an announcement.
    /// </summary>
    internal static bool TryParse(ReadOnlySpan<char> payload, out string motd, out int port)
    {
        motd = string.Empty;
        port = 0;

        var motdStart = payload.IndexOf("[MOTD]", StringComparison.Ordinal);
        if (motdStart >= 0)
        {
            var body = payload[(motdStart + 6)..];
            var motdEnd = body.IndexOf("[/MOTD]", StringComparison.Ordinal);
            if (motdEnd < 0) motdEnd = body.IndexOf("[AD]", StringComparison.Ordinal);
            motd = (motdEnd >= 0 ? body[..motdEnd] : body).ToString();
        }

        var adStart = payload.IndexOf("[AD]", StringComparison.Ordinal);
        if (adStart < 0) return false;

        var tail = payload[(adStart + 4)..];
        var adEnd = tail.IndexOf("[/AD]", StringComparison.Ordinal);
        var address = (adEnd >= 0 ? tail[..adEnd] : tail).Trim();

        var colon = address.LastIndexOf(':');
        if (colon >= 0) address = address[(colon + 1)..];

        if (!ushort.TryParse(address, NumberStyles.None, CultureInfo.InvariantCulture, out var parsed) || parsed == 0)
            return false;

        port = parsed;
        return true;
    }

    private static Socket CreateSocket(IPAddress? localInterface)
    {
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        try
        {
            // Other listeners on this machine — another bot, the game itself — must still get the group.
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            socket.Bind(new IPEndPoint(IPAddress.Any, MulticastPort));

            if (localInterface is not null)
                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
                    new MulticastOption(MulticastGroup, localInterface));
            else
                JoinEveryInterface(socket);

            return socket;
        }
        catch
        {
            socket.Dispose();
            throw;
        }
    }

    private static void JoinEveryInterface(Socket socket)
    {
        var joined = false;

        foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (adapter.OperationalStatus != OperationalStatus.Up || !adapter.SupportsMulticast) continue;

            try
            {
                var properties = adapter.GetIPProperties().GetIPv4Properties();
                if (properties is null) continue;

                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
                    new MulticastOption(MulticastGroup, properties.Index));
                joined = true;
            }
            catch (NetworkInformationException)
            {
            }
            catch (SocketException)
            {
            }
        }

        // Nothing enumerable to join to: let the stack pick the default interface.
        if (!joined)
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
                new MulticastOption(MulticastGroup, IPAddress.Any));
    }

    /// <summary>
    ///     One datagram, or <see langword="null" /> when the listen is over because the detector was
    ///     disposed. Cancellation still surfaces, so a cancelled listen ends the way callers expect.
    /// </summary>
    private async ValueTask<(int Length, IPEndPoint Source)?> ReceiveAsync(CancellationToken cancellationToken)
    {
        var any = new IPEndPoint(IPAddress.Any, 0);
        while (true)
        {
            try
            {
                var result = await _socket.ReceiveFromAsync(_buffer, SocketFlags.None, any, cancellationToken)
                    .ConfigureAwait(false);
                return (result.ReceivedBytes, (IPEndPoint)result.RemoteEndPoint);
            }
            catch (SocketException e) when (e.SocketErrorCode is SocketError.ConnectionReset
                                                or SocketError.MessageSize
                                                or SocketError.NetworkReset)
            {
                // An ICMP bounce or an oversized datagram kills one read, not the listener.
            }
            catch (Exception e) when (e is ObjectDisposedException
                                          or SocketException { SocketErrorCode: SocketError.OperationAborted })
            {
                // DisposeAsync closed the socket underneath the read: a clean end, not a fault.
                return null;
            }
        }
    }
}
