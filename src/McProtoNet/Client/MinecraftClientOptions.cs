namespace McProtoNet;

/// <summary>Connection options for <see cref="MinecraftClient" />.</summary>
public sealed class MinecraftClientOptions
{
    public required string Host { get; init; }

    public int Port { get; init; } = 25565;

    public TimeSpan ConnectTimeout { get; init; } = TimeSpan.FromSeconds(30);
}
