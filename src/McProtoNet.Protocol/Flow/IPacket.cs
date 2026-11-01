namespace McProtoNet.Protocol;

/// <summary>
/// Defines the identity that a packet reports when it is held by a reference to a non-generic type.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="IPacket{TSelf}"/> declares the identity as a static abstract member, which is reachable
/// only where the concrete packet type is known statically. A decoded packet that has left the
/// dispatcher, such as an element of a packet stream or a value in a switch, no longer carries that
/// type; without this interface its only common type is <see cref="object"/>.
/// </para>
/// <para>
/// Generated packets implement this interface explicitly, so that the instance property and the
/// static property coexist on the same type. <see cref="IPacket{TSelf}"/> does not inherit this
/// interface, because a static member cannot satisfy an instance member.
/// </para>
/// </remarks>
public interface IPacket
{
    /// <summary>
    /// Gets the identity of the packet.
    /// </summary>
    /// <value>The same value as the static identity declared by the concrete packet type.</value>
    PacketIdentity Identity { get; }

    // The JSON model view is parked pending a redesign of the public JSON interface (owner, 2026-10-22);
    // it comes back with the `emitJson` flag in minecraft-protocol-fs/minecraft-protocol-fs/Codegen/CSharp/Json.fs.
    // /// <summary>
    // /// Writes the packet as JSON: the decoded model, not the wire layout. Version layers are
    // /// flattened into the object, so the JSON carries no version-specific members.
    // /// </summary>
    // /// <param name="writer">The JSON writer to write to.</param>
    // void WriteJson(Utf8JsonWriter writer);
}
