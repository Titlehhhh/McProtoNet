using System;
using System.Collections.Generic;
using System.Linq;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
using McProtoNet.Transport.Framing;
using McProtoNet.Transport.Pipelines;
namespace McProtoNet.Benchmark.Pipelines;

public enum SendStrategy
{
    DirectStream,
    PacketPipeWriter,
    ChannelBatch,
    ChannelAligned,
    ChannelDrain
}

public enum SendTransport
{
    File,
    Tcp
}

[Config(typeof(ShortInProcessConfig))]
[MemoryDiagnoser]
public class SendStrategyBenchmarks
{
    public class ShortInProcessConfig : ManualConfig
    {
        public ShortInProcessConfig()
        {
            AddJob(Job.ShortRun
                .WithToolchain(InProcessNoEmitToolchain.Instance)
                .WithInvocationCount(1)
                .WithUnrollFactor(1));
        }
    }

    private const int Port = 6061;

    private static readonly bool VerifyDelivery =
        Environment.GetEnvironmentVariable("MCPROTO_BENCH_NOVERIFY") is null;

    [Params(1_000_000)] public int PacketsCount;
    [Params(50)] public int PacketSize;
    [Params(100)] public int FlushEvery;
    [ParamsSource(nameof(CompressionThresholds))] public int CompressionThreshold;

    public static IEnumerable<int> CompressionThresholds =>
        Environment.GetEnvironmentVariable("MCPROTO_BENCH_COMPRESSION") switch
        {
            null => [-1, 32],
            var s => s.Split(',').Select(int.Parse)
        };
    [Params(1, 4)] public int Producers;

    [Params(SendTransport.File, SendTransport.Tcp)]
    public SendTransport Transport;

    [Params(SendStrategy.PacketPipeWriter, SendStrategy.ChannelBatch, SendStrategy.ChannelAligned,
        SendStrategy.ChannelDrain)]
    public SendStrategy Strategy;

    private byte[] _packet;
    private byte[] _framedPacket;
    private string _filePath;
    private TcpListener _listener;
    private CancellationTokenSource _serverCts;
    private Stream _stream;
    private TcpClient _tcpClient;
    private long _tcpReceived;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _packet = new byte[PacketSize];
        new Random(40).NextBytes(_packet);
        var frame = new ArrayBufferWriter<byte>();
        frame.WritePacket(_packet, CompressionThreshold);
        _framedPacket = frame.WrittenSpan.ToArray();
        _filePath = Path.Combine(Path.GetTempPath(), $"mcproto-send-strategy-{Guid.NewGuid():N}.bin");

        _serverCts = new CancellationTokenSource();
        _listener = new TcpListener(IPAddress.Loopback, Port);
        _listener.Start();
        _ = RunDiscardServer(_listener, _serverCts.Token);
    }

    private async Task RunDiscardServer(TcpListener listener, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Socket socket;
            try
            {
                socket = await listener.AcceptSocketAsync(token);
            }
            catch (Exception)
            {
                return;
            }

            _ = Task.Run(async () =>
            {
                var buffer = new byte[1024 * 1024];
                try
                {
                    await using var ns = new NetworkStream(socket, true);
                    while (true)
                    {
                        int read = await ns.ReadAsync(buffer);
                        if (read == 0) return;
                        Interlocked.Add(ref _tcpReceived, read);
                    }
                }
                catch (Exception)
                {
                    /* ignored */
                }
            });
        }
    }

    [IterationSetup]
    public void IterationSetup()
    {
        Interlocked.Exchange(ref _tcpReceived, 0);
        _stream = Transport switch
        {
            SendTransport.File => new FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.None,
                bufferSize: 1),
            SendTransport.Tcp => ConnectTcp(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private Stream ConnectTcp()
    {
        _tcpClient = new TcpClient { NoDelay = true };
        _tcpClient.Connect(IPAddress.Loopback, Port);
        return _tcpClient.GetStream();
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
        long expected = (long)PacketsCount * _framedPacket.Length;

        if (VerifyDelivery && Transport == SendTransport.Tcp)
        {
            var waited = Stopwatch.StartNew();
            while (Interlocked.Read(ref _tcpReceived) < expected && waited.ElapsedMilliseconds < 15000)
                Thread.Sleep(1);
        }

        try
        {
            _stream?.Dispose();
        }
        catch
        {
            /* ignored */
        }

        _stream = null;
        _tcpClient?.Dispose();
        _tcpClient = null;

        if (!VerifyDelivery) return;

        long actual = Transport == SendTransport.File
            ? new FileInfo(_filePath).Length
            : Interlocked.Read(ref _tcpReceived);
        if (actual != expected)
            throw new InvalidOperationException($"{Transport} byte count mismatch: {actual} != {expected}");
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        _serverCts.Cancel();
        _listener.Stop();
        _listener.Dispose();
        _serverCts.Dispose();
        if (File.Exists(_filePath)) File.Delete(_filePath);
    }

    [Benchmark]
    public Task SendPackets()
    {
        return Strategy switch
        {
            SendStrategy.DirectStream => RunDirectStream(),
            SendStrategy.PacketPipeWriter => RunPacketPipeWriter(),
            SendStrategy.ChannelBatch => RunChannel(minimumBufferSize: 4096, flushEveryPackets: FlushEvery,
                flushBytes: 0),
            SendStrategy.ChannelAligned => RunChannel(minimumBufferSize: 8192, flushEveryPackets: FlushEvery,
                flushBytes: 0),
            SendStrategy.ChannelDrain => RunChannel(minimumBufferSize: 16384, flushEveryPackets: 0,
                flushBytes: 8192),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private async Task RunDirectStream()
    {
        using var gate = new SemaphoreSlim(1, 1);
        await RunProducers(async count =>
        {
            for (int i = 0; i < count; i++)
            {
                await gate.WaitAsync();
                try
                {
                    await _stream.WriteAsync(_framedPacket);
                }
                finally
                {
                    gate.Release();
                }
            }
        });
    }

    private async Task RunPacketPipeWriter()
    {
        var pipe = new Pipe();
        var writer = new MinecraftPacketPipeWriter(pipe.Writer)
        {
            CompressionThreshold = CompressionThreshold
        };
        var pump = Task.Run(() => PumpToStream(pipe.Reader, _stream));
        using var gate = new SemaphoreSlim(1, 1);

        Exception producerError = null;
        try
        {
            await RunProducers(async count =>
            {
                for (int i = 0; i < count; i++)
                {
                    await gate.WaitAsync();
                    try
                    {
                        writer.WritePacket(_packet);
                        await writer.FlushAsync();
                    }
                    finally
                    {
                        gate.Release();
                    }
                }
            });
        }
        catch (Exception ex)
        {
            producerError = ex;
        }

        try
        {
            await writer.CompleteAsync(producerError);
            await pump;
        }
        finally
        {
            writer.Dispose();
        }

        if (producerError is not null) throw producerError;
    }

    private static async Task PumpToStream(PipeReader reader, Stream stream)
    {
        try
        {
            while (true)
            {
                ReadResult result = await reader.ReadAsync();
                ReadOnlySequence<byte> buffer = result.Buffer;
                foreach (var segment in buffer)
                    await stream.WriteAsync(segment);
                reader.AdvanceTo(buffer.End);
                if (result.IsCanceled || result.IsCompleted)
                    break;
            }

            await reader.CompleteAsync();
        }
        catch (Exception ex)
        {
            await reader.CompleteAsync(ex);
            throw;
        }
    }

    private async Task RunChannel(int minimumBufferSize, int flushEveryPackets, int flushBytes)
    {
        var channel = Channel.CreateUnbounded<ReadOnlyMemory<byte>>(new UnboundedChannelOptions
        {
            SingleReader = true
        });

        var consumer = Task.Run(async () =>
        {
            var writer = PipeWriter.Create(_stream,
                new StreamPipeWriterOptions(minimumBufferSize: minimumBufferSize, leaveOpen: true));
            try
            {
                var reader = channel.Reader;
                while (await reader.WaitToReadAsync())
                {
                    int sincePackets = 0;
                    int sinceBytes = 0;
                    while (reader.TryRead(out var packet))
                    {
                        writer.WritePacket(packet.Span, CompressionThreshold);
                        sincePackets++;
                        sinceBytes += _framedPacket.Length;
                        if ((flushEveryPackets > 0 && sincePackets >= flushEveryPackets) ||
                            (flushBytes > 0 && sinceBytes >= flushBytes))
                        {
                            sincePackets = 0;
                            sinceBytes = 0;
                            await writer.FlushAsync();
                        }
                    }

                    await writer.FlushAsync();
                }

                await writer.CompleteAsync();
            }
            catch (Exception ex)
            {
                channel.Writer.TryComplete(ex);
                throw;
            }
        });

        var channelWriter = channel.Writer;
        try
        {
            await RunProducers(async count =>
            {
                for (int i = 0; i < count; i++)
                {
                    await channelWriter.WriteAsync(_packet);
                }
            });
        }
        finally
        {
            channelWriter.TryComplete();
            await consumer;
        }
    }

    private Task RunProducers(Func<int, Task> producer)
    {
        var tasks = new Task[Producers];
        int perProducer = PacketsCount / Producers;
        for (int p = 0; p < Producers; p++)
        {
            int count = p == 0 ? perProducer + PacketsCount % Producers : perProducer;
            tasks[p] = Task.Run(() => producer(count));
        }

        return Task.WhenAll(tasks);
    }
}
