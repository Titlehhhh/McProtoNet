using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace McProtoNet;

/// <summary>
/// Provides a listener for the "open to LAN" announcements a Minecraft client sends every 1.5 seconds as
/// <c>[MOTD]…[/MOTD][AD]…[/AD]</c> in UDP datagrams to the multicast group 224.0.2.60:4445.
/// </summary>
/// <remarks>
/// <para>
/// The detector owns the socket, not the enumerator. The constructor binds port 4445 and joins the
/// group; <see cref="DisposeAsync"/> closes the socket and ends any listen in progress. Disposing an
/// enumerator returned by <see cref="ListenAsync"/> does not free the port.
/// </para>
/// <para>
/// <see cref="ListenAsync"/> yields every announcement as it arrives, so the same world appears
/// repeatedly. <see cref="DiscoverAsync(TimeSpan, CancellationToken)"/> is the one-shot form and removes
/// the repeats itself.
/// </para>
/// </remarks>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="LanServerDetector"/> class that listens on every
    /// interface that carries multicast.
    /// </summary>
    /// <exception cref="SocketException">The port could not be bound, or no group could be joined.</exception>
    public LanServerDetector() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LanServerDetector"/> class that listens on the
    /// specified interface.
    /// </summary>
    /// <param name="localInterface">The local IPv4 address of the interface to listen on, or
    /// <see langword="null"/> to listen on every interface that carries multicast.</param>
    /// <exception cref="ArgumentException"><paramref name="localInterface"/> is not an IPv4
    /// address.</exception>
    /// <exception cref="SocketException">The port could not be bound, or the group could not be
    /// joined.</exception>
    public LanServerDetector(IPAddress? localInterface)
    {
        if (localInterface is not null && localInterface.AddressFamily != AddressFamily.InterNetwork)
            throw new ArgumentException("The LAN announcement group is IPv4; the interface address must be too.",
                nameof(localInterface));

        _socket = CreateSocket(localInterface);
    }

    /// <summary>
    /// Gets the multicast group worlds announce to.
    /// </summary>
    public static IPAddress MulticastGroup { get; } = IPAddress.Parse("224.0.2.60");

    /// <summary>
    /// Asynchronously releases all resources used by the current instance of the
    /// <see cref="LanServerDetector"/> class.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    /// <remarks>
    /// The socket is closed and the port is freed. A listen in progress ends without an error.
    /// </remarks>
    public ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 0) _socket.Dispose();
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Returns an asynchronous sequence of announcements as they arrive.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. This parameter has
    /// no default value.</param>
    /// <returns>An asynchronous sequence of the announcements received.</returns>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="InvalidOperationException">Another listen is already running on this
    /// detector.</exception>
    /// <remarks>
    /// The sequence ends when the token is canceled, when the caller stops enumerating, or when the
    /// detector is disposed. Malformed datagrams are dropped silently.
    /// </remarks>
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
    /// Asynchronously collects the worlds announced during the specified window, listening on every
    /// interface that carries multicast.
    /// </summary>
    /// <param name="window">How long to listen. This value must be greater than
    /// <see cref="TimeSpan.Zero"/>.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is
    /// <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous discovery operation. The result contains each
    /// announced world once.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="window"/> is less than or equal to
    /// <see cref="TimeSpan.Zero"/>.</exception>
    /// <exception cref="SocketException">The port could not be bound, or no group could be
    /// joined.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is
    /// stored into the returned task.</exception>
    /// <remarks>
    /// Worlds announce every 1.5 seconds, so a window shorter than two seconds can return an empty list.
    /// Two announcements are treated as the same world when <see cref="LanServer.EndPoint"/> matches; the
    /// source port is not compared.
    /// </remarks>
    public static Task<IReadOnlyList<LanServer>> DiscoverAsync(TimeSpan window,
        CancellationToken cancellationToken = default)
    {
        return DiscoverAsync(window, null, cancellationToken);
    }

    /// <summary>
    /// Asynchronously collects the worlds announced during the specified window, listening on the
    /// specified interface.
    /// </summary>
    /// <param name="window">How long to listen. This value must be greater than
    /// <see cref="TimeSpan.Zero"/>.</param>
    /// <param name="localInterface">The local IPv4 address of the interface to listen on, or
    /// <see langword="null"/> to listen on every interface that carries multicast.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is
    /// <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous discovery operation. The result contains each
    /// announced world once.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="window"/> is less than or equal to
    /// <see cref="TimeSpan.Zero"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="localInterface"/> is not an IPv4
    /// address.</exception>
    /// <exception cref="SocketException">The port could not be bound, or the group could not be
    /// joined.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is
    /// stored into the returned task.</exception>
    /// <remarks>
    /// Worlds announce every 1.5 seconds, so a window shorter than two seconds can return an empty list.
    /// Two announcements are treated as the same world when <see cref="LanServer.EndPoint"/> matches; the
    /// source port is not compared.
    /// </remarks>
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

    /// <summary>Attempts to parse an <c>[MOTD]…[/MOTD][AD]…[/AD]</c> payload into an MOTD and a port.</summary>
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

    // Returns one datagram, or null when the listen is over because the detector was disposed.
    // Cancellation still surfaces as an exception.
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
