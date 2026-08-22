using System.Net;
using QuickProxyNet;

namespace McProtoNet;

/// <summary>Connection options for <see cref="MinecraftClient" />.</summary>
public sealed class MinecraftClientOptions
{
    /// <summary>The port a Minecraft server listens on unless it says otherwise.</summary>
    public const int DefaultPort = 25565;

    /// <summary>Host name or IP literal the user typed.</summary>
    public required string Host { get; init; }

    /// <summary>Port the user typed. Leaving it at <see cref="DefaultPort" /> is what allows the SRV lookup.</summary>
    public int Port { get; init; } = DefaultPort;

    public TimeSpan ConnectTimeout { get; init; } = TimeSpan.FromSeconds(30);

    /// <summary>
    ///     Look up <c>_minecraft._tcp.&lt;host&gt;</c> before connecting, as vanilla does. The lookup only
    ///     happens when the port was left at <see cref="DefaultPort" /> and the host is not an IP literal.
    /// </summary>
    public bool UseSrv { get; init; } = true;

    /// <summary>
    ///     How long the SRV lookup may take before the connect goes ahead with the host as typed. Capped by
    ///     <see cref="ConnectTimeout" />, so a slow resolver can never eat the whole connect budget. It can
    ///     only shorten the lookup: the resolver carries a budget of its own, so a value above five seconds
    ///     changes nothing.
    /// </summary>
    public TimeSpan SrvTimeout { get; init; } = TimeSpan.FromSeconds(5);

    /// <summary>
    ///     Route the connection through a proxy instead of opening a socket to the server. The client
    ///     asks it for a stream to the resolved host and port, and owns that stream from then on.
    ///     Build one with <c>ProxyClientFactory.Instance.Create("socks5://user:pass@127.0.0.1:1080")</c>.
    ///     Null connects straight over TCP.
    /// </summary>
    /// <remarks>
    ///     <see cref="NoDelay" /> does not reach the proxy socket — the proxy client carries its own
    ///     <c>NoDelay</c>, and this client does not touch settings on an object the caller owns.
    /// </remarks>
    public IProxyClient? Proxy { get; init; }

    /// <summary>
    ///     Bind the outgoing socket to this local address before connecting — the way to pick which
    ///     network interface or source address the connection leaves from. Port 0 lets the OS choose
    ///     an ephemeral port. The address family must match the server's: an IPv4 local endpoint
    ///     cannot reach an IPv6-only host. Null keeps the default, unbound socket.
    /// </summary>
    /// <remarks>
    ///     This applies to the direct TCP path only. When <see cref="Proxy" /> is set the proxy client
    ///     owns its socket, and it carries a <c>LocalEndPoint</c> of its own to set instead.
    /// </remarks>
    public IPEndPoint? LocalEndPoint { get; init; }

    /// <summary>
    ///     Send small frames as they come instead of waiting for a full segment. On by default: a
    ///     Minecraft client writes small packets and wants each one out now.
    /// </summary>
    public bool NoDelay { get; init; } = true;
}
