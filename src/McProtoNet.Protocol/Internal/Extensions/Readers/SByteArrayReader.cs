using System.Runtime.InteropServices;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public readonly struct SByteArrayReader : IArrayReader<sbyte>
{
    public static void Read(ref MinecraftPrimitiveReader reader, int protocolVersion, Span<sbyte> destination)
    {
        ReadOnlySpan<byte> buff = reader.Read(destination.Length);
        ReadOnlySpan<sbyte> casted = MemoryMarshal.Cast<byte, sbyte>(buff);
        casted.CopyTo(destination);
    }
}