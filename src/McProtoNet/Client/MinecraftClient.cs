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
    /// <summary>
    ///     Creates a new instance of <see cref="MinecraftClient" /> with the specified options.
    /// </summary>
    /// <param name="options">The options for the client.</param>
    public MinecraftClient(MinecraftClientStartOptions options)
    {
        StartOptions = options;
    }

    #region Properties

    public bool IsConnected => Volatile.Read(ref _state) == Connected;
    public MinecraftClientStartOptions StartOptions { get; }
    public int ProtocolVersion => StartOptions.Version;
    public Task Completion => _completion.Task;

    #endregion

    #region Constants

    private const int None = 0;
    private const int Connecting = 1;
    private const int Connected = 2;
    private const int Disposed = 3;
    private const int Stopping = 4;

    #endregion

    #region Fields

    private volatile int _state;


    private readonly TaskCompletionSource _completion = new();
    private readonly CancellationTokenSource _aliveClient = new();
    private readonly MinecraftPacketReader _packetReader = new();
    private readonly MinecraftPacketSender _packetSender = new();
    private readonly AsyncReaderWriterLock _sendLock = new();
    private AesStream? _mainStream;

    #endregion

    #region Methods

    /// <summary>
    /// Send a packet
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async ValueTask SendPacket(OutputPacket packet)
    {
        var state = _state;

        switch (state)
        {
            case Disposed:
                ThrowDisposed();
                break;
            case Stopping:
                ThrowStopping();
                break;
        }


        if (state != Connected)
            ThrowNotConnected();

        var holder = await _sendLock.AcquireWriteLockAsync(_aliveClient.Token);
        try
        {
            await _packetSender.SendAndDisposeAsync(packet, _aliveClient.Token);
        }
        catch (Exception ex)
        {
            _completion.TrySetException(ex);
            state = Interlocked.Exchange(ref _state, Stopping);
            if (state == Stopping)
                return;
            Dispose();

            throw;
        }
        finally
        {
            holder.Dispose();
        }
    }

    /// <summary>
    /// Connects to the Minecraft server asynchronously.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the client is already connecting or connected.</exception>
    public async ValueTask ConnectAsync()
    {
        if (Interlocked.CompareExchange(ref _state, Connecting, None) != None)
            throw new InvalidOperationException();

        try
        {
            Stream stream = await ConnectInternal(StartOptions, _aliveClient.Token);
            AesStream aesStream = new(stream);
            _mainStream = aesStream;

            _packetReader.BaseStream = aesStream;
            _packetSender.BaseStream = aesStream;

            Interlocked.Exchange(ref _state, Connected);
        }
        catch (Exception ex)
        {
            _completion.TrySetException(ex);
            int state = Interlocked.Exchange(ref _state, Stopping);
            if (state == Stopping)
                return;
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
        int state = Volatile.Read(ref _state);
        if (state == Disposed)
        {
            ThrowDisposed();
        }

        if (state == Stopping)
        {
            throw new InvalidOperationException();
        }

        if (state != Connected)
        {
            throw new InvalidOperationException();
        }

        return ReceivePacketsCore(cancellationToken);
    }

    private async IAsyncEnumerable<InputPacket> ReceivePacketsCore(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!_aliveClient.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            InputPacket packet;
            try
            {
                packet = await _packetReader.ReadNextPacketAsync(_aliveClient.Token);
            }
            catch (Exception ex)
            {
                _completion.TrySetException(ex);
                Debug.WriteLine("Error in main loop: " + ex);
                int state = Interlocked.Exchange(ref _state, Stopping);
                if (state == Stopping)
                    yield break;
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
        var state = _state;
        switch (state)
        {
            case Disposed:
                ThrowDisposed();
                break;
            case Stopping:
                ThrowStopping();
                break;
        }

        if (state != Connected)
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
        int state = _state;
        switch (state)
        {
            case Disposed:
                ThrowDisposed();
                break;
            case Stopping:
                ThrowStopping();
                break;
        }

        if (state != Connected)
            ThrowNotConnected();


        _mainStream!.SwitchEncryption(privateKey.ToArray());
    }

    #endregion

    #region Dispose

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ThrowDisposed() => throw new ObjectDisposedException(string.Empty);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ThrowNotConnected() => throw new InvalidOperationException("Not connected");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ThrowStopping() => throw new InvalidOperationException("Stopping");


    /// <summary>
    ///     Disposes the client.
    /// </summary>
    public void Dispose()
    {
        int state = Interlocked.Exchange(ref _state, Disposed);
        if (state == Disposed)
            return;
        try
        {
            _aliveClient.Cancel();
        }
        catch
        {
            // ignored
        }
        finally
        {
            _mainStream?.Dispose();
            _aliveClient.Dispose();
            _sendLock.Dispose();
            _packetReader.BaseStream = null;
            _packetSender.BaseStream = null;
            _completion.TrySetResult();
        }
    }

    #endregion
}