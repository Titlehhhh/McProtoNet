namespace McProtoNet.Protocol;

/// <summary>
/// Specifies the connection phase a packet belongs to.
/// </summary>
public enum PacketPhase : byte
{
    /// <summary>The handshaking phase.</summary>
    Handshaking,

    /// <summary>The status phase.</summary>
    Status,

    /// <summary>The login phase.</summary>
    Login,

    /// <summary>The configuration phase.</summary>
    Configuration,

    /// <summary>The play phase.</summary>
    Play
}
