namespace McProtoNet.Protocol;

public enum DecodeError : byte
{
    None,

    /// <summary>The type does not live on this protocol version.</summary>
    UnsupportedVersion,

    /// <summary>Remaining != 0 after Read: a spec defect signal, not a near-success.</summary>
    TrailingBytes,

    /// <summary>Underflow or broken data; details in the exception.</summary>
    Malformed
}
