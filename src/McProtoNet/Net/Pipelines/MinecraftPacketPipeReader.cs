using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using DotNext.Buffers;
using McProtoNet.Internal;
using McProtoNet.Net.Zlib;
using McProtoNet.Serialization;
using Org.BouncyCastle.Crypto;

namespace McProtoNet.Net;

public sealed class MinecraftPacketPipeReader : PipeReader, IDisposable
{
    private static readonly NullOwner DisposedMemoryOwner = new();

    private readonly DecryptedPipeReader _pipeReader;

    // For single packet
    private PositionState? _positionState;


    private const int None = 0;
    private const int Disposed = 1;

    private volatile int _state = None;


    private volatile IMemoryOwner<byte>? _desompressedBuffer;

    private readonly PacketSourceCore _sourceCore = new();

    public MinecraftPacketPipeReader(PipeReader pipeReader)
    {
        this._pipeReader = new DecryptedPipeReader(pipeReader);
    }

    public int CompressionThreshold { get; set; } = -1;

    public bool EncryptionEnabled => _pipeReader.IsEncrypted;

    public void EnableEncryption(IBufferedCipher decryptor)
    {
        ThrowIfDisposed();
        _pipeReader.SwitchEncryption(decryptor);
    }

    public override bool TryRead(out ReadResult result)
    {
        ThrowIfDisposed();
        return _pipeReader.TryRead(out result);
    }

    public override async ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        var result = await _pipeReader.ReadAsync(cancellationToken).ConfigureAwait(false);

        if (result.IsCompleted)
        {
            CompleteCore();
        }

        return result;
    }

    public override void AdvanceTo(SequencePosition consumed)
    {
        ThrowIfDisposed();
        _pipeReader.AdvanceTo(consumed);
    }

    public override void AdvanceTo(SequencePosition consumed, SequencePosition examined)
    {
        ThrowIfDisposed();
        _pipeReader.AdvanceTo(consumed, examined);
    }

    private void CompleteCore()
    {
        if (Interlocked.CompareExchange(ref _state, Disposed, None) == Disposed)
        {
            return;
        }

        var old = Interlocked.Exchange(ref _desompressedBuffer, DisposedMemoryOwner);
        if (old != DisposedMemoryOwner && old is not null)
        {
            old.Dispose();
        }
    }

    private void SetDecompressedBuffer(ref MemoryOwner<byte> buffer)
    {
        var old = Interlocked.Exchange(ref _desompressedBuffer, buffer);
        if (old != DisposedMemoryOwner && old is not null)
        {
            old.Dispose();
        }
    }

    public void Dispose()
    {
        CompleteCore();
    }

    public override void Complete(Exception? exception = null)
    {
        _pipeReader.Complete(exception);
        CompleteCore();
    }

    public override async ValueTask CompleteAsync(Exception? exception = null)
    {
        await _pipeReader.CompleteAsync(exception).ConfigureAwait(false);
        CompleteCore();
    }

    public override void CancelPendingRead()
    {
        _pipeReader.CancelPendingRead();
    }


    public async ValueTask<NewInputPacket> ReadPacketAsync(CancellationToken token = default)
    {
        ThrowIfDisposed();
        while (true)
        {
            token.ThrowIfCancellationRequested();
            if (_positionState.HasValue)
            {
                _pipeReader.AdvanceTo(
                    _positionState.Value.Consumed,
                    _positionState.Value.Examined);
                _positionState = null;
            }

            var result = await _pipeReader.ReadAsync(token).ConfigureAwait(false);

            var buffer = result.Buffer;

            if (TryReadPacket(ref buffer, out var packet))
            {
                _positionState = new PositionState
                {
                    Consumed = buffer.Start,
                    Examined = buffer.End
                };
                MemoryOwner<byte> decompressed = default;
                var createdPacket = CreatePacket(packet, ref decompressed);
                SetDecompressedBuffer(ref decompressed);
                return createdPacket;
            }

            _pipeReader.AdvanceTo(buffer.Start, buffer.End);
            if (result.IsCompleted)
            {
                CompleteCore();
                throw new InvalidOperationException("PipeReader is completed");
            }

            if (result.IsCanceled)
            {
                throw new OperationCanceledException("ReadAsync.Result is canceled");
            }
        }
    }

    public IAsyncEnumerable<NewInputPacket> ReadPacketsAsync(
        CancellationToken cancellationToken = default)
    {
        if (_positionState.HasValue)
        {
            _pipeReader.AdvanceTo(
                _positionState.Value.Consumed,
                _positionState.Value.Examined);
            _positionState = null;
        }

        ThrowIfDisposed();
        return ReadPacketsAsyncCore(cancellationToken);
    }

    private async IAsyncEnumerable<NewInputPacket> ReadPacketsAsyncCore(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        MemoryOwner<byte> decompressedBuffer = default;
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            while (true)
            {
                var result = await _pipeReader.ReadAsync(cancellationToken).ConfigureAwait(false);

                

                var buffer = result.Buffer;
                try
                {
                    
                    while (TryReadPacket(
                               ref buffer,
                               out var packet))
                    {
                        yield return CreatePacket(packet, ref decompressedBuffer);
                        if (this.CompressionThreshold > -1)
                        {
                            //Debugger.Break();
                        }
                    }


                    if (result.IsCompleted)
                    {
                        CompleteCore();
                        break;
                    }

                    if (result.IsCanceled)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        break;
                    }
                }
                finally
                {
                    _pipeReader.AdvanceTo(buffer.Start, buffer.End);
                }
            }
        }
        finally
        {
            _sourceCore.Reset();
            decompressedBuffer.Dispose();
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryReadPacket(
        ref ReadOnlySequence<byte> buffer,
        out ReadOnlySequence<byte> packet)
    {
        packet = ReadOnlySequence<byte>.Empty;

        if (buffer.IsEmpty)
            return false; // Not enough data to read packet header

        if (!buffer.TryReadVarInt(out var length, out var headerLen))
            return false; // Unable to read packet length


        if ((long)length + headerLen > buffer.Length)
            return false;

        SequencePosition packetStart = buffer.GetPosition(headerLen);
        SequencePosition packetEnd = buffer.GetPosition(length, packetStart);

        packet = buffer.Slice(packetStart, packetEnd);
        buffer = buffer.Slice(packetEnd);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private NewInputPacket CreatePacket(
        ReadOnlySequence<byte> data,
        ref MemoryOwner<byte> decompressedBuffer)
    {
        bool isSingle1 = data.IsSingleSegment;
        if (CompressionThreshold < 0)
        {
            if (data.TryReadVarInt(out var id, out int idLen))
            {
                data = data.Slice(idLen);
                _sourceCore.Reset();
                _sourceCore.Id = id;
                _sourceCore.Data = data;
                return new NewInputPacket(_sourceCore, _sourceCore.Version);
            }

            throw new InvalidOperationException("Unable to read packet ID");
        }

        var isSingle2 = data.IsSingleSegment;
        if (data.TryReadVarInt(out var sizeUncompressed, out var len))
        {
            data = data.Slice(len);
            if (sizeUncompressed == 0)
            {
                if (data.TryReadVarInt(out var id, out int idLen))
                {
                    data = data.Slice(idLen);
                    _sourceCore.Reset();
                    _sourceCore.Id = id;
                    _sourceCore.Data = data;
                    return new NewInputPacket(_sourceCore, _sourceCore.Version);
                }

                throw new InvalidOperationException("Unable to read packet ID");
            }


            if (!decompressedBuffer.TryResize(sizeUncompressed))
            {
                decompressedBuffer.Dispose();
                decompressedBuffer =
                    new(ArrayPool<byte>.Shared, sizeUncompressed);
            }

            try
            {
                var test = data.Decompress(sizeUncompressed);
                
                data.Decompress(ref decompressedBuffer);
                data = new ReadOnlySequence<byte>(decompressedBuffer.Memory);

                if (data.TryReadVarInt(out var id, out int idLen))
                {
                    data = data.Slice(idLen);
                    _sourceCore.Reset();
                    _sourceCore.Id = id;
                    _sourceCore.Data = data;

                    return new NewInputPacket(_sourceCore, _sourceCore.Version);
                }

                throw new InvalidOperationException("Unable to read packet ID");
            }
            catch
            {
                decompressedBuffer.Dispose();
                throw;
            }
        }

        throw new InvalidOperationException("Unable to read uncompressed size");
    }


    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_state == Disposed, this.GetType());
    }


    private sealed class NullOwner : IMemoryOwner<byte>
    {
        public void Dispose()
        {
        }

        public Memory<byte> Memory => Memory<byte>.Empty;
    }

    private struct PositionState
    {
        public SequencePosition Consumed;
        public SequencePosition Examined;
    }
}