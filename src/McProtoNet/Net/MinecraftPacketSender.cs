using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using DotNext.Buffers;
using McProtoNet.Abstractions;
using McProtoNet.Net.Zlib;
using McProtoNet.Serialization;

namespace McProtoNet.Net;

/// <summary>
/// Handles sending Minecraft protocol packets with optional compression
/// </summary>
public sealed class MinecraftPacketSender : IDisposable, IAsyncDisposable
{
    private static readonly byte[] ZeroVarint = [0];
    
    private readonly byte[] _varIntBuff = new byte[5];

    private readonly Stream _stream;
    private readonly MemoryAllocator<byte> _allocator;
    private readonly bool _leaveOpen;

    private const int NonWrite = 0;
    private const int Writing = 1;
    
    private const int None = 0;
    private const int Disposed = 1;

    private volatile int _writeState = NonWrite;
    private volatile int _state = None;
    


    private int _compressionThreshold = -1;
    private bool _autoFlush = true;

    public MinecraftPacketSender(Stream stream, bool leaveOpen = false)
        : this(stream, ArrayPool<byte>.Shared, leaveOpen)
    {
    }

    public MinecraftPacketSender(Stream stream, ArrayPool<byte> pool, bool leaveOpen = false)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(pool);
        _leaveOpen = leaveOpen;
        _stream = stream;
        _allocator = pool.ToAllocator();
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
        ThrowIfDisposed();
        if (Interlocked.CompareExchange(ref _writeState, Writing, NonWrite) == Writing)
        {
            throw new InvalidOperationException("Concurrent packet sending is not allowed.");
        }

        try
        {
            if (_compressionThreshold >= 0)
            {
                var uncompressedSize = data.Length;

                if (uncompressedSize >= _compressionThreshold)
                {
                    using var compressedBuffer = Compress(data.Span);
                    int fullSize = compressedBuffer.Length + uncompressedSize.GetVarIntLength();

                    await BaseStream.WriteVarIntAsync(
                        fullSize,
                        _varIntBuff,
                        cancellationToken).ConfigureAwait(false);


                    await BaseStream.WriteVarIntAsync(
                            uncompressedSize,
                            _varIntBuff,
                            cancellationToken)
                        .ConfigureAwait(false);

                    await BaseStream.WriteAsync(compressedBuffer.Memory, cancellationToken)
                        .ConfigureAwait(false);


                    await FlushAsyncInternal(cancellationToken).ConfigureAwait(false);
                    return;
                }

                uncompressedSize++;
                await SendShort(uncompressedSize, data, cancellationToken).ConfigureAwait(false);
                await FlushAsyncInternal(cancellationToken).ConfigureAwait(false);
                return;
            }

            await SendPacketWithoutCompressionAsync(data, cancellationToken).ConfigureAwait(false);
            await FlushAsyncInternal(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            Interlocked.Exchange(ref _writeState, NonWrite);
        }
    }

    private async Task FlushAsyncInternal(CancellationToken token)
    {
        if (_autoFlush)
        {
            await _stream.FlushAsync(token).ConfigureAwait(false);
        }
    }

    private async ValueTask SendShort(int unSize, ReadOnlyMemory<byte> data, CancellationToken token)
    {
        await BaseStream.WriteVarIntAsync(unSize, _varIntBuff, token).ConfigureAwait(false);
        await BaseStream.WriteAsync(ZeroVarint, token).ConfigureAwait(false);
        await BaseStream.WriteAsync(data, token).ConfigureAwait(false);
    }


    private async ValueTask SendPacketWithoutCompressionAsync(ReadOnlyMemory<byte> data, CancellationToken token)
    {
        var len = data.Length;

        await BaseStream.WriteVarIntAsync(len, _varIntBuff, token).ConfigureAwait(false);

        await BaseStream.WriteAsync(data, token).ConfigureAwait(false);
    }


    private MemoryOwner<byte> Compress(in ReadOnlySpan<byte> data)
    {
        var compressor = LibDeflateCache.RentCompressor();
        var length = compressor.GetBound(data.Length);

        var compressedBuffer = _allocator.AllocateExactly(length);
        try
        {
            var bytesCompress = compressor.Compress(data, compressedBuffer.Span);


            compressedBuffer.Resize(bytesCompress, _allocator);

            return compressedBuffer;
        }
        catch
        {
            compressedBuffer.Dispose();
            throw;
        }
    }

    public async Task FlushAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        if (_state == Writing)
        {
            throw new InvalidOperationException("FlushAsync called while writing is in progress");
        }

        await BaseStream.FlushAsync(cancellationToken).ConfigureAwait(false);
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