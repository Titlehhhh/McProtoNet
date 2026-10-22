namespace McProtoNet.Protocol.Attributes;

/// <summary>
/// Specifies one field of the packet that the target type represents, together with the protocol
/// versions the field is present on.
/// </summary>
/// <remarks>
/// The attribute is emitted by the code generator so that Roslyn source generators in other
/// assemblies can read the packet layout from symbol metadata. The runtime does not read it.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class PacketFieldAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PacketFieldAttribute"/> class with the specified
    /// field name and type name.
    /// </summary>
    /// <param name="name">The name of the field.</param>
    /// <param name="typeName">The name of the field type.</param>
    public PacketFieldAttribute(string name, string typeName)
    {
        Name = name;
        TypeName = typeName;
    }

    /// <summary>
    /// Gets the name of the field.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the name of the field type.
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    /// Gets the name of the version group that holds the field, or <see langword="null"/> if the
    /// field is common to every layer.
    /// </summary>
    public string? Group { get; init; }

    /// <summary>
    /// Gets the first protocol version the field is present on.
    /// </summary>
    public int From { get; init; }

    /// <summary>
    /// Gets the last protocol version the field is present on.
    /// </summary>
    /// <value>
    /// The last protocol version the field is present on, or 0 if the field is present on every
    /// version from <see cref="From"/> onwards.
    /// </value>
    public int To { get; init; }
}
