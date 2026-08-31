using System.Net;
using QuickProxyNet;

namespace McProtoNet;

/// <summary>
/// Represents the connection options used by <see cref="MinecraftClient"/>.
/// </summary>
public sealed class MinecraftClientOptions
{
    /// <summary>The default port a Minecraft server listens on.</summary>
    public const int DefaultPort = 25565;

    /// <summary>
    /// Gets or sets the host name or IP literal of the server.
    /// </summary>
    public required string Host { get; init; }

    /// <summary>
    /// Gets or sets the TCP port of the server.
    /// </summary>
    /// <value>The port to connect to. The default is <see cref="DefaultPort"/>, which is also the only
    /// value that allows the SRV lookup.</value>
    public int Port { get; init; } = DefaultPort;

    /// <summary>
    /// Gets or sets the maximum time a connect attempt may take.
    /// </summary>
    /// <value>The connect timeout. The default is 30 seconds.</value>
    public TimeSpan ConnectTimeout { get; init; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets a value indicating whether <c>_minecraft._tcp.&lt;host&gt;</c> is looked up before
    /// connecting, as the vanilla client does.
    /// </summary>
    /// <value><see langword="true"/> to perform the SRV lookup; otherwise, <see langword="false"/>. The
    /// default is <see langword="true"/>.</value>
    /// <remarks>
    /// The lookup runs only when <see cref="Port"/> is <see cref="DefaultPort"/> and <see cref="Host"/> is
    /// not an IP literal.
    /// </remarks>
    public bool UseSrv { get; init; } = true;

    /// <summary>
    /// Gets or sets the maximum time the SRV lookup may take before the connect proceeds with
    /// <see cref="Host"/> as given.
    /// </summary>
    /// <value>The SRV lookup timeout. The default is 5 seconds.</value>
    /// <remarks>
    /// The value is capped by <see cref="ConnectTimeout"/>. It can only shorten the lookup: the resolver
    /// applies a budget of its own, so a value above five seconds has no effect.
    /// </remarks>
    public TimeSpan SrvTimeout { get; init; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Gets or sets the proxy client through which the connection is opened.
    /// </summary>
    /// <value>The proxy client to use, or <see langword="null"/> to connect directly over TCP. The default
    /// is <see langword="null"/>.</value>
    /// <remarks>
    /// The client asks the proxy for a stream to the resolved host and port and owns that stream from then
    /// on. <see cref="NoDelay"/> is not applied to the proxy socket; the proxy client carries its own
    /// setting.
    /// </remarks>
    public IProxyClient? Proxy { get; init; }

    /// <summary>
    /// Gets or sets the local endpoint the outgoing socket is bound to before connecting.
    /// </summary>
    /// <value>The local address and port to bind to, or <see langword="null"/> to leave the socket unbound.
    /// The default is <see langword="null"/>.</value>
    /// <remarks>
    /// Port 0 lets the operating system choose an ephemeral port. The address family must match the
    /// server's; an IPv4 local endpoint cannot reach an IPv6-only host. This applies to the direct TCP path
    /// only. When <see cref="Proxy"/> is set, the proxy client owns its socket and carries a local endpoint
    /// setting of its own.
    /// </remarks>
    public IPEndPoint? LocalEndPoint { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether small frames are sent as they come instead of waiting for a full
    /// segment.
    /// </summary>
    /// <value><see langword="true"/> to disable Nagle's algorithm on the socket; otherwise,
    /// <see langword="false"/>. The default is <see langword="true"/>.</value>
    public bool NoDelay { get; init; } = true;
}
