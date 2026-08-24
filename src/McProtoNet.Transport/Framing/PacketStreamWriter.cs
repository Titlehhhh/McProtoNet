using System.Buffers;
using System.Diagnostics;
using McProtoNet.Transport.Cryptography;

namespace McProtoNet.Transport.Framing;

/// <summary>
/// Provides a writer that sends exactly one frame per call to a <see cref="Stream"/>.
/// </summary>
/// <remarks>
/// Each call frames the packet according to <see cref="CompressionThreshold"/>, encrypts it when
/// <see cref="Cipher"/> is set, writes it, and flushes the stream when <see cref="AutoFlush"/> is
/// <see langword="true"/>. Concurrent writes are not allowed.
/// </remarks>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketStreamWriter"/> class that writes to the
    /// specified stream.
    /// </summary>
    /// <param name="stream">The stream to write frames to.</param>
    /// <param name="leaveOpen"><see langword="true"/> to leave the stream open when the writer is
    /// disposed; otherwise, <see langword="false"/>. The default is <see langword="false"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    public PacketStreamWriter(Stream stream, bool leaveOpen = false)
    {
        ArgumentNullException.ThrowIfNull(stream);
        _leaveOpen = leaveOpen;
        _stream = stream;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the stream is flushed after every frame.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to flush the stream after every frame; otherwise,
    /// <see langword="false"/>. The default is <see langword="true"/>.
    /// </value>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="InvalidOperationException">A write is in progress.</exception>
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

    /// <summary>
    /// Gets or sets the compression threshold, in bytes. A negative value disables the compression
    /// envelope.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="InvalidOperationException">A write is in progress.</exception>
    /// <remarks>
    /// A new value applies from the next frame. Set this property between frames.
    /// </remarks>
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
    /// Gets the cipher that encrypts the whole frame before it reaches the stream.
    /// </summary>
    /// <value>
    /// The encryptor, or <see langword="null"/> when the stream is not encrypted.
    /// </value>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <remarks>
    /// The cipher can be set only between two frames. No part of the next frame is buffered, so the
    /// change always lands on a frame boundary.
    /// </remarks>
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

    /// <summary>
    /// Gets the stream that frames are written to.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    public Stream BaseStream
    {
        get
        {
            ThrowIfDisposed();
            return _stream;
        }
    }

    /// <summary>
    /// Asynchronously writes one packet that already carries its varint id.
    /// </summary>
    /// <param name="packet">The packet to write: the varint packet id followed by the body.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default
    /// value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="InvalidOperationException">Another write is already in progress.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception
    /// is stored into the returned task.</exception>
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

    /// <summary>
    /// Asynchronously writes one packet that already carries its varint id, from a segmented buffer and
    /// without joining the segments first.
    /// </summary>
    /// <param name="packet">The packet to write, split across one or more segments: the varint packet
    /// id followed by the body.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default
    /// value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="InvalidOperationException">Another write is already in progress.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception
    /// is stored into the returned task.</exception>
    /// <remarks>
    /// A packet that is compressed or encrypted is copied into one buffer first.
    /// </remarks>
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

    /// <summary>
    /// Asynchronously writes one packet, prefixing the body with the specified packet id.
    /// </summary>
    /// <param name="id">The packet id, written in front of the body as a varint.</param>
    /// <param name="body">The packet body, without the id.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default
    /// value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="InvalidOperationException">Another write is already in progress.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception
    /// is stored into the returned task.</exception>
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

    /// <summary>
    /// Asynchronously flushes the underlying stream.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default
    /// value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous flush operation.</returns>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="InvalidOperationException">A write is in progress.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception
    /// is stored into the returned task.</exception>
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

    /// <summary>
    /// Releases all resources used by the current instance of the <see cref="PacketStreamWriter"/>
    /// class.
    /// </summary>
    /// <remarks>
    /// The stream is disposed unless the writer was created with <c>leaveOpen</c> set to
    /// <see langword="true"/>.
    /// </remarks>
    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref _state, Disposed, None) == Disposed) return;

        _scratch?.Dispose();
        _scratch = null;
        if (!_leaveOpen) _stream.Dispose();
    }

    /// <summary>
    /// Asynchronously releases all resources used by the current instance of the
    /// <see cref="PacketStreamWriter"/> class.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    /// <remarks>
    /// The stream is disposed unless the writer was created with <c>leaveOpen</c> set to
    /// <see langword="true"/>.
    /// </remarks>
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.CompareExchange(ref _state, Disposed, None) == Disposed) return;

        _scratch?.Dispose();
        _scratch = null;
        if (!_leaveOpen) await _stream.DisposeAsync().ConfigureAwait(false);
    }
}
