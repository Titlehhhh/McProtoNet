// EXPERIMENT (McProtoNet.Next). Options bag for MinecraftClient, following the
// BCL options pattern (JsonSerializerOptions, PipeOptions): a mutable class with
// get/set properties and a parameterless constructor, passed to the factory
// methods. The client snapshots the values at creation time — mutating the
// instance afterwards has no effect on an already created client.

#nullable enable

using System;
using System.IO;
using System.Threading;

namespace McProtoNet.Next;

/// <summary>
/// Optional settings for <see cref="MinecraftClient"/> creation.
/// </summary>
/// <remarks>
/// The client reads the values once, inside
/// <see cref="MinecraftClient.ConnectAsync(string, int, MinecraftClientOptions, CancellationToken)"/>
/// or <see cref="MinecraftClient.Create(Stream, MinecraftClientOptions)"/>.
/// Changes made to this instance after the client is created are ignored.
/// </remarks>
public sealed class MinecraftClientOptions
{
    /// <summary>
    /// Gets or sets the initial compression threshold in bytes.
    /// </summary>
    /// <value>
    /// <c>-1</c> disables compression (the default). <c>0</c> or a positive value
    /// enables the compression envelope for frames whose payload is at least this long.
    /// </value>
    /// <remarks>
    /// The live value can be changed later through
    /// <see cref="MinecraftClient.CompressionThreshold"/> — the protocol announces it
    /// mid-login with the Set Compression packet.
    /// </remarks>
    public int CompressionThreshold { get; set; } = -1;

    /// <summary>
    /// Gets or sets the timeout for establishing the TCP connection.
    /// </summary>
    /// <value>
    /// A positive interval, or <see cref="Timeout.InfiniteTimeSpan"/> to disable the
    /// timeout. The default is 30 seconds.
    /// </value>
    /// <remarks>
    /// Used only by
    /// <see cref="MinecraftClient.ConnectAsync(string, int, MinecraftClientOptions, CancellationToken)"/>.
    /// When the timeout elapses the connect attempt fails with
    /// <see cref="TimeoutException"/>; cancellation through the caller's token still
    /// surfaces as <see cref="OperationCanceledException"/>.
    /// </remarks>
    public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets a value that indicates whether the Nagle algorithm is disabled on
    /// the socket (<c>TCP_NODELAY</c>).
    /// </summary>
    /// <value>
    /// <see langword="true"/> to send small packets immediately (the default, and the
    /// sensible choice for an interactive game protocol); <see langword="false"/> to
    /// let the OS coalesce writes.
    /// </value>
    /// <remarks>
    /// Used only by
    /// <see cref="MinecraftClient.ConnectAsync(string, int, MinecraftClientOptions, CancellationToken)"/>.
    /// </remarks>
    public bool NoDelay { get; set; } = true;

    /// <summary>
    /// Gets or sets a value that indicates whether the client leaves the underlying
    /// stream open when it is disposed.
    /// </summary>
    /// <value>
    /// <see langword="false"/> (the default) to dispose the stream together with the
    /// client; <see langword="true"/> to leave it open.
    /// </value>
    /// <remarks>
    /// Honored only by <see cref="MinecraftClient.Create(Stream, MinecraftClientOptions)"/>;
    /// <see cref="MinecraftClient.ConnectAsync(string, int, MinecraftClientOptions, CancellationToken)"/>
    /// always owns the socket stream it creates. Note that closing the stream is how
    /// disposal forces the internal pumps out of blocking I/O; with
    /// <see langword="true"/> the client relies on the stream honoring
    /// <see cref="CancellationToken"/> in its read/write calls.
    /// </remarks>
    public bool LeaveOpen { get; set; }
}
