using McProtoNet.Primitives;
using McProtoNet.Transport.Cryptography;
using McProtoNet.Transport.Framing;

namespace McProtoNet.Transport;

/// <summary>
/// Represents a connection that reads and writes one frame at a time over a stream.
/// </summary>
/// <remarks>
/// <para>
/// No part of the next frame is ever buffered, so the compression threshold and encryption can be
/// switched between two frames. <see cref="ToStreaming"/> hands the connection over to a
/// <see cref="StreamingConnection"/>, which reads in batches instead.
/// </para>
/// <para>
/// The connection supports one reader and one writer and holds no send queue. <see cref="Abort"/>
/// can be called from any thread.
/// </para>
/// <para>
/// A call that cannot go through throws <see cref="ObjectDisposedException"/> after
/// <see cref="DisposeAsync"/>, <see cref="InvalidOperationException"/> for a misuse that never
/// reached the stream, such as two concurrent reads or a connection that was already moved, which
/// leaves the connection usable, <see cref="OperationCanceledException"/> for the caller's own
/// canceled token, and <see cref="ConnectionAbortedException"/> in every other case, with the reason
/// as its inner exception. The first failure of the stream itself is thrown as it is and is latched;
/// every later call reports the closed connection and that reason.
/// </para>
/// <para>
/// Cancellation of a read or a write that has already started closes the connection. The caller
/// that canceled receives <see cref="OperationCanceledException"/>, and every later call receives
/// <see cref="ConnectionAbortedException"/>. A token that is already canceled when the call starts
/// leaves the connection open.
/// </para>
/// </remarks>
public sealed class MinecraftConnection : IAsyncDisposable
{
    private readonly Stream _stream;
    private readonly bool _leaveOpen;
    private readonly PacketStreamReader _reader;
    private readonly PacketStreamWriter _writer;
    private readonly TaskCompletionSource _completion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    private PacketCipher? _encryptor;
    private PacketCipher? _decryptor;
    private StreamingConnection? _movedTo;
    private int _threshold = -1;
    private int _moved;
    private int _closed;
    private Exception? _closeReason;
    private int _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="MinecraftConnection"/> class over the specified stream.
    /// </summary>
    /// <param name="stream">The duplex stream that frames are read from and written to.</param>
    /// <param name="leaveOpen"><see langword="true"/> to leave the stream open when the connection is
    /// disposed; otherwise, <see langword="false"/>. The default is <see langword="false"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    public MinecraftConnection(Stream stream, bool leaveOpen = false)
    {
        ArgumentNullException.ThrowIfNull(stream);
        _stream = stream;
        _leaveOpen = leaveOpen;
        _reader = new PacketStreamReader(stream, leaveOpen: true);
        _writer = new PacketStreamWriter(stream, leaveOpen: true);
    }

    /// <summary>
    /// Gets the stream that frames are read from and written to.
    /// </summary>
    public Stream BaseStream => _stream;

    /// <summary>
    /// Gets a task that completes when the connection is closed.
    /// </summary>
    /// <remarks>
    /// The task also completes once <see cref="ToStreaming"/> has handed the stream over, because
    /// this instance then owns nothing. The task never faults.
    /// </remarks>
    public Task Completion => _completion.Task;

    /// <summary>
    /// Gets the exception that ended the connection.
    /// </summary>
    /// <value>
    /// The reason the connection ended, or <see langword="null"/> for a clean close or while the
    /// connection is still open.
    /// </value>
    public Exception? CloseReason => Volatile.Read(ref _closeReason);

    /// <summary>
    /// Gets or sets the compression threshold, in bytes. A negative value disables the compression envelope.
    /// </summary>
    /// <exception cref="InvalidOperationException">The connection was moved to streaming mode by
    /// <see cref="ToStreaming"/>.</exception>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="ConnectionAbortedException">The connection is closed.</exception>
    /// <remarks>
    /// A new value applies from the next frame in both directions.
    /// </remarks>
    public int CompressionThreshold
    {
        get
        {
            ThrowIfUnusable();
            return _threshold;
        }
        set
        {
            ThrowIfUnusable();
            _threshold = value;
            _reader.CompressionThreshold = value;
            _writer.CompressionThreshold = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether encryption is enabled on the connection.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if <see cref="EnableEncryption"/> has been called; otherwise,
    /// <see langword="false"/>.
    /// </value>
    public bool IsEncrypted => _encryptor is not null;

    /// <summary>
    /// Enables AES/CFB8 encryption in both directions.
    /// </summary>
    /// <param name="sharedSecret">The shared secret, which must be exactly
    /// <see cref="PacketCipher.SharedSecretLength"/> bytes long. It serves as both the key and the
    /// initialization vector.</param>
    /// <exception cref="ArgumentException"><paramref name="sharedSecret"/> is not
    /// <see cref="PacketCipher.SharedSecretLength"/> bytes long.</exception>
    /// <exception cref="InvalidOperationException">Encryption is already enabled.
    /// -or-
    /// The connection was moved to streaming mode by <see cref="ToStreaming"/>.</exception>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="ConnectionAbortedException">The connection is closed.</exception>
    /// <remarks>
    /// Encryption applies from the next frame. Call this method after the frame that agreed the
    /// secret has been written and before the next frame is read.
    /// </remarks>
    public void EnableEncryption(ReadOnlySpan<byte> sharedSecret)
    {
        ThrowIfUnusable();
        if (_encryptor is not null) ThrowHelper.ThrowEncryptionAlreadyEnabled();

        _encryptor = PacketCipher.CreateEncryptor(sharedSecret);
        _decryptor = PacketCipher.CreateDecryptor(sharedSecret);
        _reader.Cipher = _decryptor;
        _writer.Cipher = _encryptor;
    }

    /// <summary>
    /// Asynchronously reads exactly one frame from the connection.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests. The default value is
    /// <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous read operation. The result contains the
    /// packet that was read.</returns>
    /// <exception cref="InvalidOperationException">Another read is already in progress.
    /// -or-
    /// The connection was moved to streaming mode by <see cref="ToStreaming"/>.</exception>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="ConnectionAbortedException">The connection is closed, or the stream failed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This
    /// exception is stored into the returned task.</exception>
    /// <remarks>
    /// The packet owns the pooled block behind <see cref="IncomingPacket.Body"/>: dispose it when it is
    /// no longer needed, or keep it as long as needed.
    /// </remarks>
    public async ValueTask<IncomingPacket> ReadPacketAsync(CancellationToken token = default)
    {
        ThrowIfUnusable();

        // nothing is off the wire yet, so this cancellation costs the connection nothing
        token.ThrowIfCancellationRequested();
        try
        {
            return await _reader.ReadPacketAsync(token).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            var closed = OnFailure(ex, token);
            if (closed is null) throw;
            throw closed;
        }
    }

    /// <summary>
    /// Asynchronously writes one packet that already carries its varint id and flushes the stream.
    /// </summary>
    /// <param name="packet">The packet to write: the varint packet id followed by the body.</param>
    /// <param name="token">The token to monitor for cancellation requests. The default value is
    /// <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous write operation. When it completes, the bytes
    /// have been handed to the stream and flushed.</returns>
    /// <exception cref="InvalidOperationException">Another write is already in progress.
    /// -or-
    /// The connection was moved to streaming mode by <see cref="ToStreaming"/>.</exception>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="ConnectionAbortedException">The connection is closed, or the stream failed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This
    /// exception is stored into the returned task.</exception>
    public async ValueTask WritePacketAsync(ReadOnlyMemory<byte> packet, CancellationToken token = default)
    {
        ThrowIfUnusable();
        token.ThrowIfCancellationRequested();
        try
        {
            await _writer.WritePacketAsync(packet, token).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            var closed = OnFailure(ex, token);
            if (closed is null) throw;
            throw closed;
        }
    }

    /// <summary>
    /// Asynchronously writes one packet, prefixing the body with the specified packet id, and flushes
    /// the stream.
    /// </summary>
    /// <param name="id">The packet id, written in front of the body as a varint.</param>
    /// <param name="body">The packet body, without the id.</param>
    /// <param name="token">The token to monitor for cancellation requests. The default value is
    /// <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous write operation. When it completes, the bytes
    /// have been handed to the stream and flushed.</returns>
    /// <exception cref="InvalidOperationException">Another write is already in progress.
    /// -or-
    /// The connection was moved to streaming mode by <see cref="ToStreaming"/>.</exception>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="ConnectionAbortedException">The connection is closed, or the stream failed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This
    /// exception is stored into the returned task.</exception>
    public async ValueTask WritePacketAsync(int id, ReadOnlyMemory<byte> body, CancellationToken token = default)
    {
        ThrowIfUnusable();
        token.ThrowIfCancellationRequested();
        try
        {
            await _writer.WritePacketAsync(id, body, token).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            var closed = OnFailure(ex, token);
            if (closed is null) throw;
            throw closed;
        }
    }

    /// <summary>
    /// Switches the connection to streaming mode and returns the connection that takes it over.
    /// </summary>
    /// <returns>A <see cref="StreamingConnection"/> that owns the stream, the ciphers and the
    /// compression threshold of this instance.</returns>
    /// <exception cref="InvalidOperationException">The connection was already moved to streaming
    /// mode.</exception>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="ConnectionAbortedException">The connection is closed.</exception>
    /// <remarks>
    /// After the move, every other member of this instance throws
    /// <see cref="InvalidOperationException"/>, <see cref="DisposeAsync"/> does nothing, and
    /// <see cref="Abort"/> aborts the streaming connection instead. Packets already read are not
    /// affected: each owns its buffer.
    /// </remarks>
    public StreamingConnection ToStreaming()
    {
        ThrowIfUnusable();
        if (Interlocked.Exchange(ref _moved, 1) == 1) ThrowHelper.ThrowAlreadyMoved();

        var streaming = new StreamingConnection(_stream, _leaveOpen, _threshold, _encryptor, _decryptor);

        // published before the bricks go: an Abort from another thread must find it from here on
        Volatile.Write(ref _movedTo, streaming);
        _reader.Dispose();
        _writer.Dispose();

        // Abort may have landed between the check and the move: the new object inherits the close
        if (Volatile.Read(ref _closed) == 1) streaming.Abort(CloseReason);

        _completion.TrySetResult();
        return streaming;
    }

    /// <summary>
    /// Closes the connection and the underlying stream.
    /// </summary>
    /// <param name="reason">The exception to report as <see cref="CloseReason"/>, or
    /// <see langword="null"/> for a clean close. The default value is <see langword="null"/>.</param>
    /// <remarks>
    /// This method can be called from any thread. A read or a write in progress fails with the
    /// reason. The stream is closed even when the connection was created with <c>leaveOpen</c> set to
    /// <see langword="true"/>. After <see cref="ToStreaming"/>, the call aborts the streaming
    /// connection instead.
    /// </remarks>
    public void Abort(Exception? reason = null)
    {
        var moved = Volatile.Read(ref _movedTo);
        if (moved is not null)
        {
            moved.Abort(reason);
            return;
        }

        Close(reason, closeStream: true);
    }

    /// <summary>
    /// Asynchronously releases all resources used by the current instance of the
    /// <see cref="MinecraftConnection"/> class.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    /// <remarks>
    /// The underlying stream is closed unless the connection was created with <c>leaveOpen</c> set to
    /// <see langword="true"/>. After <see cref="ToStreaming"/>, this method does nothing.
    /// </remarks>
    public ValueTask DisposeAsync()
    {
        if (Volatile.Read(ref _moved) == 1) return ValueTask.CompletedTask;
        if (Interlocked.Exchange(ref _disposed, 1) == 1) return ValueTask.CompletedTask;

        Close(null, closeStream: false);
        _reader.Dispose();
        _writer.Dispose();
        _encryptor?.Dispose();
        _decryptor?.Dispose();
        return ValueTask.CompletedTask;
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
        if (Volatile.Read(ref _closed) == 1)
            return ex is OperationCanceledException && token.IsCancellationRequested
                ? null
                : new ConnectionAbortedException(CloseReason);

        // a misuse never reached the stream: the caller's bug, not the connection's death.
        // ObjectDisposedException derives from InvalidOperationException but means the stream is
        // gone, so it is a death, not a misuse — and a closed connection is checked before both.
        if (ex is InvalidOperationException and not ObjectDisposedException) return null;

        // part of a frame is gone either way, and the cipher moved with it
        Close(ex, closeStream: true);

        if (ex is not OperationCanceledException) return null;

        // a cancellation that is not the caller's is a failure of the stream, not a cancellation
        return token.IsCancellationRequested ? null : new ConnectionAbortedException(ex);
    }

    private void ThrowIfUnusable()
    {
        if (Volatile.Read(ref _moved) == 1) ThrowHelper.ThrowMovedToStreaming();
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) == 1, this);
        if (Volatile.Read(ref _closed) == 1) ThrowHelper.ThrowAborted(CloseReason);
    }
}
