using System.Buffers;
using System.Runtime.CompilerServices;
using McProtoNet.Primitives;
using McProtoNet.Transport.Cryptography;
using McProtoNet.Transport.Framing;

namespace McProtoNet.Transport;

/// <summary>
/// Represents a connection that reads packets in batches and buffers writes until they are flushed.
/// </summary>
/// <remarks>
/// <para>
/// An instance is obtained only from <see cref="MinecraftConnection.ToStreaming"/>. Neither the
/// cipher nor the compression threshold can change on this connection.
/// </para>
/// <para>
/// The connection supports one reader and one writer and holds no send queue. Another thread may
/// call only <see cref="Abort"/>. A batch and the packet bodies it hands out stay valid until the
/// next <see cref="ReadBatchAsync"/>.
/// </para>
/// <para>
/// A call that cannot go through throws <see cref="ObjectDisposedException"/> after
/// <see cref="DisposeAsync"/>, <see cref="InvalidOperationException"/> for a misuse that never
/// reached the stream, such as two concurrent reads, which leaves the connection usable,
/// <see cref="OperationCanceledException"/> for the caller's own canceled token, and
/// <see cref="ConnectionAbortedException"/> in every other case, with the reason as its inner
/// exception. The first failure of the stream itself is thrown as it is and is latched; every later
/// call reports the closed connection and that reason.
/// </para>
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

    /// <summary>
    /// Gets the stream that frames are read from and written to.
    /// </summary>
    public Stream BaseStream => _stream;

    /// <summary>
    /// Gets the compression threshold, in bytes. A negative value disables the compression envelope.
    /// </summary>
    /// <remarks>
    /// The value is fixed for the life of the connection.
    /// </remarks>
    public int CompressionThreshold => _reader.CompressionThreshold;

    /// <summary>
    /// Gets a value indicating whether encryption is enabled on the connection.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the connection was moved here with encryption already enabled;
    /// otherwise, <see langword="false"/>.
    /// </value>
    public bool IsEncrypted => _encryptor is not null;

    /// <summary>
    /// Gets a task that completes when the connection is closed.
    /// </summary>
    /// <remarks>
    /// The task never faults. Completion means the connection is closed, not that every call in
    /// progress has returned.
    /// </remarks>
    public Task Completion => _completion.Task;

    /// <summary>
    /// Gets the exception that ended the connection.
    /// </summary>
    /// <value>
    /// The reason the connection ended, or <see langword="null"/> for a clean end of stream, for a
    /// close through <see cref="CompleteAsync"/>, or while the connection is still open.
    /// </value>
    public Exception? CloseReason => Volatile.Read(ref _closeReason);

    /// <summary>
    /// Gets the number of bytes that are framed but not yet handed to the stream.
    /// </summary>
    /// <value>
    /// The number of buffered bytes, or 0 when the connection is closed or the writer failed.
    /// </value>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <remarks>
    /// This property remains readable after <see cref="CompleteAsync"/>.
    /// </remarks>
    public long UnflushedBytes
    {
        get
        {
            ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) == 1, this);
            return Volatile.Read(ref _closed) == 1 || _writer.IsBroken ? 0 : _writer.UnflushedBytes;
        }
    }

    /// <summary>
    /// Asynchronously reads every whole frame that one read produced.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests. The default value is
    /// <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous read operation. The result contains the batch
    /// that was read. An empty batch whose <see cref="PacketBatch.IsCompleted"/> is
    /// <see langword="true"/> means the stream ended.</returns>
    /// <exception cref="InvalidOperationException">Another read is already in progress.</exception>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="ConnectionAbortedException">The connection is closed, or the stream failed.</exception>
    /// <exception cref="EndOfStreamException">The stream ended in the middle of a frame.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This
    /// exception is stored into the returned task.</exception>
    /// <remarks>
    /// The batch and the packet bodies it hands out stay valid until the next call. Cancellation
    /// through <paramref name="token"/> leaves the connection open.
    /// </remarks>
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

    /// <summary>
    /// Asynchronously reads batches and returns their packets as a single sequence.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests. The default value is
    /// <see cref="CancellationToken.None"/>.</param>
    /// <returns>A sequence of packets that ends when the stream ends.</returns>
    /// <remarks>
    /// Every packet body stays valid only until the next batch is read. The exceptions of
    /// <see cref="ReadBatchAsync"/> surface during enumeration.
    /// </remarks>
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

    /// <summary>
    /// Frames one packet that already carries its varint id into the send buffer.
    /// </summary>
    /// <param name="packet">The packet to frame: the varint packet id followed by the body.</param>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="ConnectionAbortedException">The connection is closed.</exception>
    /// <exception cref="InvalidOperationException">A flush failed part-way, so the writer can no
    /// longer be used.</exception>
    /// <remarks>
    /// Nothing is sent until <see cref="FlushAsync"/> or <see cref="CompleteAsync"/> is called.
    /// </remarks>
    public void WritePacket(ReadOnlySpan<byte> packet)
    {
        ThrowIfClosed();
        _writer.WritePacket(packet);
    }

    /// <summary>
    /// Frames one packet into the send buffer, prefixing the body with the specified packet id.
    /// </summary>
    /// <param name="id">The packet id, written in front of the body as a varint.</param>
    /// <param name="body">The packet body, without the id.</param>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="ConnectionAbortedException">The connection is closed.</exception>
    /// <exception cref="InvalidOperationException">A flush failed part-way, so the writer can no
    /// longer be used.</exception>
    /// <remarks>
    /// Nothing is sent until <see cref="FlushAsync"/> or <see cref="CompleteAsync"/> is called.
    /// </remarks>
    public void WritePacket(int id, ReadOnlySpan<byte> body)
    {
        ThrowIfClosed();
        _writer.WritePacket(id, body);
    }

    /// <summary>
    /// Frames one packet into the send buffer from a segmented body, without joining the segments
    /// first.
    /// </summary>
    /// <param name="id">The packet id, written in front of the body as a varint.</param>
    /// <param name="body">The packet body, without the id, split across one or more segments.</param>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="ConnectionAbortedException">The connection is closed.</exception>
    /// <exception cref="InvalidOperationException">A flush failed part-way, so the writer can no
    /// longer be used.</exception>
    /// <remarks>
    /// Nothing is sent until <see cref="FlushAsync"/> or <see cref="CompleteAsync"/> is called. A body
    /// that is compressed is copied into one buffer first.
    /// </remarks>
    public void WritePacket(int id, in ReadOnlySequence<byte> body)
    {
        ThrowIfClosed();
        _writer.WritePacket(id, in body);
    }

    /// <summary>
    /// Asynchronously sends everything framed so far in one write and flushes the stream.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests. The default value is
    /// <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous flush operation. When it completes, the bytes
    /// have been handed to the stream and flushed.</returns>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="ConnectionAbortedException">The connection is closed, or the stream failed.</exception>
    /// <exception cref="InvalidOperationException">A flush failed part-way, so the writer can no
    /// longer be used.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This
    /// exception is stored into the returned task.</exception>
    /// <remarks>
    /// A flush that fails or is canceled after a byte has reached the stream closes the connection.
    /// The caller that canceled receives <see cref="OperationCanceledException"/>, and every later
    /// call receives
    /// <see cref="ConnectionAbortedException"/>. Cancellation with an empty send buffer leaves the
    /// connection open.
    /// </remarks>
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

    /// <summary>
    /// Asynchronously sends everything framed so far and closes the connection cleanly.
    /// </summary>
    /// <returns>A task that represents the asynchronous complete operation.</returns>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="ConnectionAbortedException">The connection is closed, or the stream failed.</exception>
    /// <exception cref="InvalidOperationException">A flush failed part-way, so the writer can no
    /// longer be used.</exception>
    /// <remarks>
    /// <see cref="CloseReason"/> stays <see langword="null"/> after this method. The underlying stream
    /// is closed unless the connection was created with <c>leaveOpen</c> set to
    /// <see langword="true"/>.
    /// </remarks>
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
    /// Closes the connection and the underlying stream.
    /// </summary>
    /// <param name="reason">The exception to report as <see cref="CloseReason"/>, or
    /// <see langword="null"/> for a clean close. The default value is <see langword="null"/>.</param>
    /// <remarks>
    /// This method can be called from any thread. A read or a flush in progress fails with the
    /// reason. The stream is closed even when the connection was created with <c>leaveOpen</c> set to
    /// <see langword="true"/>.
    /// </remarks>
    public void Abort(Exception? reason = null) => Close(reason, closeStream: true);

    /// <summary>
    /// Asynchronously releases all resources used by the current instance of the
    /// <see cref="StreamingConnection"/> class.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    /// <remarks>
    /// Bytes that are framed but not flushed are discarded. The underlying stream is closed unless the
    /// connection was created with <c>leaveOpen</c> set to <see langword="true"/>.
    /// </remarks>
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

    // Latches a dead stream and returns what the caller must see; null means rethrow the original.
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

    // Same for the send side, where the caller's own cancellation is fatal too: the buffered writer is
    // dead after any failed flush and part of a frame may already be on the wire.
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
