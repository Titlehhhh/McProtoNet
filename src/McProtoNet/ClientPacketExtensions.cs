using McProtoNet.Primitives;
using McProtoNet.Protocol;
using McProtoNet.Transport;

namespace McProtoNet;

/// <summary>
/// Provides typed send methods for <see cref="MinecraftConnection"/> and
/// <see cref="StreamingConnection"/>.
/// </summary>
/// <remarks>
/// <para>
/// The wire id is taken from the packet type, so consumer code does not assemble the id and body
/// itself. The transport adds the length prefix, compression and encryption.
/// </para>
/// <para>
/// Each method builds the body in a writer rented from <see cref="MinecraftPrimitiveWriterCache"/> and
/// returns it before the call ends, so one writer is used per thread instead of one per packet.
/// </para>
/// </remarks>
public static class ClientPacketExtensions
{
    /// <summary>
    /// Asynchronously writes the specified packet as one frame on a connection in packet mode.
    /// </summary>
    /// <typeparam name="T">The packet type. Its wire id is taken from the type.</typeparam>
    /// <param name="connection">The connection to write to.</param>
    /// <param name="packet">The packet to send.</param>
    /// <param name="protocolVersion">The protocol version to serialize for.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is
    /// <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous send operation. When it completes, the frame has
    /// been handed to the socket.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="connection"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="ProtocolNotSupportException">The packet has no id on the specified protocol
    /// version.</exception>
    /// <exception cref="ObjectDisposedException"><paramref name="connection"/> has already been
    /// disposed.</exception>
    /// <exception cref="ConnectionAbortedException">The connection is closed, or the stream
    /// failed.</exception>
    /// <exception cref="InvalidOperationException">Another write on <paramref name="connection"/> is already
    /// in progress.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is
    /// stored into the returned task.</exception>
    public static async ValueTask SendAsync<T>(this MinecraftConnection connection, T packet, int protocolVersion,
        CancellationToken cancellationToken = default)
        where T : class, IPacket<T>
    {
        ArgumentNullException.ThrowIfNull(connection);
        if (!T.TryGetPacketId(protocolVersion, out var id))
            throw new ProtocolNotSupportException(T.Identity.Key, protocolVersion);

        var writer = MinecraftPrimitiveWriterCache.Rent();
        try
        {
            packet.Write(writer, protocolVersion);
            await connection.WritePacketAsync(id, writer.WrittenMemory, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            MinecraftPrimitiveWriterCache.Return(writer);
        }
    }

    /// <summary>
    /// Asynchronously writes one frame from the specified packet id and body on a connection in packet
    /// mode.
    /// </summary>
    /// <param name="connection">The connection to write to.</param>
    /// <param name="id">The wire id of the packet.</param>
    /// <param name="body">The already serialized packet body, without the id.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is
    /// <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="connection"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException"><paramref name="connection"/> has already been
    /// disposed.</exception>
    /// <exception cref="ConnectionAbortedException">The connection is closed, or the stream
    /// failed.</exception>
    /// <exception cref="InvalidOperationException">Another write on <paramref name="connection"/> is already
    /// in progress.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is
    /// stored into the returned task.</exception>
    public static ValueTask SendRawAsync(this MinecraftConnection connection, int id, ReadOnlyMemory<byte> body,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connection);
        return connection.WritePacketAsync(id, body, cancellationToken);
    }

    /// <summary>
    /// Writes the specified packet into the send buffer of a connection in streaming mode.
    /// </summary>
    /// <typeparam name="T">The packet type. Its wire id is taken from the type.</typeparam>
    /// <param name="connection">The connection to write to.</param>
    /// <param name="packet">The packet to write.</param>
    /// <param name="protocolVersion">The protocol version to serialize for.</param>
    /// <exception cref="ArgumentNullException"><paramref name="connection"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="ProtocolNotSupportException">The packet has no id on the specified protocol
    /// version.</exception>
    /// <exception cref="ObjectDisposedException"><paramref name="connection"/> has already been
    /// disposed.</exception>
    /// <exception cref="ConnectionAbortedException">The connection is closed.</exception>
    /// <exception cref="InvalidOperationException">A previous flush on <paramref name="connection"/>
    /// failed part-way, so the writer can no longer be used.</exception>
    /// <remarks>
    /// Nothing leaves the buffer until <see cref="StreamingConnection.FlushAsync"/> is called.
    /// </remarks>
    public static void WritePacket<T>(this StreamingConnection connection, T packet, int protocolVersion)
        where T : class, IPacket<T>
    {
        ArgumentNullException.ThrowIfNull(connection);
        if (!T.TryGetPacketId(protocolVersion, out var id))
            throw new ProtocolNotSupportException(T.Identity.Key, protocolVersion);

        var writer = MinecraftPrimitiveWriterCache.Rent();
        try
        {
            packet.Write(writer, protocolVersion);
            connection.WritePacket(id, writer.WrittenSpan);
        }
        finally
        {
            MinecraftPrimitiveWriterCache.Return(writer);
        }
    }

    /// <summary>
    /// Asynchronously writes the specified packet on a connection in streaming mode and flushes the send
    /// buffer.
    /// </summary>
    /// <typeparam name="T">The packet type. Its wire id is taken from the type.</typeparam>
    /// <param name="connection">The connection to write to.</param>
    /// <param name="packet">The packet to send.</param>
    /// <param name="protocolVersion">The protocol version to serialize for.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is
    /// <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous send operation. When it completes, the buffered
    /// bytes have been handed to the socket.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="connection"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="ProtocolNotSupportException">The packet has no id on the specified protocol
    /// version.</exception>
    /// <exception cref="ObjectDisposedException"><paramref name="connection"/> has already been
    /// disposed.</exception>
    /// <exception cref="ConnectionAbortedException">The connection is closed, or the stream
    /// failed.</exception>
    /// <exception cref="InvalidOperationException">A previous flush on <paramref name="connection"/>
    /// failed part-way, so the writer can no longer be used.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is
    /// stored into the returned task.</exception>
    public static ValueTask SendAsync<T>(this StreamingConnection connection, T packet, int protocolVersion,
        CancellationToken cancellationToken = default)
        where T : class, IPacket<T>
    {
        connection.WritePacket(packet, protocolVersion);
        return connection.FlushAsync(cancellationToken);
    }
}
