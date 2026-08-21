namespace McProtoNet;

/// <summary>
///     One <c>_minecraft._tcp</c> SRV answer: where the server really listens, plus the two numbers
///     RFC 2782 uses to choose between several answers.
/// </summary>
/// <param name="Target">Host name of the server, without the trailing dot.</param>
/// <param name="Port">TCP port of the server.</param>
/// <param name="Priority">Lower wins: only the lowest-priority answers take part in the draw.</param>
/// <param name="Weight">Relative share inside one priority group.</param>
public readonly record struct SrvResult(string Target, ushort Port, ushort Priority, ushort Weight);
