using McProtoNet.Net;

namespace McProtoNet;

/// <summary>
///     Represents a Minecraft client.
/// </summary>
public interface IMinecraftClient : IDisposable
{
    /// <summary>
    ///     Sends a packet to the server.
    /// </summary>
    ValueTask SendPacket(OutputPacket packet, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets an asynchronous sequence of packets received from the server.
    /// </summary>
    IAsyncEnumerable<InputPacket> ReceivePackets(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a value indicating whether the client is connected to the server.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    ///     Connects to the server asynchronously.
    /// </summary>
    ValueTask ConnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Switches packet compression on or off.
    /// </summary>
    void SwitchCompression(int threshold);

    /// <summary>
    ///     Switches packet encryption on or off.
    /// </summary>
    void SwitchEncryption(Span<byte> privateKey);

    /// <summary>
    ///     Gets the start options for the client.
    /// </summary>
    MinecraftClientStartOptions StartOptions { get; }

    int ProtocolVersion { get; }
}
