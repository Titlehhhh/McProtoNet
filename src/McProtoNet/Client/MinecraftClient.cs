using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using McProtoNet.Primitives;
using McProtoNet.Protocol;
using McProtoNet.Transport;

namespace McProtoNet;

/// <summary>
///     The standard client: connect, send a packet, read packets, turn on encryption and
///     compression. One frame per read and one frame per write for the whole session — simple, not
///     fast. Whoever needs the fast path takes <see cref="Connection" /> and calls
///     <see cref="MinecraftConnection.ToStreaming" /> itself.
///     <para>
///     Everything above raw packets — handshake, login, the phase the connection is in — is
///     consumer code (see examples/).
///     </para>
/// </summary>
/// <remarks>
///     Sends are serialized by one gate inside, so any number of threads may call
///     <see cref="SendAsync{T}" /> at once. Reads are not: one reader at a time, as the transport
///     requires.
/// </remarks>
public sealed class MinecraftClient : IAsyncDisposable
{
    /// <summary>How long <see cref="DisposeAsync" /> waits for an in-flight send before giving up on it.</summary>
    private static readonly TimeSpan DisposeGateBudget = TimeSpan.FromSeconds(5);

    private readonly SemaphoreSlim _sendGate = new(1, 1);

    private TcpClient? _tcp;
    private MinecraftConnection? _connection;
    private int _connecting;
    private int _disposed;

    public MinecraftClient(MinecraftClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Host, nameof(options));
        ArgumentOutOfRangeException.ThrowIfLessThan(options.Port, 1, nameof(options));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(options.Port, 65535, nameof(options));
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(options.ConnectTimeout, TimeSpan.Zero, nameof(options));
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(options.SrvTimeout, TimeSpan.Zero, nameof(options));

        Options = options;
    }

    public MinecraftClientOptions Options { get; }

    /// <summary>
    ///     The transport underneath — the escape hatch for everything this client does not do:
    ///     <see cref="MinecraftConnection.ToStreaming" />, <see cref="MinecraftConnection.BaseStream" />,
    ///     <see cref="MinecraftConnection.Completion" />. Using it at the same time as the client's own
    ///     calls is the caller's problem: the send gate here does not know about it, and a connection
    ///     moved to streaming leaves every member of this client throwing.
    /// </summary>
    public MinecraftConnection Connection =>
        _connection ?? throw new InvalidOperationException("The client is not connected. Call ConnectAsync first.");

    /// <summary>True between a successful <see cref="ConnectAsync" /> and the close of the connection.</summary>
    public bool IsConnected =>
        Volatile.Read(ref _disposed) == 0 && _connection is { } connection && !connection.Completion.IsCompleted;

    /// <summary>
    ///     Negative means no compression envelope. A change takes effect from the next frame. Set it
    ///     between two frames — from the read loop itself, not from another thread while a read is
    ///     parked, which the transport refuses.
    /// </summary>
    public int CompressionThreshold
    {
        get => Connection.CompressionThreshold;
        set => Connection.CompressionThreshold = value;
    }

    /// <summary>True once <see cref="EnableEncryption" /> has run.</summary>
    public bool IsEncrypted => Connection.IsEncrypted;

    /// <summary>
    ///     Resolves the address (SRV first, as vanilla does), opens the TCP connection and takes
    ///     ownership of it. No packets are sent.
    /// </summary>
    /// <exception cref="TimeoutException"><see cref="MinecraftClientOptions.ConnectTimeout" /> ran out.</exception>
    /// <exception cref="OperationCanceledException">The caller's own token was cancelled.</exception>
    public async ValueTask ConnectAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) == 1, this);
        if (Interlocked.CompareExchange(ref _connecting, 1, 0) == 1)
            throw new InvalidOperationException("The client is already connected.");

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(Options.ConnectTimeout);

        TcpClient? tcp = null;
        try
        {
            var (host, port) = await ResolveAsync(timeout.Token).ConfigureAwait(false);

            tcp = new TcpClient { NoDelay = Options.NoDelay };
            await tcp.ConnectAsync(host, port, timeout.Token).ConfigureAwait(false);

            var connection = new MinecraftConnection(tcp.GetStream());
            _tcp = tcp;
            _connection = connection;

            // a dispose that ran while we were connecting found nothing to close: close it here,
            // or the socket this call just opened would outlive the client
            if (Volatile.Read(ref _disposed) == 1)
            {
                await connection.DisposeAsync().ConfigureAwait(false);
                throw new ObjectDisposedException(nameof(MinecraftClient));
            }
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            Failed(tcp);
            throw new TimeoutException(
                $"Connecting to {Options.Host}:{Options.Port} timed out after {Options.ConnectTimeout}.");
        }
        catch (OperationCanceledException)
        {
            Failed(tcp);

            // the exception in hand carries the linked token; the caller must see its own
            cancellationToken.ThrowIfCancellationRequested();
            throw;
        }
        catch
        {
            Failed(tcp);
            throw;
        }

        void Failed(TcpClient? opened)
        {
            opened?.Dispose();
            Volatile.Write(ref _connecting, 0);
        }
    }

    /// <summary>
    ///     Serializes the packet, takes its id from the type and writes one frame. When the call
    ///     returns the bytes are at the socket. Callers are serialized by one gate, so a keep-alive
    ///     answered from a timer never lands in the middle of somebody else's frame.
    /// </summary>
    /// <exception cref="ProtocolNotSupportException">The packet has no id on this protocol version.</exception>
    public async ValueTask SendAsync<T>(T packet, int protocolVersion, CancellationToken cancellationToken = default)
        where T : class, IPacket<T>
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) == 1, this);
        var connection = Connection;
        await _sendGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await connection.SendAsync(packet, protocolVersion, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _sendGate.Release();
        }
    }

    /// <summary>The same gate, with the id and the body given by hand: replays, fuzzing, packets outside the specs.</summary>
    public async ValueTask SendRawAsync(int id, ReadOnlyMemory<byte> body,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) == 1, this);
        var connection = Connection;
        await _sendGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await connection.WritePacketAsync(id, body, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _sendGate.Release();
        }
    }

    /// <summary>
    ///     Reads exactly one frame. <see cref="IncomingPacket.Body" /> is a window owned by the
    ///     transport and stays valid only until the next read — decode it before reading again.
    /// </summary>
    public ValueTask<IncomingPacket> ReadPacketAsync(CancellationToken cancellationToken = default) =>
        Connection.ReadPacketAsync(cancellationToken);

    /// <summary>
    ///     Every frame until the stream ends. A clean end of stream — the server closed, or this
    ///     client did — ends the enumeration; an abort or any other failure comes out as an exception.
    ///     Each body stays valid only until the next step of the enumeration.
    /// </summary>
    public async IAsyncEnumerable<IncomingPacket> ReadPacketsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var connection = Connection;
        while (await ReadOrEndAsync(connection, cancellationToken).ConfigureAwait(false) is { } packet)
            yield return packet;
    }

    /// <summary>
    ///     Turns on AES/CFB8 in both directions from the next frame on. Call it from the read loop,
    ///     right after the awaited send of the frame that agreed the secret: the transport refuses the
    ///     switch while a read is parked, and refusing it halfway leaves the connection unusable.
    /// </summary>
    public void EnableEncryption(ReadOnlySpan<byte> sharedSecret) => Connection.EnableEncryption(sharedSecret);

    /// <summary>Closes the connection from any thread; an in-flight read or write fails with the reason.</summary>
    public void Abort(Exception? reason = null) => _connection?.Abort(reason);

    /// <summary>
    ///     Kills the stream first so a send parked on a full socket fails now instead of holding the
    ///     gate, then takes the gate before the transport gives its pooled buffers back: a write still
    ///     inside the transport and a buffer already returned to the pool must not overlap. A
    ///     connection handed to <see cref="MinecraftConnection.ToStreaming" /> is no longer this
    ///     client's to close.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 1) return;

        var connection = _connection;
        if (connection is null)
        {
            _tcp?.Dispose();
            return;
        }

        if (!connection.Completion.IsCompleted) connection.Abort();

        var fenced = await _sendGate.WaitAsync(DisposeGateBudget).ConfigureAwait(false);
        try
        {
            await connection.DisposeAsync().ConfigureAwait(false);
        }
        finally
        {
            if (fenced) _sendGate.Release();
        }
    }

    /// <summary>Null means the enumeration is over: the stream ended, or this client closed it.</summary>
    private async ValueTask<IncomingPacket?> ReadOrEndAsync(MinecraftConnection connection, CancellationToken token)
    {
        try
        {
            return await connection.ReadPacketAsync(token).ConfigureAwait(false);
        }
        catch (EndOfStreamException)
        {
            return null;
        }
        catch (Exception ex) when (Volatile.Read(ref _disposed) == 1 &&
                                   ex is ConnectionAbortedException or ObjectDisposedException)
        {
            return null;
        }
    }

    /// <summary>
    ///     Vanilla's rule: an SRV lookup only when the user left the port alone and typed a name, not an
    ///     address. A dead resolver is not a dead server, so the lookup gets its own small budget and every
    ///     way it can fail — silence, a broken answer, an unencodable name — ends at the host the user typed.
    ///     Only the caller's own cancellation and the connect timeout come back out.
    /// </summary>
    private async ValueTask<(string Host, int Port)> ResolveAsync(CancellationToken connectToken)
    {
        if (!Options.UseSrv || Options.Port != MinecraftClientOptions.DefaultPort) return (Options.Host, Options.Port);
        if (IPAddress.TryParse(Options.Host, out _)) return (Options.Host, Options.Port);

        using var budget = CancellationTokenSource.CreateLinkedTokenSource(connectToken);
        budget.CancelAfter(Options.SrvTimeout < Options.ConnectTimeout ? Options.SrvTimeout : Options.ConnectTimeout);

        try
        {
            var record = await SrvResolver.ResolveAsync(Options.Host, budget.Token).ConfigureAwait(false);
            return record is { } srv ? (srv.Target, srv.Port) : (Options.Host, Options.Port);
        }
        catch (OperationCanceledException) when (!connectToken.IsCancellationRequested)
        {
            return (Options.Host, Options.Port);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // every way the lookup can fail ends here, including the ones that are not IO: reading
            // the machine's resolver list throws NetworkInformationException, which is neither an
            // IOException nor a SocketException, and a dead resolver is still not a dead server
            return (Options.Host, Options.Port);
        }
    }
}
