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
    private bool _disposed;

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

    /// <summary>Bytes framed but not yet handed to the stream.</summary>
    public long UnflushedBytes
    {
        get
        {
            ThrowIfClosed();
            return _writer.UnflushedBytes;
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
        catch (Exception ex) when (IsAborted(ex))
        {
            throw new ConnectionAbortedException(CloseReason);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // cancellation leaves the reader usable; anything else is the connection dying
            Close(ex, closeStream: true);
            throw;
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
    ///     socket. A cancelled or failed flush kills the connection — part of a frame may be on the wire.
    /// </summary>
    public async ValueTask FlushAsync(CancellationToken token = default)
    {
        ThrowIfClosed();
        try
        {
            await _writer.FlushAsync(token).ConfigureAwait(false);
        }
        catch (Exception ex) when (IsAborted(ex))
        {
            throw new ConnectionAbortedException(CloseReason);
        }
        catch (Exception ex)
        {
            Close(ex, closeStream: true);
            throw;
        }
    }

    /// <summary>Flushes what is left and closes cleanly.</summary>
    public async ValueTask CompleteAsync()
    {
        if (Volatile.Read(ref _closed) == 1) return;

        try
        {
            await _writer.CompleteAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Close(ex, closeStream: true);
            throw;
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
        if (_disposed) return;
        _disposed = true;

        Close(null, closeStream: false);
        await Completion.ConfigureAwait(false);

        _reader.Dispose();
        _writer.Dispose();
        _encryptor?.Dispose();
        _decryptor?.Dispose();
    }

    private void Close(Exception? reason, bool closeStream)
    {
        if (Volatile.Read(ref _closed) == 1) return;

        // reason first: a thread that sees _closed == 1 must also see why
        Interlocked.CompareExchange(ref _closeReason, reason, null);
        if (Interlocked.Exchange(ref _closed, 1) == 1) return;

        if (closeStream || !_leaveOpen)
        {
            try
            {
                _stream.Dispose();
            }
            catch
            {
            }
        }

        _completion.TrySetResult();
    }

    private bool IsAborted(Exception ex) =>
        ex is not OperationCanceledException && Volatile.Read(ref _closed) == 1;

    private void ThrowIfClosed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (Volatile.Read(ref _closed) == 1) throw new ConnectionAbortedException(CloseReason);
    }
}
