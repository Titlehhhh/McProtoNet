using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using DotNext.IO.Pipelines;
using McProtoNet.Net;

namespace McProtoNet.Client;

/// <summary>
/// Experimental class
/// </summary>
public class PipelinesMinecraftClient : IDisposable
{
    public int ProtocolVersion { get; }
    public static PipelinesMinecraftClient Create(Stream stream, int protocolVersion)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(protocolVersion);
        ArgumentNullException.ThrowIfNull(stream);
        return new PipelinesMinecraftClient(stream, protocolVersion);
    }

    private readonly MinecraftPacketPipeReader _pipeReader;
    private readonly MinecraftPacketPipeWriter _pipeWriter;

    public PipeWriter PacketWriter => _pipeWriter;
    public PipeReader PacketReader => _pipeReader;
    
    private readonly Stream _stream;

    private Task _task;

    internal PipelinesMinecraftClient(Stream stream, int protocolVersion)
    {
        ProtocolVersion = protocolVersion;
        var transportPipe = new Pipe();
        var appPipe = new Pipe();
        _stream = stream;


        _pipeReader = new MinecraftPacketPipeReader(transportPipe.Reader);
        var task1 = ReadFromStream(stream, transportPipe.Writer, CancellationToken.None);


        _pipeWriter = new MinecraftPacketPipeWriter(appPipe.Writer);
        var task2 = WriteToStream(stream, appPipe.Reader, CancellationToken.None);

        var task = Task.WhenAll(task1, task2).ContinueWith(async t =>
        {
            var first = t.Exception?.InnerExceptions.FirstOrDefault();

            transportPipe.Reader.CancelPendingRead();
            transportPipe.Writer.CancelPendingFlush();
            await transportPipe.Writer.CompleteAsync(first);
            await transportPipe.Writer.CompleteAsync(first);
            
            appPipe.Writer.CancelPendingFlush();
            appPipe.Reader.CancelPendingRead();

            appPipe.Writer.CompleteAsync(first);
            appPipe.Reader.CompleteAsync(first);

        }, TaskScheduler.Default);
        _task = task;
    }


    public IAsyncEnumerable<NewInputPacket> ReadPacketsAsync(CancellationToken token = default)
    {
        return _pipeReader.ReadPacketsAsync(token);
    }

    public ValueTask<NewInputPacket> ReadPacketAsync(CancellationToken token = default)
    {
        return _pipeReader.ReadPacketAsync(token);
    }

    public async ValueTask SendPacketAsync(ReadOnlyMemory<byte> packet, CancellationToken token = default)
    {
        _pipeWriter.WritePacket(packet.Span);
        var result = await _pipeWriter.FlushAsync(token).ConfigureAwait(false);
        result.ThrowIfCancellationRequested(token);
    }

    private int _state;

    private const int None = 0;
    private const int Disposed = 1;

    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref _state, Disposed, None) == Disposed)
        {
            return;
        }

        _stream.Dispose();
        _pipeReader.Complete();
        _pipeWriter.Complete();
        
        
    }


    private static async Task WriteToStream(Stream stream, PipeReader pipeReader, CancellationToken cancellationToken)
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

            foreach (var memory in result.Buffer)
            {
                await stream.WriteAsync(memory, cancellationToken).ConfigureAwait(false);
            }

            await stream.FlushAsync(cancellationToken).ConfigureAwait(false);

            pipeReader.AdvanceTo(result.Buffer.End);
        }
    }

    private static async Task ReadFromStream(Stream stream, PipeWriter pipeWriter, CancellationToken cancellationToken)
    {
        while (true)
        {
            var memory = pipeWriter.GetMemory();

            int bytes = await stream.ReadAsync(memory, cancellationToken).ConfigureAwait(false);


            if (bytes == 0)
            {
                await pipeWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
                break;
            }

            pipeWriter.Advance(bytes);

            var result = await pipeWriter.FlushAsync(cancellationToken).ConfigureAwait(false);

            if (result.IsCanceled)
            {
                break;
            }

            if (result.IsCompleted)
            {
                break;
            }
        }
    }

    class Design
    {
        static async Task Run()
        {
            var stream = (new TcpClient()).GetStream();
            var pipeReader = new MinecraftPacketPipeReader(PipeReader.Create(stream));
            CancellationTokenSource cts = new CancellationTokenSource();

            try
            {
                await foreach (var packet in pipeReader.ReadPacketsAsync(cts.Token).ConfigureAwait(false))
                {
                    //packet handling
                }
            }
            finally
            {
                await pipeReader.CompleteAsync().ConfigureAwait(false);
                cts.Dispose();
            }
        }
    }
}