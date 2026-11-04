namespace McProtoNet.Protocol;

/// <summary>
/// How the element count of a length-prefixed array is encoded on the wire.
/// </summary>
public enum LengthFormat
{
    /// <summary>A VarInt.</summary>
    VarInt,

    /// <summary>A fixed 4-byte big-endian signed integer.</summary>
    Int32
}
