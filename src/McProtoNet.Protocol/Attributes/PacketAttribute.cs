namespace McProtoNet.Protocol.Attributes;

/// <summary>
/// Specifies the identity of the packet that the target type represents.
/// </summary>
/// <remarks>
/// The attribute is emitted by the code generator from the same specification as the packet tables.
/// It exists so that Roslyn source generators in other assemblies can read the packet identity from
/// symbol metadata. The runtime does not read it.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public sealed class PacketAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PacketAttribute"/> class with the specified
    /// manifest key, phase and direction.
    /// </summary>
    /// <param name="key">The manifest key of the packet, such as <c>login.toServer.login_start</c>.</param>
    /// <param name="phase">The connection phase the packet belongs to.</param>
    /// <param name="direction">The direction the packet travels in.</param>
    public PacketAttribute(string key, PacketPhase phase, PacketDirection direction)
    {
        Key = key;
        Phase = phase;
        Direction = direction;
    }

    /// <summary>
    /// Gets the manifest key of the packet.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the connection phase the packet belongs to.
    /// </summary>
    public PacketPhase Phase { get; }

    /// <summary>
    /// Gets the direction the packet travels in.
    /// </summary>
    public PacketDirection Direction { get; }
}
