using QuickProxyNet;

namespace McProtoNet.Abstractions;

/// <summary>
/// Configuration options for starting a Minecraft client.
/// </summary>
public struct MinecraftClientStartOptions
{
    /// <summary>
    /// Gets the host address of the Minecraft server.
    /// </summary>
    public string Host { get; init; }
    
    /// <summary>
    /// Gets the port number of the Minecraft server.
    /// </summary>
    public int Port { get; init; }
    
    /// <summary>
    /// Gets the version of the Minecraft client.
    /// </summary>
    public int Version { get; init; }
    
    /// <summary>
    /// Gets or sets the proxy client used to connect to the server.
    /// </summary>
    public IProxyClient? Proxy { get; set; }
    
    /// <summary>
    /// Gets the connection timeout duration.
    /// </summary>
    public TimeSpan ConnectTimeout { get; init; }
    
    /// <summary>
    /// Gets the read timeout duration.
    /// </summary>
    public TimeSpan ReadTimeout { get; init; }
    
    /// <summary>
    /// Gets the write timeout duration.
    /// </summary>
    public TimeSpan WriteTimeout { get; init; }
}