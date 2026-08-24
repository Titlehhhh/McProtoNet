using System.Net;

namespace McProtoNet;

/// <summary>
/// Represents one "open to LAN" announcement received from a Minecraft world.
/// </summary>
/// <param name="Source">The address and source port the announcement came from.</param>
/// <param name="Motd">The text between <c>[MOTD]</c> and <c>[/MOTD]</c>, still containing section-sign
/// color codes.</param>
/// <param name="Port">The TCP port the world listens on.</param>
/// <remarks>
/// The announcement carries only the port. The address is that of the sender, so the endpoint to connect
/// to is <see cref="EndPoint"/>.
/// </remarks>
public readonly record struct LanServer(IPEndPoint Source, string Motd, int Port)
{
    /// <summary>
    /// Gets the endpoint the announced world listens on, built from the sender's address and the announced
    /// port.
    /// </summary>
    public IPEndPoint EndPoint => new(Source.Address, Port);
}
