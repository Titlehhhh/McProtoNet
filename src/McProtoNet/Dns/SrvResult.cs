namespace McProtoNet;

/// <summary>
/// Represents one <c>_minecraft._tcp</c> SRV record.
/// </summary>
/// <param name="Target">The host name of the server, without the trailing dot.</param>
/// <param name="Port">The TCP port of the server.</param>
/// <param name="Priority">The RFC 2782 priority. Only the records with the lowest value take part in the
/// draw.</param>
/// <param name="Weight">The RFC 2782 weight, which is the relative share inside one priority group.</param>
public readonly record struct SrvResult(string Target, ushort Port, ushort Priority, ushort Weight);
