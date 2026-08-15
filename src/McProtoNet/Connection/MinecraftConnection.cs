using System.IO.Pipelines;
using McProtoNet.Net;

namespace McProtoNet;

/// <summary>
///     A Minecraft protocol connection over an arbitrary duplex <see cref="Stream" />:
///     frames packets in both directions, with optional compression and encryption
///     provided by the underlying packet pipes.
/// </summary>
/// <remarks>
///     Disposal contract: finish (or cancel) packet enumeration first, then
///     <see cref="DisposeAsync" />. The synchronous <see cref="Dispose" /> only requests
///     shutdown; buffers are returned by <see cref="DisposeAsync" /> after the pumps stop.
/// </remarks>
public sealed class MinecraftConnection : IDisposable, IAsyncDisposable
{
    private readonly MinecraftPacketPipeReader _packetReader;
    private readonly MinecraftPacketPipeWriter _packetWriter;
    private readonly CancellationTokenSource _cts;
    private readonly SemaphoreSlim _sendGate = new(1, 1);

    private const int None = 0;
    private const int Disposed = 1;
    private int _state;
    private int _asyncDisposed;

    /// <summary>
    ///     Wraps an already-connected stream. The connection owns the stream and
    ///     disposes it when the pumps stop.
    /// </summary>
    public static MinecraftConnection Create(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var networkToApp = new Pipe();
        var appToNetwork = new Pipe();

        var transport = new DuplexPipe(input: appToNetwork.Reader, output: networkToApp.Writer);
        var app = new DuplexPipe(input: networkToApp.Reader, output: appToNetwork.Writer);

        return new MinecraftConnection(stream, transport, app);
    }

    internal MinecraftConnection(Stream stream, IDuplexPipe transport, IDuplexPipe app)
    {
        _cts = new CancellationTokenSource();
        _packetReader = new MinecraftPacketPipeReader(app.Input);
        _packetWriter = new MinecraftPacketPipeWriter(app.Output);

        Completion = RunAsync(stream, transport, _cts.Token);
    }

    /// <summary>Low-level packet reader; exposes encryption and compression switches.</summary>
    public MinecraftPacketPipeReader PacketReader => _packetReader;

    /// <summary>Low-level packet writer; exposes encryption and compression switches.</summary>
    public MinecraftPacketPipeWriter PacketWriter => _packetWriter;

    /// <summary>Completes when both pumps have stopped and the stream is disposed. Never faults.</summary>
    public Task Completion { get; }

    public int CompressionThreshold
    {
        get => _packetReader.CompressionThreshold;
        set
        {
            _packetReader.CompressionThreshold = value;
            _packetWriter.CompressionThreshold = value;
        }
    }

    public IAsyncEnumerable<InputPacket> ReadPacketsAsync(CancellationToken token = default)
    {
        ThrowIfDisposed();
        return _packetReader.ReadPacketsAsync(token);
    }

    public ValueTask<InputPacket> ReadPacketAsync(CancellationToken token = default)
    {
        ThrowIfDisposed();
        return _packetReader.ReadPacketAsync(token);
    }

    /// <summary>Sends one packet. Serialized internally — safe from any thread.</summary>
    public async ValueTask SendPacketAsync(ReadOnlyMemory<byte> packet, CancellationToken token = default)
    {
        ThrowIfDisposed();
        await _sendGate.WaitAsync(token).ConfigureAwait(false);
        try
        {
            ThrowIfDisposed();
            _packetWriter.WritePacket(packet.Span);
            var result = await _packetWriter.FlushAsync(token).ConfigureAwait(false);
            if (result.IsCanceled) token.ThrowIfCancellationRequested();
            if (result.IsCompleted)
            {
                throw new InvalidOperationException("Connection is closed");
            }
        }
        finally
        {
            _sendGate.Release();
        }
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _state) == Disposed, this);
        if (_cts.IsCancellationRequested)
        {
            throw new InvalidOperationException("Connection is closed");
        }
    }

    /// <summary>Requests shutdown. Buffers are released by <see cref="DisposeAsync" />.</summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _state, Disposed) == Disposed)
        {
            return;
        }

        _cts.Cancel();
    }

    public async ValueTask DisposeAsync()
    {
        Dispose();
        if (Interlocked.Exchange(ref _asyncDisposed, 1) == 1)
        {
            await Completion.ConfigureAwait(false);
            return;
        }

        await Completion.ConfigureAwait(false);
        // fence out in-flight senders: after this acquire no send holds the writer
        await _sendGate.WaitAsync().ConfigureAwait(false);
        _packetReader.Dispose();
        _packetWriter.Dispose();
        _cts.Dispose();
        _sendGate.Dispose();
    }

    private static async Task RunAsync(Stream stream, IDuplexPipe transport, CancellationToken token)
    {
        // Linked source: either pump stopping tears down the other one.
        using var pumps = CancellationTokenSource.CreateLinkedTokenSource(token);

        var inbound = PumpInboundAsync(stream, transport.Output, pumps.Token);
        var outbound = PumpOutboundAsync(stream, transport.Input, pumps.Token);

        await Task.WhenAny(inbound, outbound).ConfigureAwait(false);
        pumps.Cancel();

        Exception? failure = null;
        try
        {
            await inbound.ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (pumps.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            failure = ex;
        }

        try
        {
            await outbound.ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (pumps.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            failure ??= ex;
        }

        // any transport death — inbound or outbound — reaches the packet enumeration;
        // clean EOF and own cancellation end it without an error
        await transport.Output.CompleteAsync(failure).ConfigureAwait(false);
        await transport.Input.CompleteAsync().ConfigureAwait(false);
        stream.Dispose();
    }

    private static async Task PumpInboundAsync(Stream stream, PipeWriter pipeWriter, CancellationToken token)
    {
        while (true)
        {
            var memory = pipeWriter.GetMemory();
            int bytes = await stream.ReadAsync(memory, token).ConfigureAwait(false);
            if (bytes == 0)
            {
                return; // clean end of stream
            }

            pipeWriter.Advance(bytes);

            var result = await pipeWriter.FlushAsync(token).ConfigureAwait(false);
            if (result.IsCanceled) token.ThrowIfCancellationRequested();
            if (result.IsCompleted)
            {
                return;
            }
        }
    }

    private static async Task PumpOutboundAsync(Stream stream, PipeReader pipeReader, CancellationToken token)
    {
        while (true)
        {
            var result = await pipeReader.ReadAsync(token).ConfigureAwait(false);

            if (result.IsCanceled)
            {
                return;
            }

            foreach (var segment in result.Buffer)
                await stream.WriteAsync(segment, token).ConfigureAwait(false);

            await stream.FlushAsync(token).ConfigureAwait(false);

            pipeReader.AdvanceTo(result.Buffer.End);

            if (result.IsCompleted)
            {
                return;
            }
        }
    }
}
