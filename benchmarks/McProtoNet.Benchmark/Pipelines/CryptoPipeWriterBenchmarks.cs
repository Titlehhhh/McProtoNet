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
