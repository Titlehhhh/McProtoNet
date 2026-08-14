using System.Buffers;
using System.IO.Pipelines;
using McProtoNet.Cryptography;
using McProtoNet.Net;
using McProtoNet.Serialization;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace McProtoNet.Tests.Pipelines;

/// <summary>
/// End-to-end send/receive tests for MinecraftPacketSender + MinecraftPacketPipeReader
/// covering all combinations of compression and encryption.
/// </summary>
public class PacketPipelineTests
{
    // ── AES-CFB8 helpers ──────────────────────────────────────────────────────

    private static readonly byte[] AesKey = "McProtoNetTestKey"u8[..16].ToArray();

    private static IBufferedCipher CreateDecryptor() => CreateCipher(false);

    private static IBufferedCipher CreateCipher(bool forEncryption)
    {
        var c = CipherUtilities.GetCipher("AES/CFB8/NoPadding");
        c.Init(forEncryption, new ParametersWithIV(new KeyParameter(AesKey), AesKey, 0, 16));
        return c;
    }

    // ── ReadOnlySequence helper (avoids ImmutableArrayExtensions.ToArray ambiguity) ──

    private static byte[] SeqToArray(System.Buffers.ReadOnlySequence<byte> seq)
    {
        if (seq.IsSingleSegment)
            return seq.FirstSpan.ToArray();
        var arr = new byte[seq.Length];
        int pos = 0;
        foreach (var mem in seq)
        {
            mem.Span.CopyTo(arr.AsSpan(pos));
            pos += mem.Length;
        }
        return arr;
    }

    // ── Packet data helpers ───────────────────────────────────────────────────

    private static byte[] MakePayload(int size, byte seed = 0xAB)
    {
        var data = new byte[size];
        for (int i = 0; i < size; i++)
            data[i] = (byte)(seed + i);
        return data;
    }

    // ── Build transport bytes: sender → MemoryStream ─────────────────────────

    private static async Task<byte[]> SendPacketsAsync(
        byte[][] payloads,
        int compressionThreshold = -1,
        bool encrypt = false)
    {
        var transport = new MemoryStream();
        Stream writeStream = transport;

        AesStream? aesStream = null;
        if (encrypt)
        {
            aesStream = new AesStream(transport);
            aesStream.SwitchEncryption(AesKey);
            writeStream = aesStream;
        }

        await using var sender = new MinecraftPacketSender(writeStream, leaveOpen: true);
        sender.CompressionThreshold = compressionThreshold;

        var ct = TestContext.Current.CancellationToken;
        foreach (var payload in payloads)
            await sender.SendPacketAsync(payload, ct);

        await writeStream.FlushAsync(ct);
        return transport.ToArray();
    }

    // ── Build MinecraftPacketPipeReader from raw bytes ────────────────────────

    private static async Task<MinecraftPacketPipeReader> BuildReaderAsync(
        byte[] bytes,
        int compressionThreshold = -1,
        bool decrypt = false)
    {
        var pipe = new Pipe();
        await pipe.Writer.WriteAsync(bytes, TestContext.Current.CancellationToken);
        pipe.Writer.Complete();

        var reader = new MinecraftPacketPipeReader(pipe.Reader);
        reader.CompressionThreshold = compressionThreshold;

        if (decrypt)
            reader.EnableEncryption(PacketCipher.CreateDecryptor(AesKey));

        return reader;
    }

    // ── Tests: no compression, no encryption ─────────────────────────────────

    [Theory]
    [InlineData(100)]
    [InlineData(500)]
    [InlineData(5000)]
    public async Task NoCompression_NoEncryption_SinglePacket(int payloadSize)
    {
        var payload = MakePayload(payloadSize);
        var bytes = await SendPacketsAsync([payload]);
        var reader = await BuildReaderAsync(bytes);

        var packet = await reader.ReadPacketAsync(TestContext.Current.CancellationToken);
        Assert.Equal(payloadSize, packet.Data.Length);
        Assert.Equal(payload, packet.Data.ToArray());
    }

    [Fact]
    public async Task NoCompression_NoEncryption_MultiplePackets_VersionToken()
    {
        var payloads = new[] { MakePayload(100), MakePayload(200), MakePayload(300) };
        var bytes = await SendPacketsAsync(payloads);
        var reader = await BuildReaderAsync(bytes);
        var ct = TestContext.Current.CancellationToken;

        InputPacket prev = default;
        bool hasPrev = false;

        for (int i = 0; i < payloads.Length; i++)
        {
            var packet = await reader.ReadPacketAsync(ct);
            Assert.Equal(payloads[i].Length, packet.Data.Length);
            Assert.Equal(payloads[i], packet.Data.ToArray());

            // After reading next packet the previous sequence must be invalidated
            if (hasPrev)
            {
                var captured = prev;
                Assert.Throws<InvalidOperationException>(() => { _ = captured.FullLength; });
            }

            prev = packet;
            hasPrev = true;
        }
    }

    // ── Tests: compression ────────────────────────────────────────────────────

    [Theory]
    [InlineData(50,  256)]  // below threshold → sent uncompressed (dataLength = 0 header)
    [InlineData(300, 256)]  // above threshold → compressed
    [InlineData(1,   1)]    // threshold=1, everything ≥ 1 compresses
    [InlineData(1000, 512)] // larger payload, compressed
    public async Task Compression_NoEncryption_SinglePacket(int payloadSize, int threshold)
    {
        var payload = MakePayload(payloadSize);
        var bytes = await SendPacketsAsync([payload], compressionThreshold: threshold);
        var reader = await BuildReaderAsync(bytes, compressionThreshold: threshold);

        var packet = await reader.ReadPacketAsync(TestContext.Current.CancellationToken);
        Assert.Equal(payloadSize, packet.Data.Length);
        Assert.Equal(payload, packet.Data.ToArray());
    }

    [Fact]
    public async Task Compression_NoEncryption_MixedSize_MultiplePackets()
    {
        const int threshold = 256;
        var payloads = new[] { MakePayload(50, 0x10), MakePayload(1000, 0x20) };
        var bytes = await SendPacketsAsync(payloads, compressionThreshold: threshold);
        var reader = await BuildReaderAsync(bytes, compressionThreshold: threshold);
        var ct = TestContext.Current.CancellationToken;

        var p1 = await reader.ReadPacketAsync(ct);
        Assert.Equal(payloads[0], p1.Data.ToArray());

        var p2 = await reader.ReadPacketAsync(ct);
        Assert.Equal(payloads[1], p2.Data.ToArray());
    }

    // ── Tests: encryption ─────────────────────────────────────────────────────

    [Theory]
    [InlineData(100)]
    [InlineData(500)]
    public async Task NoCompression_WithEncryption_SinglePacket(int payloadSize)
    {
        var payload = MakePayload(payloadSize);
        var encBytes  = await SendPacketsAsync([payload], encrypt: true);
        var plainBytes = await SendPacketsAsync([payload], encrypt: false);

        // Wire bytes must differ
        Assert.NotEqual(plainBytes, encBytes);

        var reader = await BuildReaderAsync(encBytes, decrypt: true);
        var packet = await reader.ReadPacketAsync(TestContext.Current.CancellationToken);

        Assert.Equal(payloadSize, packet.Data.Length);
        Assert.Equal(payload, packet.Data.ToArray());
    }

    // ── Tests: compression + encryption ──────────────────────────────────────

    [Theory]
    [InlineData(50,  256)]  // below threshold (uncompressed header) + encrypted
    [InlineData(500, 256)]  // compressed + encrypted
    public async Task Compression_WithEncryption_SinglePacket(int payloadSize, int threshold)
    {
        var payload = MakePayload(payloadSize);
        var bytes = await SendPacketsAsync([payload], compressionThreshold: threshold, encrypt: true);
        var reader = await BuildReaderAsync(bytes, compressionThreshold: threshold, decrypt: true);

        var packet = await reader.ReadPacketAsync(TestContext.Current.CancellationToken);
        Assert.Equal(payloadSize, packet.Data.Length);
        Assert.Equal(payload, packet.Data.ToArray());
    }

    [Fact]
    public async Task Compression_WithEncryption_MultiplePackets()
    {
        const int threshold = 128;
        var payloads = new[]
        {
            MakePayload(50,   0x10),  // below threshold — uncompressed
            MakePayload(300,  0x20),  // above threshold — compressed
            MakePayload(1,    0x30),  // tiny — uncompressed
        };

        var bytes = await SendPacketsAsync(payloads, compressionThreshold: threshold, encrypt: true);
        var reader = await BuildReaderAsync(bytes, compressionThreshold: threshold, decrypt: true);
        var ct = TestContext.Current.CancellationToken;

        for (int i = 0; i < payloads.Length; i++)
        {
            var packet = await reader.ReadPacketAsync(ct);
            Assert.Equal(payloads[i].Length, packet.Data.Length);
            Assert.Equal(payloads[i], packet.Data.ToArray());
        }
    }

    // ── IAsyncEnumerable ──────────────────────────────────────────────────────

    [Fact]
    public async Task IAsyncEnumerable_NoCompression_NoEncryption_50Packets()
    {
        var payloads = Enumerable.Range(0, 50)
            .Select(i => MakePayload(100 + i, (byte)i))
            .ToArray();

        var bytes = await SendPacketsAsync(payloads);
        var reader = await BuildReaderAsync(bytes);

        int count = 0;
        await foreach (var packet in reader.ReadPacketsAsync(TestContext.Current.CancellationToken))
        {
            Assert.Equal(payloads[count], packet.Data.ToArray());
            count++;
        }
        Assert.Equal(payloads.Length, count);
    }

    [Fact]
    public async Task IAsyncEnumerable_WithCompression_50Packets()
    {
        const int threshold = 150;
        var payloads = Enumerable.Range(0, 50)
            .Select(i => MakePayload(100 + i * 5, (byte)i))
            .ToArray();

        var bytes = await SendPacketsAsync(payloads, compressionThreshold: threshold);
        var reader = await BuildReaderAsync(bytes, compressionThreshold: threshold);

        int count = 0;
        await foreach (var packet in reader.ReadPacketsAsync(TestContext.Current.CancellationToken))
        {
            Assert.Equal(payloads[count], packet.Data.ToArray());
            count++;
        }
        Assert.Equal(payloads.Length, count);
    }
}
