using System.Buffers;
using System.Runtime.CompilerServices;
using McProtoNet.Primitives;
using McProtoNet.Transport.Cryptography;
using McProtoNet.Transport.Framing;

namespace McProtoNet.Transport;

/// <summary>
///     A connection in streaming mode: reads come in batches, writes are synchronous into a buffer
///     and leave on <see cref="FlushAsync" />. Neither the cipher nor the compression threshold can
///     change here — that is what makes the big buffers safe. Reached only through
///     <see cref="MinecraftConnection.ToStreaming" />.
/// </summary>
/// <remarks>
///     One reader, one writer, no queue inside: send policy belongs to the caller. Another thread
///     may only call <see cref="Abort" />. A batch and its bodies live until the next
///     <see cref="ReadBatchAsync" />.
///     <para>
///     What comes out when a call cannot go through: <see cref="ObjectDisposedException" /> after
///     <see cref="DisposeAsync" />; <see cref="InvalidOperationException" /> for a misuse that never
///     touched the stream — two reads at once — which leaves the connection alone; an
///     <see cref="OperationCanceledException" /> only for the caller's own cancelled token;
///     <see cref="ConnectionAbortedException" /> for everything else, with the reason as its inner
///     exception. The first failure of the stream itself comes out as it is and is latched — every
///     call after it reports the closed connection and that reason.
///     </para>
/// </remarks>
public sealed class StreamingConnection : IAsyncDisposable
{
    private readonly Stream _stream;
    private readonly bool _leaveOpen;
    private readonly PacketCipher? _encryptor;
    private readonly PacketCipher? _decryptor;
    private readonly BufferedPacketReader _reader;
    private readonly BufferedPacketWriter _writer;
    private readonly TaskCompletionSource _completion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    private int _closed;
    private Exception? _closeReason;
    private int _disposed;

    internal StreamingConnection(Stream stream, bool leaveOpen, int compressionThreshold,
        PacketCipher? encryptor, PacketCipher? decryptor)
    {
        _stream = stream;
        _leaveOpen = leaveOpen;
        _encryptor = encryptor;
        _decryptor = decryptor;
        _reader = new BufferedPacketReader(stream, compressionThreshold, decryptor);
        _writer = new BufferedPacketWriter(stream, compressionThreshold, encryptor);
    }

    /// <summary>The stream underneath — the raw escape hatch for proxies and replays.</summary>
    public Stream BaseStream => _stream;

    /// <summary>Negative means no compression envelope. Fixed for the life of this connection.</summary>
    public int CompressionThreshold => _reader.CompressionThreshold;

    /// <summary>True when the connection was moved here with encryption already on.</summary>
    public bool IsEncrypted => _encryptor is not null;

    /// <summary>
    ///     Completes when the connection is closed, cleanly or not. Never faults. There are no pumps
    ///     behind it, so it means "closed", not "every in-flight call has returned".
    /// </summary>
    public Task Completion => _completion.Task;

    /// <summary>Why the connection ended. Null for a clean end of stream or <see cref="CompleteAsync" />.</summary>
    public Exception? CloseReason => Volatile.Read(ref _closeReason);

    /// <summary>
    ///     Bytes framed but not yet handed to the stream. A closed connection reports 0 — nothing it
    ///     still holds will ever go out — so it stays readable after <see cref="CompleteAsync" />.
    /// </summary>
    public long UnflushedBytes
    {
        get
        {
            ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) == 1, this);
            return Volatile.Read(ref _closed) == 1 || _writer.IsBroken ? 0 : _writer.UnflushedBytes;
        }
    }

    /// <summary>
    ///     Every whole frame that one read produced. An empty batch with
    ///     <see cref="PacketBatch.IsCompleted" /> means the stream ended.
    /// </summary>
    public async ValueTask<PacketBatch> ReadBatchAsync(CancellationToken token = default)
    {
        ThrowIfClosed();
        PacketBatch batch;
        try
        {
            batch = await _reader.ReadBatchAsync(token).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // cancellation by the caller's own token leaves the reader usable; anything else is
            // the connection dying
            var closed = OnFailure(ex, token);
            if (closed is null) throw;
            throw closed;
        }

        if (batch is { Count: 0, IsCompleted: true }) Close(null, closeStream: false);
        return batch;
    }

    /// <summary>Batches flattened into one sequence. Enumeration ends when the stream ends.</summary>
    public async IAsyncEnumerable<IncomingPacket> ReadPacketsAsync(
        [EnumeratorCancellation] CancellationToken token = default)
    {
        while (true)
        {
            var batch = await ReadBatchAsync(token).ConfigureAwait(false);
            if (batch.Count == 0)
            {
                if (batch.IsCompleted) yield break;
                continue;
            }

            foreach (var packet in batch) yield return packet;
        }
    }

    /// <summary>Frames one packet into the send buffer. Nothing leaves until <see cref="FlushAsync" />.</summary>
    public void WritePacket(ReadOnlySpan<byte> packet)
    {
        ThrowIfClosed();
        _writer.WritePacket(packet);
    }

    /// <summary>Frames one packet, putting the varint id in front of the body.</summary>
    public void WritePacket(int id, ReadOnlySpan<byte> body)
    {
        ThrowIfClosed();
        _writer.WritePacket(id, body);
    }

    /// <summary>Frames one packet whose body is split across segments, without joining it first.</summary>
    public void WritePacket(int id, in ReadOnlySequence<byte> body)
    {
        ThrowIfClosed();
        _writer.WritePacket(id, in body);
    }

    /// <summary>
    ///     One write plus one stream flush. When it returns, everything written so far is at the
    ///     socket. A cancelled or failed flush kills the connection — part of a frame may be on the
    ///     wire — so the caller who cancelled still gets its own
    ///     <see cref="OperationCanceledException" />, and every call after that gets
    ///     <see cref="ConnectionAbortedException" />.
    /// </summary>
    public async ValueTask FlushAsync(CancellationToken token = default)
    {
        ThrowIfClosed();
        try
        {
            await _writer.FlushAsync(token).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            var closed = OnFlushFailure(ex, token);
            if (closed is null) throw;
            throw closed;
        }
    }

    /// <summary>Flushes what is left and closes cleanly.</summary>
    public async ValueTask CompleteAsync()
    {
        // it promises the bytes are on the wire: on a closed connection that would be a lie
        ThrowIfClosed();

        try
        {
            await _writer.CompleteAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            var closed = OnFlushFailure(ex, CancellationToken.None);
            if (closed is null) throw;
            throw closed;
        }

        Close(null, closeStream: false);
    }

    /// <summary>
    ///     Closes the connection from any thread; an in-flight read or flush fails with the reason.
    ///     The stream goes down even when the caller owns it — that is what breaks the in-flight call.
    /// </summary>
    public void Abort(Exception? reason = null) => Close(reason, closeStream: true);

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 1) return;

        Close(null, closeStream: false);
        await Completion.ConfigureAwait(false);

        _reader.Dispose();
        _writer.Dispose();
        _encryptor?.Dispose();
        _decryptor?.Dispose();
    }

    private void Close(Exception? reason, bool closeStream)
    {
        // whoever asks for the stream to go down gets it, win or lose the race to close: a late
        // Abort must still break the call parked inside the stream. Dispose is idempotent.
        var kill = closeStream || !_leaveOpen;

        if (Volatile.Read(ref _closed) == 1)
        {
            if (kill) KillStream();
            return;
        }

        // reason first: a thread that sees _closed == 1 must also see why
        Interlocked.CompareExchange(ref _closeReason, reason, null);
        if (Interlocked.Exchange(ref _closed, 1) == 1)
        {
            if (kill) KillStream();
            return;
        }

        if (kill) KillStream();
        _completion.TrySetResult();
    }

    private void KillStream()
    {
        try
        {
            _stream.Dispose();
        }
        catch
        {
        }
    }

    /// <summary>
    ///     Latches a dead stream so the next call reports the close instead of reading a corpse, and
    ///     says what the caller must see. Null means the original exception, rethrown with its stack.
    /// </summary>
    private Exception? OnFailure(Exception ex, CancellationToken token)
    {
        if (ex is OperationCanceledException && token.IsCancellationRequested) return null;

        if (Volatile.Read(ref _closed) == 1) return new ConnectionAbortedException(CloseReason);

        // a misuse never reached the stream: the caller's bug, not the connection's death.
        // ObjectDisposedException derives from InvalidOperationException but means the stream is
        // gone, so it is a death, not a misuse — and a closed connection is checked before both.
        if (ex is InvalidOperationException and not ObjectDisposedException) return null;

        Close(ex, closeStream: true);

        // a cancellation that is not the caller's is a failure of the stream, not a cancellation
        return ex is OperationCanceledException ? new ConnectionAbortedException(ex) : null;
    }

    /// <summary>
    ///     Same, for the send side, where even the caller's own cancellation is fatal: the buffered
    ///     writer is dead after any failed flush and part of a frame may already be on the wire. The
    ///     caller who cancelled still gets its own cancellation; the connection goes down behind it.
    /// </summary>
    private Exception? OnFlushFailure(Exception ex, CancellationToken token)
    {
        if (Volatile.Read(ref _closed) == 1)
            return ex is OperationCanceledException && token.IsCancellationRequested
                ? null
                : new ConnectionAbortedException(CloseReason);

        // a misuse never reached the stream: the caller's bug, not the connection's death.
        // ObjectDisposedException derives from InvalidOperationException but means the stream is
        // gone, so it is a death, not a misuse — and a closed connection is checked before both.
        if (ex is InvalidOperationException and not ObjectDisposedException) return null;

        // a flush that never handed a byte to the stream leaves the writer whole, and with it the
        // connection: an empty buffer plus an already-cancelled token must not kill anything
        if (!_writer.IsBroken && ex is OperationCanceledException && token.IsCancellationRequested)
            return null;

        Close(ex, closeStream: true);
        if (ex is not OperationCanceledException) return null;
        return token.IsCancellationRequested ? null : new ConnectionAbortedException(ex);
    }

    private void ThrowIfClosed()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) == 1, this);
        if (Volatile.Read(ref _closed) == 1) ThrowHelper.ThrowAborted(CloseReason);
    }
}
