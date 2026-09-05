using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using McProtoNet.Primitives;
using McProtoNet.Protocol;
using McProtoNet.Transport;

namespace McProtoNet;

/// <summary>
/// Provides a Minecraft protocol client that connects to a server, sends and reads single packet
/// frames, and switches encryption and compression on.
/// </summary>
/// <remarks>
/// <para>
/// The client reads and writes one frame per call for the whole session. To use the streaming
/// path, take <see cref="Connection"/> and call <see cref="MinecraftConnection.ToStreaming"/>.
/// </para>
/// <para>
/// The handshake, the login sequence and the current protocol phase are not handled by this type;
/// they belong to consumer code.
/// </para>
/// <para>
/// Sends are serialized by an internal gate, so multiple threads can call
/// <see cref="SendAsync{T}"/> concurrently. Reads are not synchronized: the transport allows one
/// reader at a time.
/// </para>
/// </remarks>
public sealed class MinecraftClient : IAsyncDisposable
{
    // How long DisposeAsync waits for an in-flight send before giving up on it.
    private static readonly TimeSpan DisposeGateBudget = TimeSpan.FromSeconds(5);

    private readonly SemaphoreSlim _sendGate = new(1, 1);

    private TcpClient? _tcp;
    private MinecraftConnection? _connection;
    private int _connecting;
    private int _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="MinecraftClient"/> class with the specified options.
    /// </summary>
    /// <param name="options">The connection options to use. The instance is not copied.</param>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is <see langword="null"/>.
    /// -or-
    /// <see cref="MinecraftClientOptions.Host"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <see cref="MinecraftClientOptions.Host"/> is empty or consists only of white space.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <see cref="MinecraftClientOptions.Port"/> is less than 1 or greater than 65535.
    /// -or-
    /// <see cref="MinecraftClientOptions.ConnectTimeout"/> is less than or equal to
    /// <see cref="TimeSpan.Zero"/>.
    /// -or-
    /// <see cref="MinecraftClientOptions.SrvTimeout"/> is less than or equal to
    /// <see cref="TimeSpan.Zero"/>.
    /// </exception>
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

    /// <summary>
    /// Gets the options this instance was created with.
    /// </summary>
    public MinecraftClientOptions Options { get; }

    /// <summary>
    /// Gets the underlying <see cref="MinecraftConnection"/>.
    /// </summary>
    /// <remarks>
    /// This connection exposes members the client does not wrap, such as
    /// <see cref="MinecraftConnection.ToStreaming"/>, <see cref="MinecraftConnection.BaseStream"/> and
    /// <see cref="MinecraftConnection.Completion"/>. The client does not synchronize its own send
    /// operations with direct use of this connection. After the connection is switched to streaming
    /// mode, all members of this client throw <see cref="InvalidOperationException"/>.
    /// </remarks>
    /// <exception cref="InvalidOperationException">The client is not connected.</exception>
    public MinecraftConnection Connection =>
        _connection ?? throw new InvalidOperationException("The client is not connected. Call ConnectAsync first.");

    /// <summary>
    /// Gets a value indicating whether the client is connected.
    /// </summary>
    /// <value>
    /// <see langword="true"/> between a successful call to <see cref="ConnectAsync"/> and the end of
    /// the connection; otherwise, <see langword="false"/>.
    /// </value>
    public bool IsConnected =>
        Volatile.Read(ref _disposed) == 0 && _connection is { } connection && !connection.Completion.IsCompleted;

    /// <summary>
    /// Gets or sets the compression threshold, in bytes. A negative value disables compression.
    /// </summary>
    /// <remarks>
    /// A new value applies from the next frame in both directions.
    /// </remarks>
    /// <exception cref="InvalidOperationException">The client is not connected.
    /// -or-
    /// The connection was moved to streaming mode by
    /// <see cref="MinecraftConnection.ToStreaming"/>.</exception>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="ConnectionAbortedException">The connection is closed.</exception>
    public int CompressionThreshold
    {
        get => Connection.CompressionThreshold;
        set => Connection.CompressionThreshold = value;
    }

    /// <summary>
    /// Gets a value indicating whether encryption is enabled on the connection.
    /// </summary>
    /// <value>
    /// <see langword="true"/> after <see cref="EnableEncryption"/> has been called; otherwise,
    /// <see langword="false"/>.
    /// </value>
    /// <exception cref="InvalidOperationException">The client is not connected.</exception>
    public bool IsEncrypted => Connection.IsEncrypted;

    /// <summary>
    /// Asynchronously resolves the server address and opens the connection.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is
    /// <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous connect operation.</returns>
    /// <exception cref="InvalidOperationException">The client is already connected.</exception>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="TimeoutException">The connection was not established within
    /// <see cref="MinecraftClientOptions.ConnectTimeout"/>.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is
    /// stored into the returned task.</exception>
    /// <remarks>
    /// The address is resolved through an SRV lookup first, as the vanilla client does. When
    /// <see cref="MinecraftClientOptions.Proxy"/> is set, the connection is opened through the proxy to the
    /// same resolved host and port. The client takes ownership of the connection. No packets are sent.
    /// </remarks>
    public async ValueTask ConnectAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) == 1, this);
        if (Interlocked.CompareExchange(ref _connecting, 1, 0) == 1)
            throw new InvalidOperationException("The client is already connected.");

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(Options.ConnectTimeout);

        TcpClient? tcp = null;
        Stream? stream = null;
        try
        {
            var (host, port) = await ResolveAsync(timeout.Token).ConfigureAwait(false);

            if (Options.Proxy is { } proxy)
            {
                stream = await proxy.ConnectAsync(host, port, timeout.Token).ConfigureAwait(false);
            }
            else
            {
                tcp = Options.LocalEndPoint is { } local
                    ? new TcpClient(local) { NoDelay = Options.NoDelay }
                    : new TcpClient { NoDelay = Options.NoDelay };
                await tcp.ConnectAsync(host, port, timeout.Token).ConfigureAwait(false);
                stream = tcp.GetStream();
            }

            var connection = new MinecraftConnection(stream);
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
            Failed(tcp, stream);
            throw new TimeoutException(
                $"Connecting to {Options.Host}:{Options.Port} timed out after {Options.ConnectTimeout}.");
        }
        catch (OperationCanceledException)
        {
            Failed(tcp, stream);

            // the exception in hand carries the linked token; the caller must see its own
            cancellationToken.ThrowIfCancellationRequested();
            throw;
        }
        catch
        {
            Failed(tcp, stream);
            throw;
        }

        void Failed(TcpClient? opened, Stream? openedStream)
        {
            openedStream?.Dispose();
            opened?.Dispose();
            Volatile.Write(ref _connecting, 0);
        }
    }

    /// <summary>
    /// Asynchronously serializes the specified packet and writes it as one frame.
    /// </summary>
    /// <typeparam name="T">The packet type. Its wire id is taken from the type.</typeparam>
    /// <param name="packet">The packet to send.</param>
    /// <param name="protocolVersion">The protocol version to serialize for.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is
    /// <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous send operation. When it completes, the bytes have
    /// been handed to the socket.</returns>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="InvalidOperationException">The client is not connected.</exception>
    /// <exception cref="ProtocolNotSupportException">The packet has no id on the specified protocol
    /// version.</exception>
    /// <exception cref="ConnectionAbortedException">The connection is closed, or the stream
    /// failed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is
    /// stored into the returned task.</exception>
    /// <remarks>
    /// Callers are serialized by a single gate, so a frame written from another thread cannot be
    /// interleaved with this one.
    /// </remarks>
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

    /// <summary>
    /// Asynchronously writes one frame from the specified packet id and body.
    /// </summary>
    /// <param name="id">The wire id of the packet.</param>
    /// <param name="body">The already serialized packet body, without the id.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is
    /// <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="InvalidOperationException">The client is not connected.</exception>
    /// <exception cref="ConnectionAbortedException">The connection is closed, or the stream
    /// failed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is
    /// stored into the returned task.</exception>
    /// <remarks>
    /// This method uses the same send gate as <see cref="SendAsync{T}"/>.
    /// </remarks>
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
    /// Asynchronously reads exactly one frame.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is
    /// <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous read operation. The result contains the packet that
    /// was read.</returns>
    /// <exception cref="InvalidOperationException">The client is not connected.
    /// -or-
    /// Another read is already in progress.</exception>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="ConnectionAbortedException">The connection is closed, or the stream
    /// failed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is
    /// stored into the returned task.</exception>
    /// <remarks>
    /// The packet owns the pooled block behind <see cref="IncomingPacket.Body"/>: dispose it when it is
    /// no longer needed, or keep it as long as needed.
    /// </remarks>
    public ValueTask<IncomingPacket> ReadPacketAsync(CancellationToken cancellationToken = default) =>
        Connection.ReadPacketAsync(cancellationToken);

    /// <summary>
    /// Returns an asynchronous sequence of every frame read until the connection ends.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is
    /// <see cref="CancellationToken.None"/>.</param>
    /// <returns>An asynchronous sequence of the packets read from the connection.</returns>
    /// <exception cref="InvalidOperationException">The client is not connected.
    /// -or-
    /// Another read is already in progress.</exception>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="ConnectionAbortedException">The connection is closed, or the stream
    /// failed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is
    /// stored into the returned task.</exception>
    /// <remarks>
    /// <para>
    /// The enumeration never ends without an exception. A server that closed cleanly raises
    /// <see cref="EndOfStreamException"/>. Aborting the connection, including through
    /// <see cref="DisposeAsync"/>, raises <see cref="ConnectionAbortedException"/> for a read already in
    /// progress, or <see cref="ObjectDisposedException"/> for a read started after disposal has completed.
    /// </para>
    /// <para>
    /// Each packet is borrowed for one step of the enumeration: the enumeration owns it and releases
    /// its buffer when the next step begins. To keep a packet longer, call
    /// <see cref="IncomingPacket.Retain"/>.
    /// </para>
    /// </remarks>
    public async IAsyncEnumerable<IncomingPacket> ReadPacketsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var connection = Connection;
        while (true)
        {
            var packet = await connection.ReadPacketAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                yield return packet.Borrow();
            }
            finally
            {
                packet.Dispose();
            }
        }
    }

    /// <summary>
    /// Enables AES/CFB8 encryption in both directions, starting with the next frame.
    /// </summary>
    /// <param name="sharedSecret">The shared secret agreed with the server. This value must be exactly
    /// <see cref="McProtoNet.Transport.Cryptography.PacketCipher.SharedSecretLength"/> bytes long. It serves
    /// as both the key and the initialization vector.</param>
    /// <exception cref="ArgumentException"><paramref name="sharedSecret"/> is not
    /// <see cref="McProtoNet.Transport.Cryptography.PacketCipher.SharedSecretLength"/> bytes
    /// long.</exception>
    /// <exception cref="InvalidOperationException">The client is not connected.
    /// -or-
    /// Encryption is already enabled.</exception>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="ConnectionAbortedException">The connection is closed.</exception>
    /// <remarks>
    /// Call this method after the frame that agreed the secret has been written and before the next frame
    /// is read.
    /// </remarks>
    public void EnableEncryption(ReadOnlySpan<byte> sharedSecret) => Connection.EnableEncryption(sharedSecret);

    /// <summary>
    /// Closes the connection and fails any read or write in progress.
    /// </summary>
    /// <param name="reason">The exception to fail pending operations with, or <see langword="null"/> to use
    /// the default reason.</param>
    /// <remarks>
    /// This method can be called from any thread. It does nothing when the client is not connected.
    /// </remarks>
    public void Abort(Exception? reason = null) => _connection?.Abort(reason);

    /// <summary>
    /// Asynchronously releases all resources used by the current instance of the
    /// <see cref="MinecraftClient"/> class.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    /// <remarks>
    /// The stream is closed first, so a send blocked on a full socket fails instead of holding the send
    /// gate. The gate is then taken before the transport returns its pooled buffers. A connection passed
    /// to <see cref="MinecraftConnection.ToStreaming"/> is no longer owned by this client and is not
    /// closed here.
    /// </remarks>
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

    // SRV lookup only when the port is still the default and the host is a name, not an IP literal.
    // Every lookup failure falls back to the host as typed; only the caller's own cancellation escapes.
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
