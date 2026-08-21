using System.Buffers;
using System.Diagnostics;
using McProtoNet.Transport.Cryptography;

namespace McProtoNet.Transport.Framing;

/// <summary>
///     One frame per call over a <see cref="Stream" />: frame the packet (compression threshold
///     applied), encrypt it in place when a cipher is set, write and flush. When the call returns,
///     the bytes are at the socket.
/// </summary>
public sealed class PacketStreamWriter : IDisposable, IAsyncDisposable
{
    private const int NonWrite = 0;
    private const int Writing = 1;

    private const int None = 0;
    private const int Disposed = 1;

    private readonly Stream _stream;
    private readonly bool _leaveOpen;

    private volatile int _writeState = NonWrite;
    private volatile int _state = None;

    private int _compressionThreshold = -1;
    private bool _autoFlush = true;
    private PacketCipher? _cipher;
    private PooledBufferWriter? _scratch;

    public PacketStreamWriter(Stream stream, bool leaveOpen = false)
    {
        ArgumentNullException.ThrowIfNull(stream);
        _leaveOpen = leaveOpen;
        _stream = stream;
    }

    /// <summary>Flush the stream after every frame. On by default.</summary>
    public bool AutoFlush
    {
        get
        {
            ThrowIfDisposed();
            return _autoFlush;
        }
        set
        {
            ThrowIfDisposed();
            if (_writeState == Writing)
                throw new InvalidOperationException("Cannot change auto-flush while writing a packet.");

            _autoFlush = value;
        }
    }

    /// <summary>Negative means no compression envelope. A change takes effect from the next frame.</summary>
    public int CompressionThreshold
    {
        get
        {
            ThrowIfDisposed();
            return _compressionThreshold;
        }
        set
        {
            ThrowIfDisposed();
            if (_writeState == Writing)
                throw new InvalidOperationException("Cannot change compression threshold while writing a packet.");

            _compressionThreshold = value;
        }
    }

    /// <summary>
    ///     Encryptor applied in place to the whole frame before it reaches the stream. Set it between
    ///     two frames — nothing of the next frame is buffered, so the switch lands on a frame boundary.
    /// </summary>
    public PacketCipher? Cipher
    {
        get
        {
            ThrowIfDisposed();
            return _cipher;
        }
        internal set
        {
            ThrowIfDisposed();
            if (_writeState == Writing)
                throw new InvalidOperationException("Cannot change the cipher while writing a packet.");

            _cipher = value;
        }
    }

    /// <summary>The stream frames are written to.</summary>
    public Stream BaseStream
    {
        get
        {
            ThrowIfDisposed();
            return _stream;
        }
    }

    /// <summary>Writes one packet — varint id plus body, already assembled by the caller.</summary>
    public async ValueTask WritePacketAsync(ReadOnlyMemory<byte> packet, CancellationToken cancellationToken = default)
    {
        BeginWrite(cancellationToken);
        try
        {
            if (_cipher is null)
            {
                await _stream.WritePacketAsync(packet, _compressionThreshold, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var scratch = RentScratch();
                scratch.WritePacket(packet.Span, _compressionThreshold);
                _cipher.Transform(scratch.WrittenSpan);
                await _stream.WriteAsync(scratch.WrittenMemory, cancellationToken).ConfigureAwait(false);
            }

            await FlushAsyncInternal(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            EndWrite();
        }
    }

    /// <summary>Writes one packet from a segmented buffer, without joining it first.</summary>
    public async ValueTask WritePacketAsync(ReadOnlySequence<byte> packet,
        CancellationToken cancellationToken = default)
    {
        BeginWrite(cancellationToken);
        try
        {
            if (_cipher is null)
            {
                await _stream.WritePacketAsync(packet, _compressionThreshold, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var scratch = RentScratch();
                scratch.WritePacket(in packet, _compressionThreshold);
                _cipher.Transform(scratch.WrittenSpan);
                await _stream.WriteAsync(scratch.WrittenMemory, cancellationToken).ConfigureAwait(false);
            }

            await FlushAsyncInternal(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            EndWrite();
        }
    }

    /// <summary>Writes one packet, putting the varint id in front of the body.</summary>
    public async ValueTask WritePacketAsync(int id, ReadOnlyMemory<byte> body,
        CancellationToken cancellationToken = default)
    {
        BeginWrite(cancellationToken);
        try
        {
            if (_cipher is null)
            {
                await _stream.WritePacketAsync(id, body, _compressionThreshold, cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                var scratch = RentScratch();
                scratch.WritePacket(id, body.Span, _compressionThreshold);
                _cipher.Transform(scratch.WrittenSpan);
                await _stream.WriteAsync(scratch.WrittenMemory, cancellationToken).ConfigureAwait(false);
            }

            await FlushAsyncInternal(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            EndWrite();
        }
    }

    public async Task FlushAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        if (_writeState == Writing)
            throw new InvalidOperationException("FlushAsync called while writing is in progress");

        await _stream.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    private PooledBufferWriter RentScratch()
    {
        var scratch = _scratch ??= new PooledBufferWriter(4096);
        scratch.Clear();
        return scratch;
    }

    private void BeginWrite(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (Interlocked.CompareExchange(ref _writeState, Writing, NonWrite) == Writing)
            throw new InvalidOperationException("Concurrent packet sending is not allowed.");
    }

    private void EndWrite() => Interlocked.Exchange(ref _writeState, NonWrite);

    private async ValueTask FlushAsyncInternal(CancellationToken token)
    {
        if (_autoFlush) await _stream.FlushAsync(token).ConfigureAwait(false);
    }

    [StackTraceHidden]
    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_state == Disposed, this);

    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref _state, Disposed, None) == Disposed) return;

        _scratch?.Dispose();
        _scratch = null;
        if (!_leaveOpen) _stream.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.CompareExchange(ref _state, Disposed, None) == Disposed) return;

        _scratch?.Dispose();
        _scratch = null;
        if (!_leaveOpen) await _stream.DisposeAsync().ConfigureAwait(false);
    }
}
