using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using McProtoNet.Internal;
using McProtoNet.Net.Zlib;
using McProtoNet.Serialization;
using Org.BouncyCastle.Crypto;

namespace McProtoNet.Net;

internal struct PositionState
{
    public SequencePosition Consumed;
    public SequencePosition Examined;
}

internal sealed class MinecraftPacketPipeReader : IDisposable, IAsyncDisposable
{
    private static readonly NullOwner DisposedMemoryOwner = new();

    private readonly DecryptedPipeReader _pipeReader;

    // For single packet
    private PositionState? _positionState;


    private const int None = 0;
    private const int Disposed = 1;

    private volatile int _state = 0;

    private int _compressionThreshold = -1;

    private volatile IMemoryOwner<byte>? _desompressedBuffer;

    private readonly PacketSourceCore _sourceCore = new();

    public MinecraftPacketPipeReader(PipeReader pipeReader)
    {
        this._pipeReader = new DecryptedPipeReader(pipeReader);
    }

    public int CompressionThreshold { get; set; }

    public bool EncryptionEnabled => _pipeReader.IsEncrypted;

    public void EnableEncryption(IBufferedCipher decryptor)
    {
        _pipeReader.SwitchEncryption(decryptor);
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
            var result = await _pipeReader.ReadAsync(token);

            var buffer  = result.Buffer;

            if (TryReadPacket(ref buffer, out var packet))
            {
                _positionState = new PositionState()
                {
                    Consumed = buffer.Start,
                    Examined = buffer.End
                };
                return CreatePacket(packet);
            }

            _pipeReader.AdvanceTo(buffer.Start, buffer.End);
            if (result.IsCompleted)
            {
                throw new InvalidOperationException("PipeReader is completed");
            }

            if (result.IsCanceled)
            {
                throw new OperationCanceledException("ReadAsync.Result is canceled");
            }
        }
    }

    public async IAsyncEnumerable<NewInputPacket> ReadPacketsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (_positionState.HasValue)
        {
            _pipeReader.AdvanceTo(
                _positionState.Value.Consumed,
                _positionState.Value.Examined);
            _positionState = null;
        }

        ThrowIfDisposed();
        try
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var result = await _pipeReader.ReadAsync(cancellationToken).ConfigureAwait(false);


                var buffer = result.Buffer;
                try
                {
                    while (TryReadPacket(
                               ref buffer,
                               out var packet))
                    {
                        yield return CreatePacket(packet);
                    }

                    if (result.IsCompleted)
                    {
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
            var old = 
                Interlocked.Exchange(ref _desompressedBuffer, null);

            old?.Dispose();
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
    private NewInputPacket CreatePacket(ReadOnlySequence<byte> data)
    {
        if (_compressionThreshold == -1)
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

            var owner = data.Decompress(sizeUncompressed);
            data = new ReadOnlySequence<byte>(owner.Memory);

            try
            {
                if (data.TryReadVarInt(out var id, out int idLen))
                {
                    data = data.Slice(idLen);
                    _sourceCore.Reset();
                    _sourceCore.Id = id;
                    _sourceCore.Data = data;

                    var old = Interlocked.Exchange(ref _desompressedBuffer, owner);


                    ObjectDisposedException.ThrowIf(ReferenceEquals(old, DisposedMemoryOwner), GetType());


                    old?.Dispose();

                    return new NewInputPacket(_sourceCore, _sourceCore.Version);
                }

                owner.Dispose();
                throw new InvalidOperationException("Unable to read packet ID");
            }
            catch
            {
                owner.Dispose();
                throw;
            }
        }

        throw new InvalidOperationException("Unable to read uncompressed size");
    }

    public void Complete(Exception? exception = null)
    {
        ThrowIfDisposed();
        _pipeReader.Complete(exception);
    }

    public ValueTask CompleteAsync(Exception? exception = null)
    {
        ThrowIfDisposed();
        return _pipeReader.CompleteAsync(exception);
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_state == Disposed, this.GetType());
    }

    public void Dispose()
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

    public ValueTask DisposeAsync()
    {
        if (Interlocked.CompareExchange(ref _state, Disposed, None) == Disposed)
        {
            return ValueTask.CompletedTask;
        }

        var old = Interlocked.Exchange(ref _desompressedBuffer, DisposedMemoryOwner);
        if (old != DisposedMemoryOwner && old is not null)
        {
            old.Dispose();
        }

        return ValueTask.CompletedTask;
    }


    internal sealed class NullOwner : IMemoryOwner<byte>
    {
        public void Dispose()
        {
        }

        public Memory<byte> Memory => Memory<byte>.Empty;
    }
}