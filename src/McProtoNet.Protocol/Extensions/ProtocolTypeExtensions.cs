using McProtoNet.Primitives;

namespace McProtoNet.Protocol;

/// <summary>
/// Provides extension methods that read and write protocol types through
/// <see cref="MinecraftPrimitiveReader"/> and <see cref="MinecraftPrimitiveWriter"/>.
/// </summary>
public static class ProtocolTypeExtensions
{
    /// <summary>
    /// Reads a value of the specified protocol type from the reader.
    /// </summary>
    /// <typeparam name="T">The protocol type to read.</typeparam>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="protocolVersion">The protocol version of the connection.</param>
    /// <returns>The value that was read.</returns>
    public static T ReadType<T>(this ref MinecraftPrimitiveReader reader, int protocolVersion)
        where T : IProtocolType<T>
        => T.Read(ref reader, protocolVersion);

    /// <summary>
    /// Writes a value of the specified protocol type to the writer.
    /// </summary>
    /// <typeparam name="T">The protocol type to write.</typeparam>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="protocolVersion">The protocol version of the connection.</param>
    public static void WriteType<T>(this MinecraftPrimitiveWriter writer, T value, int protocolVersion)
        where T : IProtocolType<T>
        => value.Write(writer, protocolVersion);
}
