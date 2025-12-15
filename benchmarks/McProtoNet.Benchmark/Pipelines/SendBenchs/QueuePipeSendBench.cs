using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using DotNext.IO;
using McProtoNet.Net;

namespace McProtoNet.Benchmark.Pipelines.SendBenchs;

public class QueuePipeSendBench : ISendBench
{
    private MinecraftPacketPipeWriter _writer;

    private Stream _stream;

    private Pipe _pipe = new();

    private readonly int _queueSize;
    private Task _read;

    private Channel<ReadOnlyMemory<byte>> _channel;

    public QueuePipeSendBench(int queueSize)
    {
        _queueSize = queueSize;
    }


    public Task Setup(Stream stream, int compressionThreshold)
    {
        _stream = stream;
        if (_queueSize == -1)
            _channel = Channel.CreateUnbounded<ReadOnlyMemory<byte>>(new UnboundedChannelOptions()
            {
                SingleReader = true
            });
        else
        {
            _channel = Channel.CreateBounded<ReadOnlyMemory<byte>>(new BoundedChannelOptions(_queueSize)
            {
                SingleReader = true
            });
        }

        _writer = new MinecraftPacketPipeWriter(_pipe.Writer)
        {
            CompressionThreshold = compressionThreshold
        };
        var task = Task.Run(async () =>
        {
            var reader = _pipe.Reader;

            try
            {
                while (true)
                {
                    ReadResult result = await reader.ReadAsync(CancellationToken.None);
                    ReadOnlySequence<byte> buffer = result.Buffer;

                    try
                    {
                        if (!buffer.IsEmpty)
                        {
                            // Пишем в сеть по сегментам — безопасно и без лишних аллокаций
                            await _stream.WriteAsync(buffer);
                        }

                        if (result.IsCanceled || result.IsCompleted)
                            break;
                    }
                    finally
                    {
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
        });
        var task2 = Task.Run(async () =>
        {
            try
            {
                var reader = _channel.Reader;
                while (await reader.WaitToReadAsync())
                {
                    int count = 0;
                    bool isBreak = false;
                    while (reader.TryRead(out var memory))
                    {
                        count++;
                        _writer.WritePacket(memory.Span);
                        if (count == 500_000)
                        {
                            count = 0;
                            var result1 = await _writer.FlushAsync();
                            if (result1.IsCanceled || result1.IsCompleted)
                            {
                                isBreak = true;
                                break;
                            }
                        }
                    }

                    if (isBreak)
                        break;
                    var result = await _writer.FlushAsync();
                    if (result.IsCanceled || result.IsCompleted)
                        break;
                }
            }
            finally
            {
                await _pipe.Writer.CompleteAsync();
            }
        });
        _read = Task.WhenAll(task, task2);
        return Task.CompletedTask;
    }

    public async Task Run(int packetsCount, ReadOnlyMemory<byte> packet)
    {
        var writer = _channel.Writer;
        for (int i = 0; i < packetsCount; i++)
        {
            await writer.WriteAsync(packet);
        }

        writer.Complete();
        await _read;
    }

    public async Task Cleanup()
    {
        await _read;
        await _pipe.Reader.CompleteAsync();
        _channel.Writer.TryComplete();
        _pipe.Reset();
        _stream?.Dispose();
    }
}