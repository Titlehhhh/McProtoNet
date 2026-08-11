// EXPERIMENT (McProtoNet.Next) — prototype of task 14 in the owner's own framing:
// the writer does not own a buffer, it IS an IBufferWriter<byte> over someone else's.
//
// This is the shape MinecraftPrimitiveWriter would take in src/**. Here it lives beside
// the old code and touches nothing. Two things fall out of the shape, and the demo and
// the measurement path prove both:
//   1. the same writer serves any sink — a pooled staging buffer OR the pipe directly,
//      chosen by whoever calls Reset(sink), not baked into the writer;
//   2. because it is an IBufferWriter<byte>, the primitive extensions already written on
//      IBufferWriter (Serialization/Extensions.WriteVarInt) work on it with no new code.

using System.Buffers;
using System.Text;
using McProtoNet.Serialization; // WriteVarInt(this IBufferWriter<byte>, int)

namespace McProtoNet.Next.Wire;

/// <summary>
///     A protocol-primitive writer that owns no memory. It forwards every write to an
///     external <see cref="IBufferWriter{T}" /> sink, so the bytes land where the sink
///     points — a pooled staging buffer, or a <see cref="System.IO.Pipelines.PipeWriter" />
///     straight onto the wire — with no buffer of its own and no copy to hand the result out.
/// </summary>
/// <remarks>
///     Rebind the sink with <see cref="Reset(IBufferWriter{byte})" /> and the instance is
///     reusable across packets — the shape that finally gives
///     <c>MinecraftPrimitiveWriterCache</c> a reason to exist. There is deliberately no
///     <c>WrittenMemory</c> / <c>GetWrittenMemory</c>: those are the copy the send-path
///     measurement caught, and they cannot exist when the buffer belongs to the sink.
/// </remarks>
public sealed class WireWriter : IBufferWriter<byte>
{
    private IBufferWriter<byte> _sink;

    public WireWriter(IBufferWriter<byte> sink) => _sink = sink;

    /// <summary>Points the writer at a new sink and makes it ready to reuse.</summary>
    public void Reset(IBufferWriter<byte> sink) => _sink = sink;

    // ---- IBufferWriter<byte>: the whole surface, forwarded. ----

    public void Advance(int count) => _sink.Advance(count);

    public Memory<byte> GetMemory(int sizeHint = 0) => _sink.GetMemory(sizeHint);

    public Span<byte> GetSpan(int sizeHint = 0) => _sink.GetSpan(sizeHint);

    // ---- Protocol helpers. VarInt reuses the existing IBufferWriter extension; the rest
    //      are here just so the owner's handshake reads naturally in the demo. ----

    public void WriteBuffer(ReadOnlySpan<byte> value)
    {
        value.CopyTo(GetSpan(value.Length));
        Advance(value.Length);
    }

    public void WriteUnsignedShort(ushort value)
    {
        Span<byte> span = GetSpan(2);
        span[0] = (byte)(value >> 8);
        span[1] = (byte)value;
        Advance(2);
    }

    public void WriteString(string value)
    {
        int byteCount = Encoding.UTF8.GetByteCount(value);
        this.WriteVarInt(byteCount);              // extension on IBufferWriter<byte>
        Span<byte> span = GetSpan(byteCount);
        Encoding.UTF8.GetBytes(value, span);
        Advance(byteCount);
    }
}
