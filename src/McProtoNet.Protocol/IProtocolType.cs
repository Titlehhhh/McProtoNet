using McProtoNet.Primitives;

namespace McProtoNet.Protocol;

/// <summary>
/// Defines the read and write operations of a type that appears in the Minecraft protocol.
/// </summary>
/// <typeparam name="TSelf">The type that implements the interface.</typeparam>
public interface IProtocolType<TSelf> where TSelf : IProtocolType<TSelf>
{
    /// <summary>
    /// Reads a value of type <typeparamref name="TSelf"/> from the specified reader.
    /// </summary>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="protocolVersion">The protocol version of the connection.</param>
    /// <returns>The value that was read.</returns>
    static abstract TSelf Read(ref MinecraftPrimitiveReader reader, int protocolVersion);

    /// <summary>
    /// Writes the current value to the specified writer.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="protocolVersion">The protocol version of the connection.</param>
    void Write(MinecraftPrimitiveWriter writer, int protocolVersion);

    // The JSON model view is parked pending a redesign of the public JSON interface (owner, 2026-10-22);
    // it comes back with the `emitJson` flag in minecraft-protocol-fs/minecraft-protocol-fs/Codegen/CSharp/Json.fs.
    // /// <summary>
    // /// Writes the current value as JSON: the decoded model, not the wire layout.
    // /// </summary>
    // /// <param name="writer">The JSON writer to write to.</param>
    // void WriteJson(Utf8JsonWriter writer);
}
