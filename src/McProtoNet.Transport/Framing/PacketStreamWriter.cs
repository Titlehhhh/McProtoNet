using System.Buffers;
using System.Diagnostics;

namespace McProtoNet.Transport.Framing;

/// <summary>
/// Handles sending Minecraft protocol packets with optional compression
/// </summary>
public sealed class PacketStreamWriter : IDisposable, IAsyncDisposable
{
    private readonly Stream _stream;
    private readonly bool _leaveOpen;

    private const int NonWrite = 0;
    private const int Writing = 1;

    private const int None = 0;
    private const int Disposed = 1;

    private volatile int _writeState = NonWrite;
    private volatile int _state = None;

    private int _compressionThreshold = -1;
    private bool _autoFlush = true;

    public PacketStreamWriter(Stream stream, bool leaveOpen = false)
    {
        ArgumentNullException.ThrowIfNull(stream);
        _leaveOpen = leaveOpen;
        _stream = stream;
    }

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
            {
                throw new InvalidOperationException(
                    "Cannot change auto-flush while writing a packet.");
            }

            _autoFlush = value;
        }
    }

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
            {
                throw new InvalidOperationException("Cannot change compression threshold while writing a packet.");
            }

            _compressionThreshold = value;
        }
    }

    public Stream BaseStream
    {
        get
        {
            ThrowIfDisposed();
            return _stream;
        }
    }

    /// <summary>
    /// Sends a packet asynchronously with optional compression
    /// </summary>
    /// <param name="data">The packet data to send</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>A ValueTask representing the asynchronous operation</returns>
    public async ValueTask SendPacketAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        BeginWrite(cancellationToken);
        try
        {
            await _stream.WritePacketAsync(data, _compressionThreshold, cancellationToken).ConfigureAwait(false);
            await FlushAsyncInternal(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            EndWrite();
        }
    }

    /// <summary>
    /// Sends a packet asynchronously from a buffer sequence, without joining it first
    /// </summary>
    /// <param name="data">The packet data to send</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>A ValueTask representing the asynchronous operation</returns>
    public async ValueTask SendPacketAsync(ReadOnlySequence<byte> data, CancellationToken cancellationToken = default)
    {
        BeginWrite(cancellationToken);
        try
        {
            await _stream.WritePacketAsync(data, _compressionThreshold, cancellationToken).ConfigureAwait(false);
            await FlushAsyncInternal(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            EndWrite();
        }
    }

    /// <summary>
    /// Sends a packet asynchronously, writing the VarInt id in front of the body
    /// </summary>
    /// <param name="id">The packet id</param>
    /// <param name="body">The packet body, without the id</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>A ValueTask representing the asynchronous operation</returns>
    public async ValueTask SendPacketAsync(int id, ReadOnlyMemory<byte> body,
        CancellationToken cancellationToken = default)
    {
        BeginWrite(cancellationToken);
        try
        {
            await _stream.WritePacketAsync(id, body, _compressionThreshold, cancellationToken).ConfigureAwait(false);
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
        {
            throw new InvalidOperationException("FlushAsync called while writing is in progress");
        }

        await _stream.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    private void BeginWrite(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (Interlocked.CompareExchange(ref _writeState, Writing, NonWrite) == Writing)
        {
            throw new InvalidOperationException("Concurrent packet sending is not allowed.");
        }
    }

    private void EndWrite()
    {
        Interlocked.Exchange(ref _writeState, NonWrite);
    }

    private async ValueTask FlushAsyncInternal(CancellationToken token)
    {
        if (_autoFlush)
        {
            await _stream.FlushAsync(token).ConfigureAwait(false);
        }
    }

    [StackTraceHidden]
    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_state == Disposed, this);
    }

    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref _state, Disposed, None) == Disposed)
        {
            return;
        }

        if (!_leaveOpen)
        {
            _stream.Dispose();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.CompareExchange(ref _state, Disposed, None) == Disposed)
        {
            return;
        }

        if (!_leaveOpen)
        {
            await _stream.DisposeAsync().ConfigureAwait(false);
        }
    }
}
