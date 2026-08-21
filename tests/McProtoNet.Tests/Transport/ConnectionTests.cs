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

    /// <summary>Disposed and aborted are different answers, and they are the same two on both connections.</summary>
    [Fact]
    public async Task DisposedAndAborted_AreDistinguishableOnBothConnections()
    {
        var token = TestContext.Current.CancellationToken;
        var reason = new IOException("kicked");

        var disposedLogin = new MinecraftConnection(new GateStream());
        await disposedLogin.DisposeAsync();
        Assert.Throws<ObjectDisposedException>(() => disposedLogin.CompressionThreshold = 0);
        await Assert.ThrowsAsync<ObjectDisposedException>(
            async () => await disposedLogin.ReadPacketAsync(token));

        var abortedLogin = new MinecraftConnection(new GateStream());
        abortedLogin.Abort(reason);
        var loginError = Assert.Throws<ConnectionAbortedException>(() => abortedLogin.CompressionThreshold = 0);
        Assert.Same(reason, loginError.InnerException);
        Assert.Contains("aborted", loginError.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("kicked", loginError.Message, StringComparison.Ordinal);
        Assert.Same(reason, abortedLogin.CloseReason);
        Assert.True(abortedLogin.Completion.IsCompletedSuccessfully);

        var disposedGame = new MinecraftConnection(new GateStream()).ToStreaming();
        await disposedGame.DisposeAsync();
        Assert.Throws<ObjectDisposedException>(() => disposedGame.WritePacket(1, "x"u8));
        Assert.Throws<ObjectDisposedException>(() => _ = disposedGame.UnflushedBytes);

        await using var abortedGame = new MinecraftConnection(new GateStream()).ToStreaming();
        abortedGame.Abort(reason);
        var gameError = Assert.Throws<ConnectionAbortedException>(() => abortedGame.WritePacket(1, "x"u8));
        Assert.Same(reason, gameError.InnerException);
        Assert.Contains("kicked", gameError.Message, StringComparison.Ordinal);
    }

    /// <summary>A dead stream is latched: the second call reports the close, it does not read the corpse again.</summary>
    [Fact]
    public async Task StreamFailure_LatchesTheLoginConnection()
    {
        var token = TestContext.Current.CancellationToken;
        await using var login = new MinecraftConnection(new FailingStream(new IOException("peer reset")));

        var first = await Assert.ThrowsAsync<IOException>(async () => await login.ReadPacketAsync(token));
        Assert.Equal("peer reset", first.Message);
        Assert.Same(first, login.CloseReason);
        Assert.True(login.Completion.IsCompletedSuccessfully);

        var later = await Assert.ThrowsAsync<ConnectionAbortedException>(
            async () => await login.ReadPacketAsync(token));
        Assert.Same(first, later.InnerException);
        Assert.Throws<ConnectionAbortedException>(() => login.CompressionThreshold = 0);
    }

    /// <summary>A cancellation that is not the caller's is a broken stream, not a cancellation.</summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ForeignCancellation_ComesOutAsAborted(bool streaming)
    {
        var token = TestContext.Current.CancellationToken;
        using var foreign = new CancellationTokenSource();
        await foreign.CancelAsync();
        var stream = new FailingStream(new OperationCanceledException("the stream gave up", foreign.Token));

        var login = new MinecraftConnection(stream);
        if (!streaming)
        {
            var error = await Assert.ThrowsAsync<ConnectionAbortedException>(
                async () => await login.ReadPacketAsync(token));
            Assert.IsAssignableFrom<OperationCanceledException>(error.InnerException);
            Assert.Same(error.InnerException, login.CloseReason);
            await login.DisposeAsync();
            return;
        }

        await using var game = login.ToStreaming();
        var gameError = await Assert.ThrowsAsync<ConnectionAbortedException>(
            async () => await game.ReadBatchAsync(token));
        Assert.IsAssignableFrom<OperationCanceledException>(gameError.InnerException);
        Assert.Same(gameError.InnerException, game.CloseReason);
    }

    /// <summary>
    ///     A cancelled flush kills the connection: the caller who cancelled still gets its own
    ///     cancellation, everyone after it gets the close — never a stale cancellation.
    /// </summary>
    [Fact]
    public async Task CancelledFlush_KillsTheConnectionWithoutAStaleCancellation()
    {
        var gate = new GateStream();
        await using var game = new MinecraftConnection(gate).ToStreaming();
        game.WritePacket(1, new byte[64]);

        using var cts = new CancellationTokenSource();
        var flush = game.FlushAsync(cts.Token).AsTask();
        await gate.WriteStarted;
        await cts.CancelAsync();

        var cancelled = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => flush);
        Assert.Same(cancelled, game.CloseReason);

        var next = Assert.Throws<ConnectionAbortedException>(() => game.WritePacket(2, "x"u8));
        Assert.IsAssignableFrom<OperationCanceledException>(next.InnerException);
    }

    /// <summary>A closed connection still answers how much it holds: nothing.</summary>
    [Fact]
    public async Task UnflushedBytes_StaysReadableAfterCompleteAsync()
    {
        var token = TestContext.Current.CancellationToken;
        await using var wire = await Loopback.CreateAsync(token);

        var game = new MinecraftConnection(wire.Client).ToStreaming();
        game.WritePacket(9, "last words"u8);
        Assert.True(game.UnflushedBytes > 0);

        await game.CompleteAsync();

        Assert.Equal(0, game.UnflushedBytes);
        Assert.Null(game.CloseReason);
        Assert.Throws<ConnectionAbortedException>(() => game.WritePacket(1, "x"u8));

        await game.DisposeAsync();
        Assert.Throws<ObjectDisposedException>(() => _ = game.UnflushedBytes);
    }

    /// <summary>A closed connection reports 0 even when frames were still sitting in the buffer.</summary>
    [Fact]
    public async Task UnflushedBytes_IsZeroAfterAnAbortWithFramesPending()
    {
        await using var game = new MinecraftConnection(new GateStream()).ToStreaming();

        game.WritePacket(1, new byte[64]);
        Assert.True(game.UnflushedBytes > 0);

        game.Abort(new IOException("kicked"));

        Assert.Equal(0, game.UnflushedBytes);
    }

    /// <summary>CompleteAsync promises the bytes are on the wire, so on a closed connection it must not lie.</summary>
    [Fact]
    public async Task CompleteAsync_OnAClosedConnection_Throws()
    {
        var reason = new IOException("kicked");
        var aborted = new MinecraftConnection(new GateStream()).ToStreaming();
        aborted.WritePacket(1, new byte[64]);
        aborted.Abort(reason);

        var error = await Assert.ThrowsAsync<ConnectionAbortedException>(async () => await aborted.CompleteAsync());
        Assert.Same(reason, error.InnerException);
        await aborted.DisposeAsync();

        var disposed = new MinecraftConnection(new GateStream()).ToStreaming();
        await disposed.DisposeAsync();
        await Assert.ThrowsAsync<ObjectDisposedException>(async () => await disposed.CompleteAsync());
    }

    /// <summary>A flush that hands the stream nothing must not take the connection down with it.</summary>
    [Fact]
    public async Task CancelledFlushOfAnEmptyBuffer_LeavesTheConnectionOpen()
    {
        var token = TestContext.Current.CancellationToken;
        await using var wire = await Loopback.CreateAsync(token);
        await using var game = new MinecraftConnection(wire.Client).ToStreaming();

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await game.FlushAsync(cts.Token));

        Assert.Null(game.CloseReason);
        Assert.False(game.Completion.IsCompleted);

        // still usable: the frame goes out on a live token
        game.WritePacket(3, "alive"u8);
        await game.FlushAsync(token);
    }

    /// <summary>
    ///     One frame at a time cannot resume: a cancelled read has already eaten part of a frame and
    ///     moved the cipher, so the connection dies rather than desync in silence.
    /// </summary>
    [Fact]
    public async Task CancelledReadInFlight_KillsTheLoginConnection()
    {
        var token = TestContext.Current.CancellationToken;
        var gate = new GateStream();
        await using var login = new MinecraftConnection(gate);

        using var cts = new CancellationTokenSource();
        var read = login.ReadPacketAsync(cts.Token).AsTask();
        await gate.ReadStarted;
        await cts.CancelAsync();

        var cancelled = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => read);
        Assert.Same(cancelled, login.CloseReason);
        await login.Completion;

        var later = await Assert.ThrowsAsync<ConnectionAbortedException>(
            async () => await login.ReadPacketAsync(token));
        Assert.Same(cancelled, later.InnerException);
    }

    /// <summary>A token cancelled before the call starts costs nothing, so nothing is torn down.</summary>
    [Fact]
    public async Task PreCancelledRead_LeavesTheLoginConnectionOpen()
    {
        await using var login = new MinecraftConnection(new GateStream());

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            async () => await login.ReadPacketAsync(cts.Token));

        Assert.Null(login.CloseReason);
        Assert.False(login.Completion.IsCompleted);
    }

    /// <summary>Two reads at once is the caller's bug; it must not cost anyone the connection.</summary>
    [Fact]
    public async Task ConcurrentRead_IsAUsageErrorAndLeavesTheConnectionOpen()
    {
        var token = TestContext.Current.CancellationToken;
        var gate = new GateStream();
        var game = new MinecraftConnection(gate).ToStreaming();

        var first = game.ReadBatchAsync(token).AsTask();
        await gate.ReadStarted;

        await Assert.ThrowsAsync<InvalidOperationException>(async () => await game.ReadBatchAsync(token));
        Assert.Null(game.CloseReason);
        Assert.False(game.Completion.IsCompleted);

        game.Abort();
        await Assert.ThrowsAnyAsync<Exception>(() => first);
        await game.DisposeAsync();
    }

    /// <summary>A watchdog holding the pre-move reference must still bring down the right connection.</summary>
    [Fact]
    public async Task AbortAfterToStreaming_ReachesTheStreamingConnection()
    {
        var login = new MinecraftConnection(new GateStream());
        await using var game = login.ToStreaming();

        var reason = new IOException("watchdog");
        login.Abort(reason);

        Assert.Same(reason, game.CloseReason);
        await game.Completion;
        var error = Assert.Throws<ConnectionAbortedException>(() => game.WritePacket(1, "x"u8));
        Assert.Same(reason, error.InnerException);
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
