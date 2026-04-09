using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using McProtoNet.Net;
using McProtoNet.Serialization;

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

    private static byte[] ConsumerBuffer = new byte[1024 * 1024];

    private TcpListener listener;
    private CancellationTokenSource cts;

    public async Task Run(int packetsCount, int compressionThreshold, ServerMode mode)
    {
        cts = new CancellationTokenSource();
        listener = new TcpListener(IPAddress.Any, 6060);
        listener.Start();

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var stream = new MemoryStream();
        await using var writer = new MinecraftPacketSender(stream);
        writer.CompressionThreshold = compressionThreshold;

        for (int i = 0; i < packetsCount; i++)
        {
            var buffer = GeneratePacket();
            var packet = new OutputPacket(buffer);
            await writer.SendAndDisposeAsync(packet, CancellationToken.None);
        }

        bytes = stream.ToArray();

        _ = Task.Run(async () =>
        {
            while (!cts.IsCancellationRequested)
            {
                var socket = await listener.AcceptSocketAsync(CancellationToken.None);
                await Task.Run(async () =>
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
                        int count = 0;
                        try
                        {
                            await using var ns = new NetworkStream(socket, true);
                            while (true)
                            {
                                await ns.ReadExactlyAsync(ConsumerBuffer, CancellationToken.None);
                            }
                            // await using var buffer = new PoolingBufferedStream(ns);
                            // var packetReader = new MinecraftPacketReader
                            // {
                            //     BaseStream = buffer
                            // };
                            // packetReader.SwitchCompression(compressionThreshold);
                            //
                            // while (true)
                            // {
                            //     using var p = await packetReader.ReadNextPacketAsync();
                            //     count++;
                            // }
                        }
                        catch (Exception ex)
                        {
                            //Console.WriteLine($"TestServer: {ex}");
                            // ignored
                        }

                        return;
                        if (count != packetsCount)
                            Environment.FailFast($"TestServer: Packets count mismatch {count} != {packetsCount}");
                    }
                });
            }
        });
    }

    public void Stop()
    {
        cts.Cancel();
        listener.Stop();
        listener.Dispose();
        cts.Dispose();
    }
}