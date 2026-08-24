using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
namespace McProtoNet.Protocol;

/// <summary>
/// Represents a block position that the protocol encodes as a single 64-bit integer.
/// </summary>
/// <param name="X">The X coordinate of the block.</param>
/// <param name="Y">The Y coordinate of the block.</param>
/// <param name="Z">The Z coordinate of the block.</param>
/// <remarks>
/// The encoded form holds X in the upper 26 bits, Z in the next 26 bits and Y in the lowest 12 bits,
/// each as a signed value. Coordinates outside those ranges are truncated on write.
/// </remarks>
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public readonly partial record struct Position(int X, int Y, int Z) : IProtocolType<Position>
{
    /// <summary>
    /// Reads a <see cref="Position"/> from the specified reader.
    /// </summary>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="protocolVersion">The protocol version of the connection.</param>
    /// <returns>The position that was read.</returns>
    /// <exception cref="ProtocolNotSupportException"><paramref name="protocolVersion"/> does not
    /// support this type.</exception>
    public static Position Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Position>(protocolVersion);
        var encoded = reader.ReadSignedLong();
        var x = (int)(encoded >> 38);
        var y = (int)(encoded << 52 >> 52);
        var z = (int)(encoded << 26 >> 38);
        return new Position(x, y, z);
    }

    /// <summary>
    /// Writes the current position to the specified writer.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="protocolVersion">The protocol version of the connection.</param>
    /// <exception cref="ProtocolNotSupportException"><paramref name="protocolVersion"/> does not
    /// support this type.</exception>
    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Position>(protocolVersion);
        var encoded = ((long)(X & 0x3FFFFFF) << 38) |
                      ((long)(Z & 0x3FFFFFF) << 12) |
                      (long)(Y & 0xFFF);
        writer.WriteSignedLong(encoded);
    }
}
