using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

/// <summary>
///     Typed send: id comes from the type, varint(id) + body assembly leaves consumer code.
///     The transport appends length and compression itself.
///     <para>
///     Both methods build the body in a writer rented from <see cref="MinecraftPrimitiveWriterCache" />
///     and returned in a <c>finally</c> — one writer per thread instead of one per packet. They hand
///     the transport <see cref="MinecraftPrimitiveWriter.WrittenMemory" /> directly, with no pooled
///     copy in between: <c>SendPacketAsync</c> passes the body to <c>WritePacket</c>, which takes a
///     <see cref="ReadOnlySpan{T}" /> and therefore cannot retain it, and it does so before its first
///     await. The body is inside the pipe before the send task can complete, so the writer is free to
///     be reused the moment the await returns.
///     </para>
/// </summary>
public static class ClientPacketExtensions
{
    public static async ValueTask SendAsync<T>(this MinecraftConnection connection, T packet, int protocolVersion,
        CancellationToken cancellationToken = default)
        where T : class, IPacket<T>
    {
        if (!T.TryGetPacketId(protocolVersion, out var id))
            throw new UnsupportedVersionException(T.Identity.Key, protocolVersion);

        var writer = MinecraftPrimitiveWriterCache.Rent();
        try
        {
            writer.WriteVarInt(id);
            packet.Write(writer, protocolVersion);
            await connection.SendPacketAsync(writer.WrittenMemory, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            MinecraftPrimitiveWriterCache.Return(writer);
        }
    }

    public static ValueTask SendAsync<T>(this MinecraftClient client, T packet, int protocolVersion,
        CancellationToken cancellationToken = default)
        where T : class, IPacket<T>
        => client.Connection.SendAsync(packet, protocolVersion, cancellationToken);

    public static ValueTask SendRawAsync(this MinecraftClient client, int id, ReadOnlyMemory<byte> body,
        CancellationToken cancellationToken = default)
        => client.Connection.SendRawAsync(id, body, cancellationToken);

    /// <summary>Low-level path stays open: panel injection, replays, fuzzing.</summary>
    public static async ValueTask SendRawAsync(this MinecraftConnection connection, int id, ReadOnlyMemory<byte> body,
        CancellationToken cancellationToken = default)
    {
        var writer = MinecraftPrimitiveWriterCache.Rent();
        try
        {
            writer.WriteVarInt(id);
            writer.WriteBuffer(body.Span);
            await connection.SendPacketAsync(writer.WrittenMemory, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            MinecraftPrimitiveWriterCache.Return(writer);
        }
    }
}
