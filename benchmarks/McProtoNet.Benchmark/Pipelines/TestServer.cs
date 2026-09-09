using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using McProtoNet.Primitives;
using McProtoNet.Transport.Framing;
namespace McProtoNet.Benchmark.Pipelines;

public enum ServerMode
{
    Receive,
    Send
}

public class TestServer
{
    private static MemoryOwner<byte> GeneratePacket()
    {
        var packet = new TestPacket
        {
            EntityId = 1,
            DX = 2,
            DY = 3,
            DZ = 4,
            Yaw = 5,
            Pitch = 6,
            OnGround = true
        };
        MinecraftPrimitiveWriter writer = new();
        writer.WriteVarInt(3); //ID
        packet.Serialize(ref writer);
        return writer.GetWrittenMemory();
    }

    private byte[] bytes;

    private TcpListener listener;
    private CancellationTokenSource cts;

    private int _frameLength;
    private long _gapTicks;
    private long _budget;
    private volatile bool _open;
    private readonly ConcurrentQueue<Socket> _paced = new();
    private Thread _pacer;

    public async Task Run(int packetsCount, int compressionThreshold, ServerMode mode, TimeSpan gap = default,
        int framesPerConnection = 0)
    {
        cts = new CancellationTokenSource();
        listener = new TcpListener(IPAddress.Any, 6060);
        listener.Start();

        var stream = new MemoryStream();
        await using var writer = new PacketStreamWriter(stream);
        writer.CompressionThreshold = compressionThreshold;

        for (int i = 0; i < packetsCount; i++)
        {
            var buffer = GeneratePacket();
            var packet = new OutgoingPacket(buffer);
            await writer.WriteAndDisposeAsync(packet, CancellationToken.None);
        }

        bytes = stream.ToArray();

        var paced = mode == ServerMode.Receive && gap > TimeSpan.Zero;
        if (paced)
        {
            _frameLength = bytes.Length / packetsCount;
            if (_frameLength * packetsCount != bytes.Length)
                throw new InvalidOperationException("Paced mode needs frames of one length.");

            _gapTicks = (long)(gap.TotalSeconds * Stopwatch.Frequency);
            _budget = (long)(framesPerConnection > 0 ? framesPerConnection : packetsCount) * _frameLength;

            _pacer = new Thread(PaceLoop) { IsBackground = true, Priority = ThreadPriority.AboveNormal };
            _pacer.Start();
        }

        _ = Task.Run(async () =>
        {
            while (!cts.IsCancellationRequested)
            {
                var socket = await listener.AcceptSocketAsync(CancellationToken.None);

                if (paced)
                {
                    socket.NoDelay = true;
                    socket.LingerState = new LingerOption(true, 0);
                    socket.Blocking = false;
                    _paced.Enqueue(socket);
                    continue;
                }

                _ = Task.Run(async () =>
                {
                    if (mode == ServerMode.Receive)
                    {
                        try
                        {
                            await using var ns = new NetworkStream(socket, true);
                            await ns.WriteAsync(bytes, CancellationToken.None);
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }
                    else
                    {
                        var sink = new byte[1024 * 1024];
                        try
                        {
                            await using var ns = new NetworkStream(socket, true);
                            while (true)
                            {
                                await ns.ReadExactlyAsync(sink, CancellationToken.None);
                            }
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }
                });
            }
        });
    }

    public void Release()
    {
        _open = true;
    }

    public void Hold()
    {
        _open = false;
    }

    private void PaceLoop()
    {
        var live = new List<Feeder>();
        var token = cts.Token;
        var next = Stopwatch.GetTimestamp();

        while (!token.IsCancellationRequested)
        {
            if (!_open)
            {
                foreach (var feeder in live) Drop(feeder.Socket);
                live.Clear();

                Thread.Sleep(1);
                next = Stopwatch.GetTimestamp();
                continue;
            }

            while (_paced.TryDequeue(out var socket)) live.Add(new Feeder(socket, _budget));

            if (live.Count == 0)
            {
                Thread.SpinWait(64);
                next = Stopwatch.GetTimestamp();
                continue;
            }

            for (var i = live.Count - 1; i >= 0; i--)
            {
                var feeder = live[i];
                if (feeder.Remaining <= 0) continue;

                try
                {
                    var count = (int)Math.Min(Math.Min(_frameLength, feeder.Remaining),
                        bytes.Length - feeder.Offset);
                    var sent = feeder.Socket.Send(bytes, feeder.Offset, count, SocketFlags.None);

                    feeder.Offset += sent;
                    feeder.Remaining -= sent;
                    if (feeder.Offset >= bytes.Length) feeder.Offset = 0;
                }
                catch (SocketException e) when (e.SocketErrorCode == SocketError.WouldBlock)
                {
                    // the reader is behind; the other connections keep their turn
                }
                catch (Exception)
                {
                    live.RemoveAt(i);
                    Drop(feeder.Socket);
                }
            }

            next += _gapTicks;
            var now = Stopwatch.GetTimestamp();
            if (now >= next) next = now;
            else
                while (Stopwatch.GetTimestamp() < next)
                    Thread.SpinWait(8);
        }

        foreach (var feeder in live) Drop(feeder.Socket);
    }

    private static void Drop(Socket socket)
    {
        try
        {
            socket.Dispose();
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public void Stop()
    {
        _open = false;
        cts.Cancel();
        listener.Stop();
        listener.Dispose();
        _pacer?.Join(TimeSpan.FromSeconds(5));
        _pacer = null;
        while (_paced.TryDequeue(out var socket)) Drop(socket);
        cts.Dispose();
    }

    private sealed class Feeder(Socket socket, long budget)
    {
        public Socket Socket { get; } = socket;

        public int Offset { get; set; }

        public long Remaining { get; set; } = budget;
    }
}
