using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
using McProtoNet.Net;

namespace McProtoNet.Benchmark.Pipelines;

public enum EncWriterKind
{
    BarePipeWriter,
    WrapperNoCipher,
    WrapperEncrypted
}

public class CryptoPipeWriterBenchConfig : ManualConfig
{
    public CryptoPipeWriterBenchConfig()
    {
        AddJob(Job.ShortRun.WithToolchain(InProcessNoEmitToolchain.Instance));
    }
}

[Config(typeof(CryptoPipeWriterBenchConfig))]
[MemoryDiagnoser]
public class CryptoPipeWriterBenchmarks
{
    private static readonly byte[] Secret = "0123456789ABCDEF"u8.ToArray();

    [Params(100_000)] public int PacketsCount;
    [Params(50, 512)] public int PacketSize;

    [Params(EncWriterKind.BarePipeWriter, EncWriterKind.WrapperNoCipher, EncWriterKind.WrapperEncrypted)]
    public EncWriterKind Kind;

    private byte[] _packet;
    private string _path;
    private FileStream _file;
    private PipeWriter _writer;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _packet = new byte[PacketSize];
        new Random(40).NextBytes(_packet);
        _path = Path.Combine(Path.GetTempPath(), $"mcprotonet-encwriter-{PacketSize}.bench");
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _file = new FileStream(_path, FileMode.Create, FileAccess.Write, FileShare.None, 1);
        PipeWriter inner = PipeWriter.Create(_file);

        switch (Kind)
        {
            case EncWriterKind.BarePipeWriter:
                _writer = inner;
                break;
            case EncWriterKind.WrapperNoCipher:
                _writer = new CryptoPipeWriter(inner);
                break;
            case EncWriterKind.WrapperEncrypted:
                var encrypted = new CryptoPipeWriter(inner);
                encrypted.EnableEncryption(Secret);
                _writer = encrypted;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
        _writer.Complete();
        (_writer as IDisposable)?.Dispose();
        _writer = null;
        _file.Dispose();
        _file = null;
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        File.Delete(_path);
    }

    [Benchmark]
    public async Task WritePackets()
    {
        PipeWriter writer = _writer;
        for (int i = 0; i < PacketsCount; i++)
        {
            writer.WritePacket(_packet.AsSpan());
            await writer.FlushAsync();
        }
    }
}

[Config(typeof(CryptoPipeWriterBenchConfig))]
[MemoryDiagnoser]
public class CryptoPipeWriterOpsBenchmarks
{
    private const int OpsPerInvoke = 256;
    private static readonly byte[] Secret = "0123456789ABCDEF"u8.ToArray();

    [Params(1, 64, 8192, 65536)] public int Size;

    [Params(EncWriterKind.BarePipeWriter, EncWriterKind.WrapperNoCipher, EncWriterKind.WrapperEncrypted)]
    public EncWriterKind Kind;

    private byte[] _payload;
    private Pipe _pipe;
    private PipeWriter _writer;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _payload = new byte[Size];
        new Random(40).NextBytes(_payload);
        _pipe = new Pipe(new PipeOptions(pauseWriterThreshold: 0, useSynchronizationContext: false));
        _writer = Wrap(_pipe.Writer);
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        _writer.Complete();
        (_writer as IDisposable)?.Dispose();
        _pipe.Reader.Complete();
    }

    [Benchmark(OperationsPerInvoke = OpsPerInvoke)]
    public async Task WriteAndFlush()
    {
        PipeWriter writer = _writer;
        for (int i = 0; i < OpsPerInvoke; i++)
        {
            _payload.CopyTo(writer.GetSpan(_payload.Length));
            writer.Advance(_payload.Length);
            await writer.FlushAsync();
            Drain();
        }
    }

    [Benchmark(OperationsPerInvoke = OpsPerInvoke)]
    public async Task ByteChurnAndFlush()
    {
        PipeWriter writer = _writer;
        for (int i = 0; i < OpsPerInvoke; i++)
        {
            for (int j = 0; j < _payload.Length; j++)
            {
                writer.GetMemory(1).Span[0] = _payload[j];
                writer.Advance(1);
            }

            await writer.FlushAsync();
            Drain();
        }
    }

    [Benchmark(OperationsPerInvoke = OpsPerInvoke)]
    public void WriteAndComplete()
    {
        for (int i = 0; i < OpsPerInvoke; i++)
        {
            var pipe = new Pipe(new PipeOptions(pauseWriterThreshold: 0, useSynchronizationContext: false));
            PipeWriter writer = Wrap(pipe.Writer);
            _payload.CopyTo(writer.GetSpan(_payload.Length));
            writer.Advance(_payload.Length);
            writer.Complete();
            (writer as IDisposable)?.Dispose();
            if (pipe.Reader.TryRead(out ReadResult result))
            {
                pipe.Reader.AdvanceTo(result.Buffer.End);
            }

            pipe.Reader.Complete();
        }
    }

    private void Drain()
    {
        if (_pipe.Reader.TryRead(out ReadResult result))
        {
            _pipe.Reader.AdvanceTo(result.Buffer.End);
        }
    }

    private PipeWriter Wrap(PipeWriter inner)
    {
        switch (Kind)
        {
            case EncWriterKind.BarePipeWriter:
                return inner;
            case EncWriterKind.WrapperNoCipher:
                return new CryptoPipeWriter(inner);
            case EncWriterKind.WrapperEncrypted:
                var encrypted = new CryptoPipeWriter(inner);
                encrypted.EnableEncryption(Secret);
                return encrypted;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
