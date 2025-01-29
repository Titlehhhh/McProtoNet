using System.Diagnostics;
using System.Net.Sockets;
using System.Reactive;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Channels;
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

    public IObservable<InputPacket> ReceivePackets => _receivePackets;
    public bool IsConnected => Volatile.Read(ref _state) == Connected;
    public MinecraftClientStartOptions StartOptions { get; }
    public int ProtocolVersion => StartOptions.Version;

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

    private readonly Subject<InputPacket> _receivePackets = new();

    private readonly TaskCompletionSource _completion = new();
    private readonly CancellationTokenSource _aliveClient = new();
    private readonly MinecraftPacketReader _packetReader = new();
    private readonly MinecraftPacketSender _packetSender = new();

    private readonly Channel<OutputPacket> _packetQueue = Channel.CreateUnbounded<OutputPacket>();
    private AesStream? _mainStream;

    #endregion

    #region Methods

    /// <summary>
    /// Send a packet
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public ValueTask SendPacket(OutputPacket packet)
    {
        int state = _state;
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

        Debug.WriteLine($"Enqueue packet: [{string.Join(", ", packet.Memory.ToArray())}]");
        return _packetQueue.Writer.WriteAsync(packet, _aliveClient.Token);
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

            Task handlePackets = HandlePackets();
            Task mainLoop = MainLoop();

            Task allTasks = Task.WhenAll(handlePackets, mainLoop);

            _ = allTasks.ContinueWith(s =>
            {
                Dispose();
                _completion.TrySetResult();
            });

            Interlocked.Exchange(ref _state, Connected);
        }
        catch
        {
            int state = Interlocked.Exchange(ref _state, Stopping);
            if (state == Stopping)
                return;

            Dispose();

            throw;
        }
    }

    private async ValueTask<Stream> ConnectInternal(MinecraftClientStartOptions startOptions,
        CancellationToken cancellationToken)
    {
        using CancellationTokenSource timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(startOptions.ConnectTimeout);
        if (startOptions.Proxy is not null)
        {
            Stream stream;


            stream = await startOptions.Proxy.ConnectAsync(startOptions.Host, startOptions.Port, timeout.Token);


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

    private async Task MainLoop()
    {
        Debug.WriteLine("Starting main loop");
        try
        {
            while (!_aliveClient.IsCancellationRequested)
            {
                InputPacket packet = await _packetReader.ReadNextPacketAsync(_aliveClient.Token);
                Debug.WriteLine("Received packet: " + packet.Id);
                try
                {
                    _receivePackets.OnNext(packet);
                }
                finally
                {
                    packet.Dispose();
                }
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine("Error in main loop: " + e);
            int state = Interlocked.Exchange(ref _state, Stopping);
            if (state == Stopping)
                return;
            _receivePackets.OnError(e);
            Dispose();
        }

        Debug.WriteLine("Main loop stopped");
    }

    private async Task HandlePackets()
    {
        Debug.WriteLine("Starting packet handler");
        try
        {
            await foreach (var packet in _packetQueue.Reader.ReadAllAsync(_aliveClient.Token))
            {
                Debug.WriteLine($"Send packet: [{string.Join(", ", packet.Memory.ToArray())}]");
                await _packetSender.SendAndDisposeAsync(packet, _aliveClient.Token);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error in packet handler: " + ex);
            int state = Interlocked.Exchange(ref _state, Stopping);
            if (state == Stopping)
                return;
            _receivePackets.OnError(ex);
            Dispose();
        }
        finally
        {
            while (_packetQueue.Reader.TryRead(out var packet))
            {
                packet.Dispose();
            }
        }

        Debug.WriteLine("Packet handler stopped");
    }


    public void SwitchCompression(int threshold)
    {
        int state = _state;
        if (state == Disposed)
        {
            ThrowDisposed();
        }

        if (state == Stopping)
        {
            throw new InvalidOperationException();
        }

        if (state != Connected)
            throw new InvalidOperationException();


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
        if (state == Disposed)
        {
            ThrowDisposed();
        }

        if (state == Stopping)
        {
            throw new InvalidOperationException();
        }

        if (state != Connected)
            throw new InvalidOperationException();


        _mainStream!.SwitchEncryption(privateKey.ToArray());
    }

    #endregion

    #region Dispose

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ThrowDisposed() => throw new ObjectDisposedException(string.Empty);

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
            _packetQueue.Writer.TryComplete();

            _packetReader.BaseStream = null;
            _packetSender.BaseStream = null;
            _receivePackets.OnCompleted();
        }
    }

    #endregion
}