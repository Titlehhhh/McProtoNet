using McProtoNet.Primitives;
using McProtoNet.Protocol;
using McProtoNet.Transport;

namespace McProtoNet;

/// <summary>
///     Typed send: the id comes from the packet type, the varint(id) + body assembly leaves consumer
///     code. The transport appends length, compression and encryption itself.
///     <para>
///     Every method builds the body in a writer rented from <see cref="MinecraftPrimitiveWriterCache" />
///     and returned in a <c>finally</c> — one writer per thread instead of one per packet. The
///     transport takes the body as a span or copies it into its own buffer before the first await,
///     so the writer is free again the moment the call returns.
///     </para>
/// </summary>
public static class ClientPacketExtensions
{
    /// <summary>Login-mode send: framed, encrypted and at the socket when the call returns.</summary>
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

    /// <summary>Low-level login-mode path: panel injection, replays, fuzzing.</summary>
    public static ValueTask SendRawAsync(this MinecraftConnection connection, int id, ReadOnlyMemory<byte> body,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connection);
        return connection.WritePacketAsync(id, body, cancellationToken);
    }

    /// <summary>Streaming-mode send: frames into the send buffer, nothing leaves until a flush.</summary>
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

    /// <summary>Streaming-mode send with an immediate flush: at the socket when the call returns.</summary>
    public static ValueTask SendAsync<T>(this StreamingConnection connection, T packet, int protocolVersion,
        CancellationToken cancellationToken = default)
        where T : class, IPacket<T>
    {
        connection.WritePacket(packet, protocolVersion);
        return connection.FlushAsync(cancellationToken);
    }
}
