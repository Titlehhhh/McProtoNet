using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using McProtoNet.Transport.Pipelines;
namespace McProtoNet.Benchmark.Pipelines.SendBenchs;

public class Pipelines2SendBench : ISendBench
{
    private MinecraftPacketPipeWriter _writer;

    private Stream _stream;

    private readonly Pipe _pipe = new(new PipeOptions(
        useSynchronizationContext: true));


    private Task _task;
    

    public async Task Setup(Stream stream, int compressionThreshold)
    {
        _stream = stream;
        _writer = new MinecraftPacketPipeWriter(_pipe.Writer)
        {
            CompressionThreshold = compressionThreshold
        };
        _task = RunWriter();
    }

    private async Task RunWriter()
    {
        var reader = _pipe.Reader;

        try
        {
            while (true)
            {
                // Используем токен, чтобы ReadAsync можно было прервать снаружи
                ReadResult result = await reader.ReadAsync(CancellationToken.None).ConfigureAwait(false);
                ReadOnlySequence<byte> buffer = result.Buffer;

                try
                {
                    if (!buffer.IsEmpty)
                    {
                        foreach (var segment in buffer)
                            await _stream.WriteAsync(segment).ConfigureAwait(false);
                    }

                    if (result.IsCanceled || result.IsCompleted)
                        break;
                }
                finally
                {
                    // Если пусто — AdvanceTo(start), иначе AdvanceTo(end)
                    reader.AdvanceTo(buffer.IsEmpty ? buffer.Start : buffer.End);
                }
            }
        }
        catch (OperationCanceledException)
        {
            /* ожидаемо при Cancel */
        }
        catch (Exception ex)
        {
            Console.WriteLine("Reader loop exception: " + ex);
            Environment.FailFast("Reader loop error");
        }
        finally
        {
            
        }
    }

    public async Task Run(int packetsCount, ReadOnlyMemory<byte> packet)
    {
        for (int i = 0; i < packetsCount; i++)
        {
            _writer.WritePacket(packet.Span);
            await _writer.FlushAsync();
        }
        await _pipe.Writer.CompleteAsync();
    }

    public async Task Cleanup()
    {
        await _task;
        _pipe.Reader.CancelPendingRead();
        await _pipe.Reader.CompleteAsync().ConfigureAwait(false);
        _pipe.Reset();
        _stream?.Dispose();
    }
}