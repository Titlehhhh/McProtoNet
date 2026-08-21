using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
using McProtoNet.Transport.Cryptography;
using McProtoNet.Transport.Pipelines;
namespace McProtoNet.Benchmark.Pipelines;

public enum CryptoReaderKind
{
    BareReader,
    WrapperNoCipher,
    WrapperEncrypted
}

public enum ChunkProfile
{
    Tiny,
    Large
}

public enum ConsumePattern
{
    All,
    Frames,
    Churn
}

public enum SourceKind
{
    Sync,
    Pipe
}

public class CryptoPipeReaderBenchConfig : ManualConfig
{
    public CryptoPipeReaderBenchConfig()
    {
        AddJob(Job.ShortRun
            .WithToolchain(InProcessNoEmitToolchain.Instance)
            .WithWarmupCount(3)
            .WithIterationCount(10));
    }
}

[Config(typeof(CryptoPipeReaderBenchConfig))]
[MemoryDiagnoser]
public class CryptoPipeReaderBenchmarks
{
    private const int FrameSize = 100;

    private static readonly byte[] Secret = "0123456789ABCDEF"u8.ToArray();

    [Params(2 * 1024 * 1024)] public int TotalBytes;

    [Params(CryptoReaderKind.BareReader, CryptoReaderKind.WrapperNoCipher, CryptoReaderKind.WrapperEncrypted)]
    public CryptoReaderKind Kind;

    [Params(ChunkProfile.Tiny, ChunkProfile.Large)] public ChunkProfile Chunk;

    [Params(ConsumePattern.All, ConsumePattern.Frames, ConsumePattern.Churn)]
    public ConsumePattern Consume;

    [Params(SourceKind.Sync, SourceKind.Pipe)] public SourceKind Source;

    private byte[] _plain;
    private byte[] _encrypted;
    private int[] _chunkPlan;

    private PipeReader _reader;
    private Task _producer;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _plain = new byte[TotalBytes];
        new Random(40).NextBytes(_plain);

        _encrypted = (byte[])_plain.Clone();
        using (PacketCipher encryptor = PacketCipher.CreateEncryptor(Secret))
        {
            encryptor.Transform(_encrypted);
        }

        _chunkPlan = BuildChunkPlan(TotalBytes, Chunk);
    }

    private static int[] BuildChunkPlan(int total, ChunkProfile profile)
    {
        var random = new Random(41);
        var plan = new List<int>();
        int remaining = total;
        while (remaining > 0)
        {
            int size = profile == ChunkProfile.Tiny
                ? random.Next(1, 65)
                : random.Next(4 * 1024, 64 * 1024 + 1);
            size = Math.Min(size, remaining);
            plan.Add(size);
            remaining -= size;
        }

        return plan.ToArray();
    }

    [IterationSetup]
    public void IterationSetup()
    {
        byte[] data = Kind == CryptoReaderKind.WrapperEncrypted ? _encrypted : _plain;

        PipeReader inner;
        if (Source == SourceKind.Sync)
        {
            inner = new ChunkedSourceReader(data, _chunkPlan);
            _producer = Task.CompletedTask;
        }
        else
        {
            var pipe = new Pipe(new PipeOptions(
                pauseWriterThreshold: 256 * 1024,
                resumeWriterThreshold: 128 * 1024,
                useSynchronizationContext: false));
            inner = pipe.Reader;
            _producer = Task.Run(() => ProduceAsync(pipe.Writer, data, _chunkPlan));
        }

        switch (Kind)
        {
            case CryptoReaderKind.BareReader:
                _reader = inner;
                break;
            case CryptoReaderKind.WrapperNoCipher:
                _reader = new CryptoPipeReader(inner);
                break;
            case CryptoReaderKind.WrapperEncrypted:
                var encrypted = new CryptoPipeReader(inner);
                encrypted.EnableEncryption(Secret);
                _reader = encrypted;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static async Task ProduceAsync(PipeWriter writer, byte[] data, int[] plan)
    {
        int position = 0;
        foreach (int size in plan)
        {
            writer.Write(data.AsSpan(position, size));
            position += size;
            await writer.FlushAsync().ConfigureAwait(false);
        }

        await writer.CompleteAsync().ConfigureAwait(false);
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
        _reader.Complete();
        (_reader as IDisposable)?.Dispose();
        _reader = null;
        _producer.GetAwaiter().GetResult();
        _producer = null;
    }

    [Benchmark]
    public async Task<long> ReadStream()
    {
        PipeReader reader = _reader;
        long total = 0;
        while (true)
        {
            ReadResult result = await reader.ReadAsync().ConfigureAwait(false);
            ReadOnlySequence<byte> buffer = result.Buffer;

            SequencePosition consumed;
            SequencePosition examined;
            switch (Consume)
            {
                case ConsumePattern.All:
                    consumed = buffer.End;
                    examined = buffer.End;
                    total += buffer.Length;
                    break;
                case ConsumePattern.Frames:
                {
                    long frames = buffer.Length - buffer.Length % FrameSize;
                    consumed = buffer.GetPosition(frames);
                    examined = buffer.End;
                    total += frames;
                    break;
                }
                case ConsumePattern.Churn:
                {
                    if (buffer.Length >= FrameSize)
                    {
                        consumed = buffer.GetPosition(FrameSize);
                        examined = consumed;
                        total += FrameSize;
                    }
                    else
                    {
                        consumed = buffer.Start;
                        examined = buffer.End;
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            reader.AdvanceTo(consumed, examined);

            if (result.IsCompleted && (Consume == ConsumePattern.All || buffer.Length < FrameSize))
            {
                break;
            }
        }

        return total;
    }

    private sealed class ChunkedSourceReader : PipeReader
    {
        private readonly byte[] _data;
        private readonly int[] _plan;
        private int _planIndex;
        private int _consumed;
        private int _examined;
        private int _served;

        public ChunkedSourceReader(byte[] data, int[] plan)
        {
            _data = data;
            _plan = plan;
        }

        public override bool TryRead(out ReadResult result)
        {
            result = Next();
            return true;
        }

        public override ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken = default)
        {
            return new ValueTask<ReadResult>(Next());
        }

        private ReadResult Next()
        {
            if (_examined >= _served && _planIndex < _plan.Length)
            {
                _served += _plan[_planIndex++];
            }

            var buffer = new ReadOnlySequence<byte>(_data, _consumed, _served - _consumed);
            return new ReadResult(buffer, isCanceled: false, isCompleted: _served == _data.Length);
        }

        public override void AdvanceTo(SequencePosition consumed)
        {
            AdvanceTo(consumed, consumed);
        }

        public override void AdvanceTo(SequencePosition consumed, SequencePosition examined)
        {
            _consumed = consumed.GetInteger();
            _examined = examined.GetInteger();
        }

        public override void CancelPendingRead()
        {
        }

        public override void Complete(Exception exception = null)
        {
        }
    }
}
