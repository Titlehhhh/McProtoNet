using System.Diagnostics.CodeAnalysis;
using McProtoNet.NBT;
using McProtoNet.Primitives;
namespace McProtoNet.Protocol;

/// <summary>
/// Provides static methods that decode an <see cref="IncomingPacket"/> into a concrete packet type.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="IncomingPacket.Body"/> is a window into the transport buffer and is valid only until the
/// next read. Both entry points decode immediately and return a packet that owns its data.
/// </para>
/// <para>
/// Only a broken or unsupported wire form is reported as a decode error. <see cref="InvalidDataException"/>,
/// <see cref="EndOfStreamException"/>, <see cref="NbtFormatException"/>, <see cref="WrongLayerException"/>
/// and <see cref="PacketDecodeException"/> map to <see cref="DecodeError.Malformed"/>, and
/// <see cref="ProtocolNotSupportException"/> maps to <see cref="DecodeError.UnsupportedVersion"/>. Any
/// other exception is not treated as a decode error and is propagated to the caller.
/// </para>
/// </remarks>
public static class PacketIo
{
    /// <summary>
    /// Attempts to decode the body of the specified raw packet as the packet type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The packet type to decode.</typeparam>
    /// <param name="raw">The raw packet whose body is decoded.</param>
    /// <param name="protocolVersion">The protocol version to decode the body for.</param>
    /// <param name="packet">When this method returns, contains the decoded packet, or
    /// <see langword="null"/> if the body could not be decoded.</param>
    /// <param name="error">When this method returns, contains the reason the body could not be decoded,
    /// or <see cref="DecodeError.None"/> if it was decoded.</param>
    /// <returns>
    /// <see langword="true"/> if the body was decoded; otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// A body that is decoded but leaves unread bytes is reported as
    /// <see cref="DecodeError.TrailingBytes"/>.
    /// </remarks>
    public static bool TryDecode<T>(in IncomingPacket raw, int protocolVersion,
        [NotNullWhen(true)] out T? packet, out DecodeError error)
        where T : class, IPacket<T>
    {
        packet = null;
        var reader = new MinecraftPrimitiveReader(raw.Body);
        try
        {
            packet = T.Read(ref reader, protocolVersion);
        }
        catch (ProtocolNotSupportException)
        {
            error = DecodeError.UnsupportedVersion;
            return false;
        }
        catch (Exception ex) when (IsMalformed(ex))
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

    /// <summary>
    /// Decodes the body of the specified raw packet as the packet type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The packet type to decode.</typeparam>
    /// <param name="raw">The raw packet whose body is decoded.</param>
    /// <param name="protocolVersion">The protocol version to decode the body for.</param>
    /// <returns>The decoded packet.</returns>
    /// <exception cref="PacketDecodeException">
    /// The body is malformed.
    /// -or-
    /// The packet is not supported on <paramref name="protocolVersion"/>.
    /// -or-
    /// The body was decoded but bytes remain unread.
    /// </exception>
    public static T Decode<T>(in IncomingPacket raw, int protocolVersion) where T : class, IPacket<T>
    {
        var reader = new MinecraftPrimitiveReader(raw.Body);
        T packet;
        try
        {
            packet = T.Read(ref reader, protocolVersion);
        }
        catch (ProtocolNotSupportException e)
        {
            throw new PacketDecodeException(typeof(T), DecodeError.UnsupportedVersion, e);
        }
        catch (Exception e) when (IsMalformed(e))
        {
            throw new PacketDecodeException(typeof(T), DecodeError.Malformed, e);
        }

        if (reader.RemainingCount != 0)
            throw new PacketDecodeException(typeof(T), DecodeError.TrailingBytes);

        return packet;
    }

    private static bool IsMalformed(Exception ex) =>
        ex is InvalidDataException or EndOfStreamException or NbtFormatException
            or WrongLayerException or PacketDecodeException;
}
