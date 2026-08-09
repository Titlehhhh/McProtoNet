namespace McProtoNet.Protocol.Attributes;

/// <summary>
///     One api field of a packet, with its version-presence range, for third-party source
///     generators. <see cref="To" /> left at 0 means "open end — alive as of now".
///     Printed by the F# codegen; the runtime never reads it.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class PacketFieldAttribute : Attribute
{
    public PacketFieldAttribute(string name, string typeName)
    {
        Name = name;
        TypeName = typeName;
    }

    public string Name { get; }
    public string TypeName { get; }

    /// <summary>Version group holding the field; null = common (lives in every layer).</summary>
    public string? Group { get; init; }

    public int From { get; init; }
    public int To { get; init; }
}
