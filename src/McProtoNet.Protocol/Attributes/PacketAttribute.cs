namespace McProtoNet.Protocol.Attributes;

/// <summary>
///     Declarative identity for third-party Roslyn source generators (they see referenced
///     assemblies only through symbol metadata: attributes yes, method bodies no).
///     Printed by the F# codegen from the same spec as the tables — cannot drift.
///     The runtime never reads it.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public sealed class PacketAttribute : Attribute
{
    public PacketAttribute(string key, PacketPhase phase, PacketDirection direction)
    {
        Key = key;
        Phase = phase;
        Direction = direction;
    }

    public string Key { get; }
    public PacketPhase Phase { get; }
    public PacketDirection Direction { get; }
}
