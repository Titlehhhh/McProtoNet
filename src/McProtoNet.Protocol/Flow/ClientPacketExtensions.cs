using McProtoNet.Client;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

/// <summary>
///     Typed send: id comes from the type, varint(id) + body assembly leaves consumer code.
///     The transport appends length and compression itself.
/// </summary>
public static class ClientPacketExtensions
{
    public static async ValueTask SendAsync<T>(this PipelinesMinecraftClient client, T packet, int protocolVersion,
        CancellationToken cancellationToken = default)
        where T : class, IPacket<T>
    {
        if (!T.TryGetPacketId(protocolVersion, out var id))
            throw new UnsupportedVersionException(T.Identity.Key, protocolVersion);

        var writer = new MinecraftPrimitiveWriter();
        writer.WriteVarInt(id);
        packet.Write(writer, protocolVersion);
        using var owner = writer.GetWrittenMemory();
        await client.SendPacketAsync(owner.Memory, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Low-level path stays open: panel injection, replays, fuzzing.</summary>
    public static async ValueTask SendRawAsync(this PipelinesMinecraftClient client, int id, ReadOnlyMemory<byte> body,
        CancellationToken cancellationToken = default)
    {
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteVarInt(id);
        writer.WriteBuffer(body.Span);
        using var owner = writer.GetWrittenMemory();
        await client.SendPacketAsync(owner.Memory, cancellationToken).ConfigureAwait(false);
    }
}
