using System.Buffers;
using DotNext.Buffers;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public static class PacketMarshaller
{
    
    
    /// <summary>
    /// Serializes the packet into a pooled buffer.
    /// The caller MUST dispose the returned <see cref="MemoryOwner{T}"/> when done.
    /// </summary>
    public static MemoryOwner<byte> Serialize(IPacket packet, int protocolVersion)
    {
        if (!packet.IsVersionSupported(protocolVersion))
            throw new ProtocolNotSupportException(packet.GetPacketId().Name, protocolVersion, packet.GetSupportedVersions());

        var writer = new MinecraftPrimitiveWriter();
        try
        {
            packet.Serialize(ref writer, protocolVersion);
            return writer.GetWrittenMemory();
        }
        catch (PacketSerializationException)
        {
            writer.Dispose();
            throw;
        }
        catch (Exception ex)
        {
            writer.Dispose();
            throw new PacketSerializationException(
                $"Failed to serialize packet '{packet.GetPacketId().Name}' for protocol {protocolVersion}.",
                ex,
                packet.GetPacketId().Name,
                protocolVersion);
        }
    }

    /// <summary>
    /// Deserializes the packet from a <see cref="ReadOnlyMemory{T}"/>.
    /// </summary>
    public static void Deserialize(IPacket packet, ReadOnlyMemory<byte> data, int protocolVersion)
    {
        if (!packet.IsVersionSupported(protocolVersion))
            throw new ProtocolNotSupportException(packet.GetPacketId().Name, protocolVersion, packet.GetSupportedVersions());

        var reader = new MinecraftPrimitiveReader(data);
        try
        {
            packet.Deserialize(ref reader, protocolVersion);
        }
        catch (PacketDeserializationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new PacketDeserializationException(
                $"Failed to deserialize packet '{packet.GetPacketId().Name}' for protocol {protocolVersion}.",
                ex,
                packet.GetPacketId().Name,
                protocolVersion);
        }
    }

    /// <summary>
    /// Serializes the packet directly into an existing <see cref="MinecraftPrimitiveWriter"/>.
    /// Caller controls buffer lifetime.
    /// </summary>
    public static void Serialize(IPacket packet, ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        if (!packet.IsVersionSupported(protocolVersion))
            throw new ProtocolNotSupportException(packet.GetPacketId().Name, protocolVersion, packet.GetSupportedVersions());

        try
        {
            packet.Serialize(ref writer, protocolVersion);
        }
        catch (PacketSerializationException) { throw; }
        catch (Exception ex)
        {
            throw new PacketSerializationException(
                $"Failed to serialize packet '{packet.GetPacketId().Name}' for protocol {protocolVersion}.",
                ex, packet.GetPacketId().Name, protocolVersion);
        }
    }

    /// <summary>
    /// Deserializes the packet directly from an existing <see cref="MinecraftPrimitiveReader"/>.
    /// </summary>
    public static void Deserialize(IPacket packet, ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        if (!packet.IsVersionSupported(protocolVersion))
            throw new ProtocolNotSupportException(packet.GetPacketId().Name, protocolVersion, packet.GetSupportedVersions());

        try
        {
            packet.Deserialize(ref reader, protocolVersion);
        }
        catch (PacketDeserializationException) { throw; }
        catch (Exception ex)
        {
            throw new PacketDeserializationException(
                $"Failed to deserialize packet '{packet.GetPacketId().Name}' for protocol {protocolVersion}.",
                ex, packet.GetPacketId().Name, protocolVersion);
        }
    }

    /// <summary>
    /// Deserializes the packet from a <see cref="ReadOnlySequence{T}"/> (pipeline/network path).
    /// </summary>
    public static void Deserialize(IPacket packet, ReadOnlySequence<byte> data, int protocolVersion)
    {
        if (!packet.IsVersionSupported(protocolVersion))
            throw new ProtocolNotSupportException(packet.GetPacketId().Name, protocolVersion, packet.GetSupportedVersions());

        var reader = new MinecraftPrimitiveReader(data);
        try
        {
            packet.Deserialize(ref reader, protocolVersion);
        }
        catch (PacketDeserializationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new PacketDeserializationException(
                $"Failed to deserialize packet '{packet.GetPacketId().Name}' for protocol {protocolVersion}.",
                ex,
                packet.GetPacketId().Name,
                protocolVersion);
        }
    }
}
