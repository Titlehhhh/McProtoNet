// McProtoNet.Next experiment — typed floor. Lives next to the old API, changes nothing in src/**.

using System.Diagnostics.CodeAnalysis;
using McProtoNet.Net;
using McProtoNet.Protocol;

namespace McProtoNet.Next;

/// <summary>
///     Sugar for the raw floor: ask a raw packet whether it is a given packet type and decode
///     it in one short word, instead of spelling out <see cref="PacketIo" /> statics.
/// </summary>
/// <remarks>
///     <see cref="InputPacket.Data" /> is a window into the transport buffer, valid only until
///     the next transport read: call these members immediately after receiving the packet,
///     never across an <c>await</c>. The decoded packet that comes out owns its data.
/// </remarks>
public static class InputPacketExtensions
{
    /// <summary>
    ///     Checks whether the wire id of this raw packet matches packet type
    ///     <typeparamref name="T" /> on the given protocol version.
    /// </summary>
    /// <typeparam name="T">The packet type to test against.</typeparam>
    /// <param name="packet">The raw packet whose id to test.</param>
    /// <param name="protocolVersion">The protocol version that defines the wire id of <typeparamref name="T" />.</param>
    /// <returns>
    ///     <c>true</c> when <typeparamref name="T" /> exists on the version and its wire id equals
    ///     <see cref="InputPacket.Id" />; <c>false</c> when the ids differ or the type does not
    ///     exist on the version.
    /// </returns>
    /// <remarks>
    ///     Only ids are compared. A wire id is unique per phase and direction, not globally —
    ///     the caller is responsible for asking about a type that lives in the phase the
    ///     connection is actually in.
    /// </remarks>
    public static bool Is<T>(this in InputPacket packet, int protocolVersion) where T : class, IPacket<T>
        => T.TryGetPacketId(protocolVersion, out var id) && id == packet.Id;

    /// <summary>
    ///     Decodes the body of this raw packet as <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The packet type to decode.</typeparam>
    /// <param name="packet">The raw packet to decode.</param>
    /// <param name="protocolVersion">The protocol version used to decode the body.</param>
    /// <returns>The decoded packet; it owns its data and outlives the transport window.</returns>
    /// <exception cref="PacketDecodeException">
    ///     The body does not decode as <typeparamref name="T" />: the type does not exist on the
    ///     version (<see cref="DecodeError.UnsupportedVersion" />), the data is broken
    ///     (<see cref="DecodeError.Malformed" />), or bytes remain after reading
    ///     (<see cref="DecodeError.TrailingBytes" />).
    /// </exception>
    public static T Decode<T>(this in InputPacket packet, int protocolVersion) where T : class, IPacket<T>
        => PacketIo.Decode<T>(in packet, protocolVersion);

    /// <summary>
    ///     Attempts to decode the body of this raw packet as <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The packet type to decode.</typeparam>
    /// <param name="packet">The raw packet to decode.</param>
    /// <param name="protocolVersion">The protocol version used to decode the body.</param>
    /// <param name="decoded">The decoded packet, or <c>null</c> when decoding fails.</param>
    /// <param name="error">Why decoding failed; <see cref="DecodeError.None" /> on success.</param>
    /// <returns><c>true</c> when the body decodes fully; otherwise <c>false</c>.</returns>
    public static bool TryDecode<T>(this in InputPacket packet, int protocolVersion,
        [NotNullWhen(true)] out T? decoded, out DecodeError error) where T : class, IPacket<T>
        => PacketIo.TryDecode(in packet, protocolVersion, out decoded, out error);
}
