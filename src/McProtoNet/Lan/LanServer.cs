using System.Net;

namespace McProtoNet;

/// <summary>
///     One "open to LAN" announcement. The world announces its port; the address is whoever sent the
///     datagram, so the server to connect to is <see cref="EndPoint" />.
/// </summary>
/// <param name="Source">Address and source port the announcement came from.</param>
/// <param name="Motd">Text between <c>[MOTD]</c> and <c>[/MOTD]</c>, still in section-sign colour codes.</param>
/// <param name="Port">TCP port the world listens on.</param>
public readonly record struct LanServer(IPEndPoint Source, string Motd, int Port)
{
    /// <summary>Where the announced world actually listens: the sender's address and the announced port.</summary>
    public IPEndPoint EndPoint => new(Source.Address, Port);
}
