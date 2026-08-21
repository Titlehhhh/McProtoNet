using McProtoNet.Tests.Infrastructure;
using McProtoNet.Transport;
using McProtoNet.Transport.Cryptography;
using McProtoNet.Transport.Framing;

namespace McProtoNet.Tests.Transport;

public class ConnectionTests
{
    private static readonly byte[] Secret = "ABCDEFGHIJKLMGASDASDGSGSDF"u8[..16].ToArray();

    /// <summary>
    ///     The whole ladder in one test: login one frame at a time, the cipher and the threshold turned
    ///     on between frames, then the move to streaming — and not a byte lost across the move.
    /// </summary>
    [Fact]
    public async Task ToStreaming_CarriesCipherAndThreshold()
    {
        var token = TestContext.Current.CancellationToken;
        await using var wire = await Loopback.CreateAsync(token);

        var login = new MinecraftConnection(wire.Client);
        using var serverReader = new PacketStreamReader(wire.Server, leaveOpen: true);
        using var serverWriter = new PacketStreamWriter(wire.Server, leaveOpen: true);

        // handshake in the clear
        await login.WritePacketAsync(0x00, "handshake"u8.ToArray(), token);
        var handshake = await serverReader.ReadPacketAsync(token);
        Assert.Equal(0x00, handshake.Id);
        Assert.Equal("handshake"u8.ToArray(), handshake.Body.ToArray());

        await serverWriter.WritePacketAsync(0x01, "encryption_request"u8.ToArray(), token);
        var request = await login.ReadPacketAsync(token);
        Assert.Equal(0x01, request.Id);

        // both sides switch between two frames
        login.EnableEncryption(Secret);
        serverReader.Cipher = PacketCipher.CreateDecryptor(Secret);
        serverWriter.Cipher = PacketCipher.CreateEncryptor(Secret);
        Assert.True(login.IsEncrypted);

        await login.WritePacketAsync(0x02, "encryption_response"u8.ToArray(), token);
        var response = await serverReader.ReadPacketAsync(token);
        Assert.Equal("encryption_response"u8.ToArray(), response.Body.ToArray());

        // ... and again for compression
        login.CompressionThreshold = 256;
        serverReader.CompressionThreshold = 256;
        serverWriter.CompressionThreshold = 256;

        var big = new byte[4000];
        Random.Shared.NextBytes(big);
        await login.WritePacketAsync(0x03, big, token);
        var compressed = await serverReader.ReadPacketAsync(token);
        Assert.Equal(big, compressed.Body.ToArray());

        // the move: same stream, same ciphers, same threshold
        await using var game = login.ToStreaming();
        Assert.True(game.IsEncrypted);
        Assert.Equal(256, game.CompressionThreshold);

        var packets = Frames.Sample(seed: 55, repeats: 1);

        // both directions carry more than a socket buffer holds, so each side reads while the other writes
        var serverReads = Task.Run(async () =>
        {
            var got = new List<TestPacket>();
            foreach (var _ in packets)
            {
                var packet = await serverReader.ReadPacketAsync(token);
                got.Add(new TestPacket(packet.Id, packet.Body.ToArray()));
            }

            return got;
        }, token);

        foreach (var packet in packets) game.WritePacket(packet.Id, packet.Body);
        Assert.True(game.UnflushedBytes > 0);
        await game.FlushAsync(token);
        Assert.Equal(0, game.UnflushedBytes);
        AssertSame(packets, await serverReads);

        var serverWrites = Task.Run(async () =>
        {
            foreach (var packet in packets)
                await serverWriter.WritePacketAsync(packet.Id, packet.Body, token);
        }, token);

        var received = new List<TestPacket>();
        while (received.Count < packets.Count)
        {
            var batch = await game.ReadBatchAsync(token);
            foreach (var packet in batch) received.Add(new TestPacket(packet.Id, packet.Body.ToArray()));
        }

        await serverWrites;
        AssertSame(packets, received);
    }

    private static void AssertSame(IReadOnlyList<TestPacket> expected, IReadOnlyList<TestPacket> actual)
    {
        Assert.Equal(expected.Count, actual.Count);
        for (var i = 0; i < expected.Count; i++)
        {
            Assert.Equal(expected[i].Id, actual[i].Id);
            Assert.Equal(expected[i].Body, actual[i].Body);
        }
    }

    [Fact]
    public async Task MovedConnection_ThrowsOnEveryMember()
    {
        var login = new MinecraftConnection(new GateStream());
        await using var game = login.ToStreaming();

        Assert.Throws<InvalidOperationException>(() => login.CompressionThreshold = 0);
        Assert.Throws<InvalidOperationException>(() => login.EnableEncryption(Secret));
        Assert.Throws<InvalidOperationException>(() => login.ToStreaming());
        Assert.True(login.DisposeAsync().IsCompletedSuccessfully);

        game.Abort();
    }


    [Fact]
    public async Task AbortFromAnotherThread_FailsAnInFlightRead()
    {
        var gate = new GateStream();
        await using var game = new MinecraftConnection(gate).ToStreaming();

        var read = game.ReadBatchAsync(TestContext.Current.CancellationToken).AsTask();
        await gate.ReadStarted;

        var reason = new InvalidOperationException("kicked");
        await Task.Run(() => game.Abort(reason));

        var error = await Assert.ThrowsAsync<ConnectionAbortedException>(() => read);
        Assert.Same(reason, error.InnerException);
        Assert.Same(reason, game.CloseReason);
        Assert.True(game.Completion.IsCompletedSuccessfully);
        Assert.Throws<ConnectionAbortedException>(() => game.WritePacket(1, "x"u8));
    }

    [Fact]
    public async Task AbortFromAnotherThread_FailsAnInFlightFlush()
    {
        var gate = new GateStream();
        await using var game = new MinecraftConnection(gate).ToStreaming();

        game.WritePacket(1, new byte[64]);
        var flush = game.FlushAsync(TestContext.Current.CancellationToken).AsTask();
        await gate.WriteStarted;

        var reason = new IOException("reset");
        await Task.Run(() => game.Abort(reason));

        var error = await Assert.ThrowsAsync<ConnectionAbortedException>(() => flush);
        Assert.Same(reason, error.InnerException);
        Assert.Same(reason, game.CloseReason);
        await game.Completion;
    }

    [Fact]
    public async Task CleanEndOfStream_CompletesWithoutAReason()
    {
        var token = TestContext.Current.CancellationToken;
        await using var wire = await Loopback.CreateAsync(token);

        var login = new MinecraftConnection(wire.Client);
        using (var serverWriter = new PacketStreamWriter(wire.Server, leaveOpen: true))
        {
            await serverWriter.WritePacketAsync(7, "bye"u8.ToArray(), token);
        }

        await using var game = login.ToStreaming();
        wire.Server.Dispose();

        var seen = new List<int>();
        await foreach (var packet in game.ReadPacketsAsync(token)) seen.Add(packet.Id);

        Assert.Equal([7], seen);
        Assert.Null(game.CloseReason);
        await game.Completion;
        Assert.Throws<ConnectionAbortedException>(() => game.WritePacket(1, "x"u8));
    }

    [Fact]
    public async Task CompleteAsync_FlushesAndCloses()
    {
        var token = TestContext.Current.CancellationToken;
        await using var wire = await Loopback.CreateAsync(token);

        var game = new MinecraftConnection(wire.Client).ToStreaming();
        game.WritePacket(9, "last words"u8);
        await game.CompleteAsync();

        Assert.Null(game.CloseReason);
        await game.Completion;

        using var serverReader = new PacketStreamReader(wire.Server, leaveOpen: true);
        var packet = await serverReader.ReadPacketAsync(token);
        Assert.Equal(9, packet.Id);
        Assert.Equal("last words"u8.ToArray(), packet.Body.ToArray());

        await game.DisposeAsync();
    }

    [Fact]
    public async Task CancelledRead_LosesNothing()
    {
        var token = TestContext.Current.CancellationToken;
        var packets = Frames.SmallSample(seed: 63);
        var wire = Frames.Build(packets, -1, false);

        var stream = new ScriptedReadStream();
        using var reader = new BufferedPacketReader(stream);

        stream.Push(wire.AsSpan(0, 1));

        using var cts = new CancellationTokenSource();
        var read = reader.ReadBatchAsync(cts.Token).AsTask();
        await Task.Delay(50, token);
        await cts.CancelAsync();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => read);

        stream.Push(wire.AsSpan(1));

        var received = new List<TestPacket>();
        while (received.Count < packets.Count)
        {
            var batch = await reader.ReadBatchAsync(token);
            foreach (var packet in batch) received.Add(new TestPacket(packet.Id, packet.Body.ToArray()));
        }

        for (var i = 0; i < packets.Count; i++)
        {
            Assert.Equal(packets[i].Id, received[i].Id);
            Assert.Equal(packets[i].Body, received[i].Body);
        }
    }
}
