using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using McProtoNet.Net;
using McProtoNet.Serialization;

namespace McProtoNet.Client;

/// <summary>
/// Experimental class
/// </summary>
public class PipelinesMinecraftClient : IDisposable, IAsyncDisposable
{
    public int ProtocolVersion { get; }


    public static PipelinesMinecraftClient Create(Stream stream, int protocolVersion)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(protocolVersion);
        ArgumentNullException.ThrowIfNull(stream);

        var networkToAppPipe = new Pipe();
        var appToNetworkPipe = new Pipe();


        var transport = new DuplexPipe(
            input: appToNetworkPipe.Reader,
            output: networkToAppPipe.Writer
        );
        var app = new DuplexPipe(
            input: networkToAppPipe.Reader,
            output: appToNetworkPipe.Writer
        );

        return new PipelinesMinecraftClient(stream, transport, app, protocolVersion);
    }

    internal PipelinesMinecraftClient(Stream stream, IDuplexPipe transport, IDuplexPipe app, int protocolVersion)
    {
        ProtocolVersion = protocolVersion;

        _stream = stream;
        _cts = new CancellationTokenSource();


        _pipeReader = new MinecraftPacketPipeReader(app.Input);
        _pipeWriter = new MinecraftPacketPipeWriter(app.Output);


        var task1 = ReadFromStream(stream, transport.Output, _cts.Token)
            .ContinueWith(async (t, state) =>
            {
                var exception = t.Exception?.Flatten().InnerExceptions.FirstOrDefault();
                var transportOutput = (IDuplexPipe)state!;
                await transportOutput.Input.CompleteAsync(exception);
            }, transport, TaskScheduler.Default);


        var task2 = WriteToStream(stream, transport.Input, _cts.Token)
            .ContinueWith(async (t, state) =>
            {
                var exception = t.Exception?.Flatten().InnerExceptions.FirstOrDefault();
                var transportPipe = (IDuplexPipe)state!;
                await transportPipe.Output.CompleteAsync(exception);
            }, transport, TaskScheduler.Default);

        var task = Task.WhenAll(task1, task2)
            .ContinueWith(_ =>
            {
                stream.Dispose();
                _pipeReader.Dispose();
                //_pipeWriter.Dispose();
            }, TaskScheduler.Default);

        Completion = task;
    }

    private readonly MinecraftPacketPipeReader _pipeReader;
    private readonly MinecraftPacketPipeWriter _pipeWriter;

    public MinecraftPacketPipeWriter PacketWriter => _pipeWriter;
    public MinecraftPacketPipeReader PacketReader => _pipeReader;
    public Task Completion { get; }

    private readonly Stream _stream;

    private CancellationTokenSource _cts;


    public int CompressionThreshold
    {
        get;
        set
        {
            _pipeReader.CompressionThreshold = value;
            _pipeWriter.CompressionThreshold = value;
            field = value;
        }
    }

    public IAsyncEnumerable<InputPacket> ReadPacketsAsync(CancellationToken token = default)
    {
        ThrowIfDisposed();
        return _pipeReader.ReadPacketsAsync(token);
    }

    public ValueTask<InputPacket> ReadPacketAsync(CancellationToken token = default)
    {
        ThrowIfDisposed();
        return _pipeReader.ReadPacketAsync(token);
    }

    public async ValueTask SendPacketAsync(ReadOnlyMemory<byte> packet, CancellationToken token = default)
    {
        ThrowIfDisposed();
        _pipeWriter.WritePacket(packet.Span);
        var result = await _pipeWriter.FlushAsync(token).ConfigureAwait(false);
        if (result.IsCanceled) token.ThrowIfCancellationRequested();
        if (result.IsCompleted)
        {
            throw new InvalidOperationException("Stream is closed");
        }
    }

    public async ValueTask SendEmptyPacketAsync(int id, CancellationToken token = default)
    {
        ThrowIfDisposed();
        Span<byte> idBytes = [0, 0, 0, 0, 0];
        var len = id.GetVarIntLength(idBytes);
        _pipeWriter.WritePacket(idBytes[..len]);


        var result = await _pipeWriter.FlushAsync(token).ConfigureAwait(false);
        if (result.IsCanceled) token.ThrowIfCancellationRequested();
        if (result.IsCompleted)
        {
            throw new InvalidOperationException("Stream is closed");
        }
    }

    private int _state;

    private const int None = 0;
    private const int Disposed = 1;

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_state == Disposed, "Stream is closed");
        if (_cts.IsCancellationRequested)
        {
            throw new InvalidOperationException("Stream is closed");
        }
    }

    public void Stop()
    {
        _cts.Cancel();
    }

    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref _state, Disposed, None) == Disposed)
        {
            return;
        }

        _cts.Cancel();
        _cts.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        Dispose();
        await Completion.ConfigureAwait(false);
    }


    private static async Task WriteToStream(Stream stream, PipeReader pipeReader, CancellationToken cancellationToken)
    {
        try
        {
            while (true)
            {
                var result = await pipeReader.ReadAsync(cancellationToken).ConfigureAwait(false);

                if (result.IsCanceled)
                {
                    break;
                }

                if (result.IsCompleted)
                {
                    break;
                }

               

                foreach (var segment in result.Buffer)
                    await stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);

                await stream.FlushAsync(cancellationToken).ConfigureAwait(false);

                pipeReader.AdvanceTo(result.Buffer.End);
            }

            await pipeReader.CompleteAsync().ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            await pipeReader.CompleteAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await pipeReader.CompleteAsync(ex).ConfigureAwait(false);
            throw;
        }
    }


    private static async Task ReadFromStream(Stream stream, PipeWriter pipeWriter, CancellationToken cancellationToken)
    {
        try
        {
            while (true)
            {
                var memory = pipeWriter.GetMemory();
                int bytes = await stream.ReadAtLeastAsync(
                    memory,
                    1,
                    throwOnEndOfStream: true,
                    cancellationToken).ConfigureAwait(false);


                pipeWriter.Advance(bytes);

                var result = await pipeWriter.FlushAsync(cancellationToken).ConfigureAwait(false);

                if (result.IsCanceled) cancellationToken.ThrowIfCancellationRequested();

                if (result.IsCompleted)
                {
                    break;
                }
            }

            await pipeWriter.CompleteAsync().ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            await pipeWriter.CompleteAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await pipeWriter.CompleteAsync(ex).ConfigureAwait(false);
            throw;
        }
    }
}