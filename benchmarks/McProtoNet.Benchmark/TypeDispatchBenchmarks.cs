using System;
using BenchmarkDotNet.Attributes;
using McProtoNet.Protocol;
using McProtoNet.Protocol.Extensions;
using McProtoNet.Serialization;

namespace McProtoNet.Benchmark;

/// <summary>
/// Measures overhead of WriteType&lt;T&gt; dispatch chain vs direct calls.
///
/// Hypothesis: JIT inlines WriteType&lt;T&gt; at callsite and eliminates dead branches
/// → Direct and Dispatch should be ~equal.
///
/// If NOT inlined → Dispatch will be slower (linear scan through ~50 if-checks).
/// </summary>
[DisassemblyDiagnoser(maxDepth: 3)]
[MemoryDiagnoser]
public class TypeDispatchBenchmarks
{
    private MinecraftPrimitiveWriter _writer = null!;
    private const int ProtocolVersion = 770;

    // Types near the START of the dispatch chain
    private Position _position = new Position(100, 64, -200);
    private Vec3f _vec3f = new Vec3f(1.0f, 2.0f, 3.0f);

    // Type near the END of the dispatch chain
    private MovementFlags _movementFlags = new MovementFlags(true, false);

    [GlobalSetup]
    public void Setup()
    {
        _writer = new MinecraftPrimitiveWriter();
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _writer.Reset();
    }

    // ── Direct calls (baseline) ──────────────────────────────────────────────

    [Benchmark(Baseline = true)]
    public void Direct_Position()
        => _writer.WritePosition(_position, ProtocolVersion);

    [Benchmark]
    public void Direct_Vec3f()
        => _writer.WriteVec3f(_vec3f, ProtocolVersion);

    [Benchmark]
    public void Direct_MovementFlags()
        => _writer.WriteMovementFlags(_movementFlags, ProtocolVersion);

    // ── Via WriteType<T> dispatch ────────────────────────────────────────────

    [Benchmark]
    public void Dispatch_Position()
        => _writer.WriteType<Position>(_position, ProtocolVersion);

    [Benchmark]
    public void Dispatch_Vec3f()
        => _writer.WriteType<Vec3f>(_vec3f, ProtocolVersion);

    [Benchmark]
    public void Dispatch_MovementFlags()
        => _writer.WriteType<MovementFlags>(_movementFlags, ProtocolVersion);
}
