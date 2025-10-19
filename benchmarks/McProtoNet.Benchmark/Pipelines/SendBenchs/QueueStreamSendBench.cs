using System;
using System.IO;
using System.Threading.Channels;
using System.Threading.Tasks;
using DotNext.IO;
using McProtoNet.Net;

namespace McProtoNet.Benchmark.Pipelines.SendBenchs;

public class QueueStreamSendBench : ISendBench
{
    private MinecraftPacketSender _sender;
    private Stream _stream;
    private readonly int _queueSize;

    private Channel<ReadOnlyMemory<byte>> _channel;
    private Task _read;
   
    public QueueStreamSendBench(int queueSize)
    {
        _queueSize = queueSize;
    }   

    public Task Setup(Stream stream, int compressionThreshold)
    {
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
        _stream = new PoolingBufferedStream(stream);

        _sender = new MinecraftPacketSender(_stream)
        {
            AutoFlush = false,
            CompressionThreshold = compressionThreshold
        };
        Task read = Task.Run(async () =>
        {
            try
            {
                var reader = _channel.Reader;
                while (await reader.WaitToReadAsync())
                {
                    int count = 0;
                    while (reader.TryRead(out var memory))
                    {
                        count++;
                        await _sender.SendPacketAsync(memory);
                        if (count == 500_000)
                        {
                            count = 0;
                            await _sender.FlushAsync();
                        }
                    }

                    await _sender.FlushAsync();
                }
            }
            finally
            {
                _channel.Writer.TryComplete();
            }
        });
        _read = read;
        return Task.CompletedTask;
    }

    public async Task Run(int packetsCount, ReadOnlyMemory<byte> packet)
    {
        
        var writer = _channel.Writer;
        for (int i = 0; i < packetsCount; i++)
        {
            await writer.WriteAsync(packet);
        }
        _channel.Writer.Complete();
        await _read;
    }

    public async Task Cleanup()
    {
        await _sender.DisposeAsync();
    }
}