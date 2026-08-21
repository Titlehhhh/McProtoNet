using System.Buffers;
using System.IO.Pipelines;
using McProtoNet.Transport.Pipelines;
namespace McProtoNet.Tests.Pipelines;

public class MinecraftPacketPipeReaderTests
{
    private static readonly byte[] TestKey = "0123456789ABCDEF"u8.ToArray();

    private static (MinecraftPacketPipeWriter writer, MinecraftPacketPipeReader reader) CreatePair(
        int compressionThreshold = -1, bool encrypted = false)
    {
        var pipe = new Pipe();
        var writer = new MinecraftPacketPipeWriter(pipe.Writer) { CompressionThreshold = compressionThreshold };
        var reader = new MinecraftPacketPipeReader(pipe.Reader) { CompressionThreshold = compressionThreshold };

        if (encrypted)
        {
            writer.EnableEncryption(TestKey);
            reader.EnableEncryption(TestKey);
        }

        return (writer, reader);
    }

    [Fact]
    public async Task ReadPacketsAsync_ShouldKeepBufferedPackets_WhenEnumerationBreaks()
    {
        var (writer, reader) = CreatePair();
        writer.WritePacket(1, "first"u8);
        writer.WritePacket(2, "second"u8);
        await writer.FlushAsync();

        await foreach (var packet in reader.ReadPacketsAsync())
        {
            Assert.Equal(1, packet.Id);
            break;
        }

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        await foreach (var packet in reader.ReadPacketsAsync(timeout.Token))
        {
            Assert.Equal(2, packet.Id);
            break;
        }
    }

    [Fact]
    public async Task ReadPacketAsync_ShouldReturnSecondPacket_FromSameRead()
    {
        var (writer, reader) = CreatePair();
        writer.WritePacket(1, "first"u8);
        writer.WritePacket(2, "second"u8);
        await writer.FlushAsync();

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var first = await reader.ReadPacketAsync(timeout.Token);
        Assert.Equal(1, first.Id);

        var second = await reader.ReadPacketAsync(timeout.Token);
        Assert.Equal(2, second.Id);
    }

    [Fact]
    public async Task RoundTrip_ShouldSurviveTinyWireFragments()
    {
        var wirePipe = new Pipe();
        var readerPipe = new Pipe();
        var writer = new MinecraftPacketPipeWriter(wirePipe.Writer) { CompressionThreshold = 64 };
        var reader = new MinecraftPacketPipeReader(readerPipe.Reader) { CompressionThreshold = 64 };
        writer.EnableEncryption(TestKey);
        reader.EnableEncryption(TestKey);

        var random = new Random(43);
        var bodies = new byte[50][];
        for (int i = 0; i < bodies.Length; i++)
        {
            bodies[i] = new byte[random.Next(0, 500)];
            random.NextBytes(bodies[i]);
            writer.WritePacket(i + 1, bodies[i]);
        }

        await writer.FlushAsync();
        writer.Complete();

        ReadResult wireResult = await wirePipe.Reader.ReadAsync();
        Assert.True(wireResult.IsCompleted);
        byte[] wire = wireResult.Buffer.ToArray();
        wirePipe.Reader.AdvanceTo(wireResult.Buffer.End);

        var feeder = Task.Run(async () =>
        {
            var fragments = new Random(44);
            int position = 0;
            while (position < wire.Length)
            {
                int length = Math.Min(1 + fragments.Next(7), wire.Length - position);
                await readerPipe.Writer.WriteAsync(wire.AsMemory(position, length));
                position += length;
            }

            readerPipe.Writer.Complete();
        });

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        int received = 0;
        await foreach (var packet in reader.ReadPacketsAsync(timeout.Token))
        {
            Assert.Equal(received + 1, packet.Id);
            Assert.Equal(bodies[received], packet.Data.ToArray());
            received++;
        }

        Assert.Equal(bodies.Length, received);
        await feeder;
    }

    [Fact]
    public async Task EnableEncryption_ShouldWork_MidEnumeration()
    {
        var (writer, reader) = CreatePair();
        writer.WritePacket(1, "login start"u8);
        await writer.FlushAsync();

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        int received = 0;

        await foreach (var packet in reader.ReadPacketsAsync(timeout.Token))
        {
            received++;
            if (packet.Id == 1)
            {
                reader.EnableEncryption(TestKey);
                writer.EnableEncryption(TestKey);
                writer.WritePacket(2, "encrypted follow-up"u8);
                await writer.FlushAsync();
                continue;
            }

            Assert.Equal(2, packet.Id);
            Assert.Equal("encrypted follow-up"u8.ToArray(), packet.Data.ToArray());
            break;
        }

        Assert.Equal(2, received);
    }

    [Fact]
    public async Task EnableEncryption_ShouldWork_BetweenPacketReads()
    {
        var (writer, reader) = CreatePair();
        writer.WritePacket(1, "plain handshake"u8);
        await writer.FlushAsync();

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var first = await reader.ReadPacketAsync(timeout.Token);
        Assert.Equal(1, first.Id);

        reader.EnableEncryption(TestKey);
        writer.EnableEncryption(TestKey);

        writer.WritePacket(2, "encrypted payload"u8);
        await writer.FlushAsync();

        var second = await reader.ReadPacketAsync(timeout.Token);
        Assert.Equal(2, second.Id);
        Assert.Equal("encrypted payload"u8.ToArray(), second.Data.ToArray());
    }

    [Theory]
    [InlineData(-1, false)]
    [InlineData(-1, true)]
    [InlineData(0, false)]
    [InlineData(0, true)]
    [InlineData(256, false)]
    [InlineData(256, true)]
    public async Task RoundTrip_ShouldDeliverAllPackets(int compressionThreshold, bool encrypted)
    {
        var (writer, reader) = CreatePair(compressionThreshold, encrypted);

        var random = new Random(40);
        var bodies = new byte[200][];
        for (int i = 0; i < bodies.Length; i++)
        {
            bodies[i] = new byte[random.Next(0, 5000)];
            random.NextBytes(bodies[i]);
        }

        var producer = Task.Run(async () =>
        {
            for (int i = 0; i < bodies.Length; i++)
            {
                writer.WritePacket(i + 1, bodies[i]);
                if (i % 7 == 0)
                {
                    await writer.FlushAsync();
                }
            }

            await writer.FlushAsync();
            writer.Complete();
        });

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        int received = 0;
        await foreach (var packet in reader.ReadPacketsAsync(timeout.Token))
        {
            Assert.Equal(received + 1, packet.Id);
            Assert.Equal(bodies[received], packet.Data.ToArray());
            received++;
        }

        Assert.Equal(bodies.Length, received);
        await producer;
    }
}
