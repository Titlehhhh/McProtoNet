using System.Diagnostics.CodeAnalysis;
using McProtoNet.Primitives;
namespace McProtoNet.Protocol;

/// <summary>
///     IncomingPacket -> concrete packet. <see cref="IncomingPacket.Data" /> is a window into the pipe
///     buffer, valid only until the next transport read — both entry points decode immediately;
///     what leaves is a materialized packet that owns its data.
/// </summary>
public static class PacketIo
{
    public static bool TryDecode<T>(in IncomingPacket raw, int protocolVersion,
        [NotNullWhen(true)] out T? packet, out DecodeError error)
        where T : class, IPacket<T>
    {
        packet = null;
        var reader = new MinecraftPrimitiveReader(raw.Data);
        try
        {
            packet = T.Read(ref reader, protocolVersion);
        }
        catch (ProtocolNotSupportException)
        {
            error = DecodeError.UnsupportedVersion;
            return false;
        }
        catch (Exception)
        {
            error = DecodeError.Malformed;
            return false;
        }

        if (reader.RemainingCount != 0)
        {
            packet = null;
            error = DecodeError.TrailingBytes;
            return false;
        }

        error = DecodeError.None;
        return true;
    }

    public static T Decode<T>(in IncomingPacket raw, int protocolVersion) where T : class, IPacket<T>
    {
        var reader = new MinecraftPrimitiveReader(raw.Data);
        T packet;
        try
        {
            packet = T.Read(ref reader, protocolVersion);
        }
        catch (ProtocolNotSupportException e)
        {
            throw new PacketDecodeException(typeof(T), DecodeError.UnsupportedVersion, e);
        }
        catch (Exception e)
        {
            throw new PacketDecodeException(typeof(T), DecodeError.Malformed, e);
        }

        if (reader.RemainingCount != 0)
            throw new PacketDecodeException(typeof(T), DecodeError.TrailingBytes);

        return packet;
    }
}
