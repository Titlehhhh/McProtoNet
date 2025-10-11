using System.Net.Sockets;
using System.Runtime.CompilerServices;
using McProtoNet.Abstractions;
using McProtoNet.Cryptography;
using McProtoNet.Net;


namespace McProtoNet.Client;

/// <summary>
///     Represents a Minecraft client capable of connecting to a server, sending and receiving packets.
/// </summary>
public class MinecraftClient : IMinecraftClient
{
    private enum State
    {
        None = 0,
        Connecting = 1,
        Connected = 2,
        Disposed = 3
    }

    /// <summary>
    ///     Creates a new instance of <see cref="MinecraftClient" /> with the specified options.
    /// </summary>
    /// <param name="options">The configuration options for the client.</param>
    public MinecraftClient(MinecraftClientStartOptions options)
    {
        StartOptions = options;
    }

    #region Properties

    /// <summary>
    ///     Gets a value indicating whether the client is currently connected to the Minecraft server.
    /// </summary>
    /// <value>
    ///     <c>true</c> if connected; otherwise, <c>false</c>.
    /// </value>
    public bool IsConnected => _state == (int)State.Connected;
    /// <summary>
    ///     Gets the startup configuration options used to initialize this client instance.
    /// </summary>
    /// <value>Read-only startup options.</value>
    public MinecraftClientStartOptions StartOptions { get; }
    /// <summary>
    ///     Gets the protocol version number used by this client.
    /// </summary>
    /// <value>
    ///     Protocol version derived from <see cref="MinecraftClientStartOptions.Version" />.
    /// </value>
    public int ProtocolVersion => StartOptions.Version;

    #endregion


    #region Fields

    private State CurrentState => (State)_state;

    private volatile int _state;

    private readonly MinecraftPacketReader _packetReader = new();
    private readonly MinecraftPacketSender _packetSender = new();
    private AesStream? _mainStream;

    #endregion

    #region Methods

    /// <summary>
    ///     Sends a packet to the Minecraft server asynchronously.
    /// </summary>
    /// <param name="packet">The packet to send.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask" /> representing the asynchronous send operation.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the client has been disposed.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the client is not connected.</exception>
    public async ValueTask SendPacket(OutputPacket packet, CancellationToken cancellationToken = default)
    {
        var state = CurrentState;

        switch (state)
        {
            case State.Disposed:
                ThrowDisposed();
                break;
        }

        if (state != State.Connected)
            ThrowNotConnected();
        try
        {
            await _packetSender.SendAndDisposeAsync(packet, cancellationToken);
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private State CompareExchange(State value, State comparand)
    {
        return (State)Interlocked.CompareExchange(ref _state, (int)value, (int)comparand);
    }

    /// <summary>
    ///     Establishes a connection to the Minecraft server asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the connection attempt.</param>
    /// <returns>A <see cref="ValueTask" /> representing the connection operation.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the client has been disposed.</exception>
    /// <exception cref="InvalidOperationException">Thrown if already connecting or connected.</exception>
    public async ValueTask ConnectAsync(CancellationToken cancellationToken)
    {
        var state = CompareExchange(State.Connecting, State.None);


        if (state == State.Disposed)
        {
            ThrowDisposed();
        }

        if (state != State.None)
        {
            throw new InvalidOperationException("Already connecting or connected");
        }

        try
        {
            Stream stream = await ConnectInternal(StartOptions, cancellationToken);
            AesStream aesStream = new(stream);
            Interlocked.Exchange(ref _mainStream, aesStream)?.Dispose();

            _packetReader.BaseStream = aesStream;
            _packetSender.BaseStream = aesStream;


            state = CompareExchange(State.Connected, State.Connecting);
            if (state != State.Connecting)
            {
                await aesStream.DisposeAsync();
            }
        }
        catch
        {
            Dispose();
            throw;
        }
    }

    private async ValueTask<Stream> ConnectInternal(MinecraftClientStartOptions startOptions,
        CancellationToken cancellationToken = default)
    {
        using CancellationTokenSource timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(startOptions.ConnectTimeout);
        if (startOptions.Proxy is not null)
        {
            var stream = await startOptions.Proxy.ConnectAsync(startOptions.Host, startOptions.Port, timeout.Token);
            stream.WriteTimeout = (int)startOptions.WriteTimeout.TotalMilliseconds;
            stream.ReadTimeout = (int)startOptions.ReadTimeout.TotalMilliseconds;
            return stream;
        }

        {
            TcpClient tcpClient = new TcpClient();

            await using var _ = cancellationToken.Register(d => ((TcpClient)d)?.Dispose(), tcpClient);

            tcpClient.ReceiveTimeout = (int)startOptions.ReadTimeout.TotalMilliseconds;
            tcpClient.SendTimeout = (int)startOptions.WriteTimeout.TotalMilliseconds;
            try
            {
                await tcpClient.ConnectAsync(startOptions.Host, startOptions.Port, timeout.Token);
            }
            catch
            {
                tcpClient.Dispose();
                throw;
            }


            return tcpClient.GetStream();
        }
    }
    /// <summary>
    ///     Starts receiving packets from the server asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel packet reception.</param>
    /// <returns>An asynchronous enumerable of received packets.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the client has been disposed.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the client is not connected.</exception>
    public IAsyncEnumerable<InputPacket> ReceivePackets(CancellationToken cancellationToken)
    {
        var state = CurrentState;
        if (state == State.Disposed)
        {
            ThrowDisposed();
        }

        if (state != State.Connected)
        {
            ThrowNotConnected();
        }

        return ReceivePacketsCore(cancellationToken);
    }


    private async IAsyncEnumerable<InputPacket> ReceivePacketsCore(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            InputPacket packet;
            try
            {
                packet = await _packetReader.ReadNextPacketAsync(cancellationToken);
            }
            catch
            {
                Dispose();
                throw;
            }

            try
            {
                yield return packet;
            }
            finally
            {
                packet.Dispose();
            }
        }
    }

    /// <summary>
    ///     Enables or disables packet compression.
    /// </summary>
    /// <param name="threshold">
    ///     Compression threshold in bytes.
    ///     Packets larger than this size will be compressed (set to 0 to disable).
    /// </param>
    /// <exception cref="ObjectDisposedException">Thrown if the client has been disposed.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the client is not connected.</exception>
    public void SwitchCompression(int threshold)
    {
        var state = CurrentState;
        switch (state)
        {
            case State.Disposed:
                ThrowDisposed();
                break;
        }

        if (state != State.Connected)
            ThrowNotConnected();


        _packetSender.SwitchCompression(threshold);
        _packetReader.SwitchCompression(threshold);
    }

    /// <summary>
    ///     Enables packet encryption using the specified private key.
    /// </summary>
    /// <param name="privateKey">The 16-byte private key for AES encryption.</param>
    /// <exception cref="ObjectDisposedException">Thrown if the client has been disposed.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the client is not connected.</exception>
    public void SwitchEncryption(Span<byte> privateKey)
    {
        var state = CurrentState;
        switch (state)
        {
            case State.Disposed:
                ThrowDisposed();
                break;
        }

        if (state != State.Connected)
            ThrowNotConnected();


        _mainStream!.SwitchEncryption(privateKey.ToArray());
    }

    #endregion

    #region Dispose

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ThrowDisposed() => throw new ObjectDisposedException(string.Empty);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ThrowNotConnected() => throw new InvalidOperationException("Not connected");


    /// <summary>
    ///     Disposes the client.
    /// </summary>
    public void Dispose()
    {
        int state = Interlocked.Exchange(ref _state, (int)State.Disposed);
        if (state == (int)State.Disposed)
            return;

        _mainStream?.Dispose();
        _packetReader.BaseStream = null;
        _packetSender.BaseStream = null;
    }

    #endregion
}