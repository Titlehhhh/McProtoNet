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
    ///     Send small frames as they come instead of waiting for a full segment. On by default: a
    ///     Minecraft client writes small packets and wants each one out now.
    /// </summary>
    public bool NoDelay { get; init; } = true;
}
