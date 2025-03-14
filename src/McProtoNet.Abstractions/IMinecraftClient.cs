﻿namespace McProtoNet.Abstractions;


/// <summary>
///     Represents a Minecraft client.
/// </summary>
public interface IMinecraftClient : IDisposable
{
    /// <summary>
    ///     Sends a packet to the server.
    /// </summary>
    /// <param name="packet">The packet to send.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the client is disposed, or when the client is stopping.</exception>
    ValueTask SendPacket(OutputPacket packet, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets an observable sequence of packets received from the server.
    /// </summary>
    IAsyncEnumerable<InputPacket> ReceivePackets(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a value indicating whether the client is connected to the server.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    ///     Gets a value indicating whether the client is stopping.
    /// </summary>
    /// <returns>true if the client is stopping; otherwise, false.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the client is disposed.</exception>
    ValueTask ConnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Switches packet compression on or off.
    /// </summary>
    /// <param name="threshold">The threshold above which packets are compressed.</param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the client is disposed, or when the client is stopping.
    /// </exception>
    void SwitchCompression(int threshold);

    /// <summary>
    ///     Switches packet encryption on or off.
    /// </summary>
    /// <param name="privateKey">The private key to use for encryption.</param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the client is disposed, or when the client is stopping.
    /// </exception>
    void SwitchEncryption(Span<byte> privateKey);

    /// <summary>
    ///     Gets the start options for the client.
    /// </summary>
    MinecraftClientStartOptions StartOptions { get; }

    int ProtocolVersion { get; }
}