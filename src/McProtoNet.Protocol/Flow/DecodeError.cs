namespace McProtoNet.Protocol;

/// <summary>
/// Specifies the reason a packet could not be decoded.
/// </summary>
public enum DecodeError : byte
{
    /// <summary>The packet was decoded successfully.</summary>
    None,

    /// <summary>The packet type does not exist on the requested protocol version.</summary>
    UnsupportedVersion,

    /// <summary>Bytes remained in the payload after the packet was read.</summary>
    TrailingBytes,

    /// <summary>The payload ended early or held invalid data.</summary>
    Malformed
}
