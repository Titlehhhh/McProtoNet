using System.Diagnostics;
using System.Net.Sockets;
using System.Reactive;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Channels;
using DotNext.Threading;
using McProtoNet.Abstractions;
using McProtoNet.Cryptography;
using McProtoNet.Net;
using QuickProxyNet;


namespace McProtoNet.Client;

public class CompletionResult
{
    public TimeSpan RunningTime { get; }
}

/// <summary>
///     Represents a Minecraft client.
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
    /// <param name="options">The options for the client.</param>
    public MinecraftClient(MinecraftClientStartOptions options)
    {
        StartOptions = options;
    }

    #region Properties

    public bool IsConnected => _state == (int)State.Connected;
    public MinecraftClientStartOptions StartOptions { get; }
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
    /// Send a packet
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
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

        await _packetSender.SendAndDisposeAsync(packet, cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private State CompareExchange(State value, State comparand)
    {
        return (State)Interlocked.CompareExchange(ref _state, (int)value, (int)comparand);
    }

    /// <summary>
    /// Connects to the Minecraft server asynchronously.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the client is already connecting or connected.</exception>
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
            _mainStream = aesStream;

            _packetReader.BaseStream = aesStream;
            _packetSender.BaseStream = aesStream;

            state = CompareExchange(State.Connected, State.Connecting);
            if (state != State.Connecting)
            {
                await aesStream.DisposeAsync();
            }
        }
        catch (Exception ex)
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
    ///     Switches packet encryption on or off.
    /// </summary>
    /// <param name="privateKey">The private key to use for encryption.</param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the client is disposed, or when the client is stopping.
    /// </exception>
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