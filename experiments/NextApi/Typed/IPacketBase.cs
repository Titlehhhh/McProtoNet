// McProtoNet.Next experiment — typed floor. Lives next to the old API, changes nothing in src/**.

using System;
using System.Diagnostics.CodeAnalysis;
using McProtoNet.Net;
using McProtoNet.Protocol;

namespace McProtoNet.Next;

/// <summary>
///     Non-generic view of a decoded packet: the instance-level counterpart of
///     <see cref="IPacket{TSelf}" />. The generated contract exposes <c>Identity</c> only as a
///     static abstract member, so a decoded packet held by a plain reference cannot answer
///     "which packet are you?" without its concrete type. This interface closes that gap and is
///     the element type the typed stream wants to yield.
/// </summary>
/// <remarks>
///     <para>
///         The generated packet classes do not implement this interface yet, and this experiment
///         does not change generated code. Until the generator emits <c>IPacketBase</c> on every
///         packet, <see cref="PacketAdapter{T}" /> carries a decoded packet behind this interface:
///         the visitor inside <see cref="PacketAdapter.TryDecode" /> catches the packet with its
///         concrete type <c>T</c> statically known and wraps it. Packets are
///         classes, so there is no boxing anywhere on this path; the adapter itself is one small
///         allocation that a generator-emitted interface would remove.
///     </para>
///     <para>
///         API-review decision: the whole family is <b>internal</b> — a prototype, not surface.
///         No public door consumes or yields it yet (<c>ReadTypedAsync</c> yields
///         <see cref="object" />), and an interface shipped without an API that takes it would
///         only compete with the real door. It goes public if and when the generator emits it
///         on the packet classes.
///     </para>
/// </remarks>
internal interface IPacketBase
{
    /// <summary>Gets the identity of the packet type this instance belongs to.</summary>
    PacketIdentity Identity { get; }
}

/// <summary>
///     Adapter that carries a decoded packet behind the non-generic <see cref="IPacketBase" />
///     while the generated packet classes do not implement that interface themselves.
/// </summary>
/// <typeparam name="T">The concrete generated packet type.</typeparam>
/// <remarks>
///     One adapter instance is one allocation; the wrapped packet is a class, so no boxing
///     occurs. Consumers who need the concrete packet back match on
///     <c>PacketAdapter&lt;SomePacket&gt;</c> and read <see cref="Packet" /> — which is exactly
///     why the main typed stream yields the packet objects directly instead of adapters.
/// </remarks>
internal sealed class PacketAdapter<T> : IPacketBase where T : class, IPacket<T>
{
    /// <summary>Initializes the adapter around a decoded packet.</summary>
    /// <param name="packet">The decoded packet to expose.</param>
    /// <exception cref="ArgumentNullException"><paramref name="packet" /> is <c>null</c>.</exception>
    public PacketAdapter(T packet)
    {
        ArgumentNullException.ThrowIfNull(packet);
        Packet = packet;
    }

    /// <summary>Gets the wrapped packet with its concrete type intact.</summary>
    public T Packet { get; }

    /// <inheritdoc />
    public PacketIdentity Identity => T.Identity;
}

/// <summary>
///     Non-generic companion of <see cref="PacketAdapter{T}" />. Demonstrates the bridge
///     pattern: dispatch a raw packet through <see cref="PacketFlow" /> and hand the decoded
///     result out as <see cref="IPacketBase" />.
/// </summary>
internal static class PacketAdapter
{
    /// <summary>
    ///     Decodes a raw packet and returns it behind <see cref="IPacketBase" />.
    /// </summary>
    /// <param name="raw">The raw packet to decode.</param>
    /// <param name="protocolVersion">The protocol version used to decode the body.</param>
    /// <param name="phase">The protocol phase the connection is currently in.</param>
    /// <param name="direction">The direction the packet travels.</param>
    /// <param name="packet">The decoded packet behind the non-generic interface, or <c>null</c>.</param>
    /// <returns>
    ///     <c>true</c> when the registry maps the wire id and the packet decodes; <c>false</c> when
    ///     the id is unknown in this phase and direction — a normal condition of the stream, not an
    ///     error, which is why this member has no throwing twin.
    /// </returns>
    /// <remarks>
    ///     <see cref="InputPacket.Data" /> is a window into the transport buffer, valid only until
    ///     the next transport read: call this method immediately, never across an <c>await</c>.
    ///     The packet that comes out owns its data and lives as long as you like.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <paramref name="phase" /> or <paramref name="direction" /> is not a defined value.
    /// </exception>
    public static bool TryDecode(in InputPacket raw, int protocolVersion, PacketPhase phase,
        PacketDirection direction, [NotNullWhen(true)] out IPacketBase? packet)
    {
        if (phase > PacketPhase.Play)
            throw new ArgumentOutOfRangeException(nameof(phase), phase, "Unknown protocol phase.");
        if (direction > PacketDirection.Serverbound)
            throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unknown packet direction.");

        var visitor = new AdaptingVisitor();
        PacketFlow.Dispatch(in raw, protocolVersion, phase, direction, ref visitor);
        packet = visitor.Result;
        return packet is not null;
    }

    /// <summary>
    ///     The catch point of the pattern. <see cref="IPacketVisitor.Visit{T}" /> receives the
    ///     packet with <c>T</c> statically known at the generated call site, wraps it in
    ///     <see cref="PacketAdapter{T}" />, and the wrapper leaves as <see cref="IPacketBase" />.
    ///     Unknown ids leave no result; the Try method reports <c>false</c>.
    /// </summary>
    private struct AdaptingVisitor : IPacketVisitor
    {
        public IPacketBase? Result;

        public void Visit<T>(T packet) where T : class, IPacket<T> => Result = new PacketAdapter<T>(packet);

        public void Unknown(in InputPacket raw) => Result = null;
    }
}
