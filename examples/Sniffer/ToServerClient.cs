using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.Intrinsics;
using McProtoNet;
using McProtoNet.Abstractions;
using McProtoNet.Net;
using McProtoNet.Protocol;
using McProtoNet.Serialization;

internal class ToServerClient : IDisposable
{
    private readonly string _host;
    private readonly ushort _port;
    private TcpClient tcp;
    private MinecraftPacketReader _reader;
    private MinecraftPacketSender _sender;
    private SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

    public ToServerClient(string host, ushort port)
    {
        _host = host;
        _port = port;
    }

    public Stream MainStream => tcp.GetStream();

    public void SetCompress(int threshold)
    {
        _reader.SwitchCompression(threshold);
        _sender.SwitchCompression(threshold);
    }

    public async Task Connect()
    {
        tcp = new TcpClient();
        await tcp.ConnectAsync(_host, _port);
        _reader = new MinecraftPacketReader();
        _sender = new MinecraftPacketSender();

        _reader.BaseStream = tcp.GetStream();
        _sender.BaseStream = tcp.GetStream();
    }

    public async Task<InputPacket> ReadNextPacket()
    {
        return await _reader.ReadNextPacketAsync();
    }

    public async Task SendPacket(OutputPacket packet)
    {
        await _lock.WaitAsync();
        try
        {
            await _sender.SendAndDisposeAsync(packet, default);
        }
        finally
        {
            _lock.Release();
        }
    }

    private CancellationTokenSource _exploitCancel;

    private Task Dig(int status, Position pos, sbyte face)
    {
        var packet = CreateDigPacket(status, pos, face);
        return SendPacket(packet);
    }

    public Task Swing()
    {
        var packet = CreateSwingPacket();
        return SendPacket(packet);
    }

    public void StartExploit(string arg)
    {
        Console.WriteLine("Exploit Started");
        try
        {
            if (_exploitCancel is null)
            {
                _exploitCancel = new CancellationTokenSource();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("GG: " + e);
        }

        Task.Run(async () =>
        {
            try
            {
                int[] splits = arg.Split(' ').Select(x => int.Parse(x)).ToArray();


                Position start = new Position(Math.Min(splits[0], splits[3]),
                    Math.Min(splits[1], splits[4]),
                    Math.Min(splits[2], splits[5]));

                Position stop = new Position(Math.Max(splits[0], splits[3]),
                    Math.Max(splits[1], splits[4]),
                    Math.Max(splits[2], splits[5]));


                Stopwatch stopwatch = new Stopwatch();
                TimeSpan lastTime = TimeSpan.FromMilliseconds(10);
                Console.WriteLine($"Start exploit from: {start} to: {stop}");
                int totalBlocks = 0;
                int processedBlocks = 0;

                Stopwatch totalTime = Stopwatch.StartNew();

                for (int mode = 0; mode <= 1; mode++)
                {
                    for (int y = start.Y; y <= stop.Y; y++)
                    {
                        for (int x = start.X; x <= stop.X; x++)
                        {
                            for (int z = start.Z; z <= stop.Z; z++)
                            {
                                _exploitCancel.Token.ThrowIfCancellationRequested();

                                if (mode == 0)
                                {
                                    if (FilterPos(x, y, z))
                                        totalBlocks++;
                                }
                                else
                                {
                                    if (FilterPos(x, y, z))
                                    {
                                        stopwatch.Start();
                                        processedBlocks++;
                                        Position pos = new Position(x, y, z);
                                        await Dig(0, pos, 0);
                                        await Dig(1, pos, 0);

                                        await Task.Delay(30);
                                        stopwatch.Stop();
                                        lastTime = stopwatch.Elapsed;
                                        if (processedBlocks % 100 == 0)
                                        {
                                            double progress = processedBlocks / (double)totalBlocks * 100;

                                            int remaining = totalBlocks - processedBlocks;

                                            TimeSpan remainingTime = remaining * lastTime;

                                            string sTime = string.Format("{0:D2} min {1:D2} sec", remainingTime.Minutes,
                                                remainingTime.Seconds);
                                            Console.WriteLine(
                                                $"Progress: {progress:F2}% Remaining time: {sTime} Height: {y} Осталось блоков: {remaining}");
                                        }

                                        stopwatch.Restart();
                                    }
                                }
                            }
                        }
                    }
                }

                totalTime.Stop();

                Console.WriteLine("All Good. Time: " + totalTime.Elapsed);
            }
            catch (Exception e)
            {
                Console.WriteLine("ExploitErr: " + e);
            }
            finally
            {
                _exploitCancel.Dispose();
                _exploitCancel = null;
            }
        });
    }

    private static bool FilterPos(int x, int y, int z)
    {
        int chunkX = x & 15;
        int chunkZ = z & 15;

        const int min = 0;
        const int max = 15;

        int dX = Math.Min(Distance(chunkX, min), Distance(max, chunkX));
        int dZ = Math.Min(Distance(chunkZ, min), Distance(max, chunkZ));

        return dX <= 2 || dZ <= 2;
    }

    private static int Distance(int a, int b)
    {
        return Math.Abs(a - b);
    }

    private static OutputPacket CreateDigPacket(int status, Position position, sbyte face)
    {
        scoped var writer = new MinecraftPrimitiveWriter();
        try
        {
            writer.WriteVarInt(0x1B);
            writer.WriteVarInt(status);
            writer.WritePosition(position);
            writer.WriteSignedByte(face);
            return new OutputPacket(writer.GetWrittenMemory());
        }
        finally
        {
            writer.Dispose();
        }
    }

    private static OutputPacket CreateSwingPacket()
    {
        scoped var writer = new MinecraftPrimitiveWriter();
        try
        {
            writer.WriteVarInt(0x2C);
            writer.WriteVarInt(0);
            return new OutputPacket(writer.GetWrittenMemory());
        }
        finally
        {
            writer.Dispose();
        }
    }

    public void StopExploit()
    {
        Console.WriteLine("Exploit Stop");
        _exploitCancel.Cancel();
        _exploitCancel.Dispose();
        _exploitCancel = null;
    }

    public void Dispose()
    {
        tcp.Dispose();
        _lock.Dispose();
    }
}