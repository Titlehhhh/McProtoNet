using McProtoNet.Primitives;
using McProtoNet.Transport.Cryptography;
using McProtoNet.Transport.Framing;

namespace McProtoNet.Transport;

/// <summary>
///     A connection in one-at-a-time mode: one frame per read, one frame per write, nothing held
///     back. This is where the switches live — the compression threshold and encryption — because
///     nothing of the next frame is ever buffered, so a switch always lands between two frames.
///     Handshaking, status and login live here; <see cref="ToStreaming" /> moves the rest of the
///     connection to <see cref="StreamingConnection" />.
/// </summary>
/// <remarks>
///     One reader, one writer, no queue inside: send policy belongs to the caller.
///     <see cref="Abort" /> may be called from any thread.
///     <para>
///     What comes out when a call cannot go through: <see cref="ObjectDisposedException" /> after
///     <see cref="DisposeAsync" />; <see cref="InvalidOperationException" /> for a misuse that never
///     touched the stream — two reads at once, a connection already moved — which leaves the
///     connection alone; <see cref="OperationCanceledException" /> only for the caller's own
///     cancelled token; <see cref="ConnectionAbortedException" /> for everything else, with the
///     reason as its inner exception. The first failure of the stream itself comes out as it is and
///     is latched — every call after it reports the closed connection and that reason.
///     </para>
///     <para>
///     Cancelling a read or a write here kills the connection. One frame at a time means the call
///     has already eaten part of a frame off the wire, or put part of one on it, and the cipher has
///     moved with those bytes; there is no resuming from that. Whoever cancelled still gets its own
///     <see cref="OperationCanceledException" />, and every call after it gets
///     <see cref="ConnectionAbortedException" />. A token already cancelled before the call started
///     costs nothing and leaves the connection open.
///     </para>
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

    public MinecraftConnection(Stream stream, bool leaveOpen = false)
    {
        ArgumentNullException.ThrowIfNull(stream);
        _stream = stream;
        _leaveOpen = leaveOpen;
        _reader = new PacketStreamReader(stream, leaveOpen: true);
        _writer = new PacketStreamWriter(stream, leaveOpen: true);
    }

    /// <summary>The stream underneath — the raw escape hatch for proxies and replays.</summary>
    public Stream BaseStream => _stream;

    /// <summary>
    ///     Completes when the connection is closed, cleanly or not, and also once
    ///     <see cref="ToStreaming" /> has handed everything over — after the move this object owns
    ///     nothing, so it is done whatever the streaming connection goes on to do. Never faults.
    /// </summary>
    public Task Completion => _completion.Task;

    /// <summary>Why the connection ended. Null for a clean close or while it is still open.</summary>
    public Exception? CloseReason => Volatile.Read(ref _closeReason);

    /// <summary>Negative means no compression envelope. A change takes effect from the next frame.</summary>
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

    /// <summary>True once <see cref="EnableEncryption" /> has run.</summary>
    public bool IsEncrypted => _encryptor is not null;

    /// <summary>
    ///     Turns on AES/CFB8 in both directions from the next frame on. Call it right after the frame
    ///     that agreed the secret has been written and before the next one is read.
    /// </summary>
    public void EnableEncryption(ReadOnlySpan<byte> sharedSecret)
    {
        ThrowIfUnusable();
        if (_encryptor is not null) ThrowHelper.ThrowEncryptionAlreadyEnabled();

        _encryptor = PacketCipher.CreateEncryptor(sharedSecret);
        _decryptor = PacketCipher.CreateDecryptor(sharedSecret);
        _reader.Cipher = _decryptor;
        _writer.Cipher = _encryptor;
    }

    /// <summary>Reads exactly one frame. The body window is valid until the next read.</summary>
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

    /// <summary>Writes one packet and flushes. When it returns, the bytes are at the socket.</summary>
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

    /// <summary>Writes one packet, putting the varint id in front of the body, and flushes.</summary>
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
    ///     Consumes this connection and hands the stream, the ciphers and the threshold to a
    ///     streaming one. Afterwards every member here throws, <see cref="DisposeAsync" /> is empty
    ///     and <see cref="Abort" /> goes to the streaming connection instead.
    /// </summary>
    /// <remarks>
    ///     Like a read, this invalidates the body of the last packet <see cref="ReadPacketAsync" />
    ///     returned — decode it before moving.
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
    ///     Closes the stream from any thread; an in-flight read or write fails with the reason.
    ///     After <see cref="ToStreaming" /> this aborts the streaming connection instead, so a
    ///     watchdog holding the old reference still brings the right one down with the right reason.
    /// </summary>
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

    /// <summary>
    ///     Latches a dead stream so the next call reports the close instead of reading a corpse, and
    ///     says what the caller must see. Null means the original exception, rethrown with its stack.
    /// </summary>
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
