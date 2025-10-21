using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using McProtoNet.Internal;
using McProtoNet.Net.Zlib;
using McProtoNet.Serialization;
using Org.BouncyCastle.Crypto;

namespace McProtoNet.Net;

internal sealed class MinecraftPacketPipeReader : IDisposable, IAsyncDisposable
{
    private static readonly NullOwner DisposedMemoryOwner = new();

    private readonly DecryptedPipeReader _pipeReader;

    // For single packet
    private SequencePosition? _position;


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


    public async ValueTask<NewInputPacket> ReadPacketAsync(CancellationToken token)
    {
        ThrowIfDisposed();
        while (true)
        {
            token.ThrowIfCancellationRequested();
            var result = await _pipeReader.ReadAsync(token);

            ReadOnlySequence<byte> sequence = result.Buffer;

            if (TryReadPacket(ref sequence, out var packet))
            {
                if (_position.HasValue)
                {
                    _pipeReader.AdvanceTo(_position.Value);
                    _position = null;
                }

                _position = sequence.Start;
                return CreatePacket(packet);
            }

            _pipeReader.AdvanceTo(sequence.Start, sequence.End);
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
        ThrowIfDisposed();
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _pipeReader.ReadAsync(cancellationToken).ConfigureAwait(false);


            var buffer = result.Buffer;
            
            try
            {
                while (TryReadPacket(ref buffer, out var packet))
                {
                    yield return CreatePacket(packet);

                    if (_position.HasValue)
                    {
                        _pipeReader.AdvanceTo(_position.Value);
                        _position = null;
                    }
                }
                
                if (result.IsCompleted)
                {
                    throw new InvalidOperationException("PipeReader is completed");
                }

                if (result.IsCanceled)
                {
                    throw new OperationCanceledException("ReadAsync.Result is canceled");
                }
            }
            finally
            {
                _pipeReader.AdvanceTo(buffer.Start, buffer.End);
            }
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryReadPacket(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> packet)
    {
        scoped SequenceReader<byte> reader = new(buffer);


        packet = ReadOnlySequence<byte>.Empty;

        if (buffer.Length < 1) return false; // Not enough data to read packet header

        if (!reader.TryReadVarInt(out var length, out _)) return false; // Unable to read packet length

        if (length > reader.Remaining) return false; // Not enough data to read full packet


        packet = reader.UnreadSequence.Slice(0, length);

        reader.Advance(length);


        buffer = buffer.Slice(reader.Position);

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