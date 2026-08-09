namespace McProtoNet.Protocol;

public enum PacketPhase : byte
{
    Handshaking,
    Status,
    Login,
    Configuration,
    Play
}

public enum PacketDirection : byte
{
    Clientbound,
    Serverbound
}

/// <summary>
///     Value identity of a packet type. <paramref name="Key" /> is the manifest key
///     (`login.toServer.login_start`); <paramref name="Ordinal" /> is the dense index of the
///     packet inside its (phase, direction) catalog, ordered by Key — stable across builds,
///     the currency of the hot path (dispatch jump tables, subscription slots).
/// </summary>
public readonly record struct PacketIdentity(
    string Key,
    string Name,
    PacketPhase Phase,
    PacketDirection Direction,
    ushort Ordinal);
