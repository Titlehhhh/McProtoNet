#nullable enable
using System;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;
public delegate void TrailingBytesHook(int packetId, int protocolVersion, long remainingBytes);
/// <summary>Generated dispatcher. Packets whose codegen is still stubbed are not
/// dispatched — they fall through to <c>Unknown</c> instead of throwing inside the
/// receive loop. Trailing bytes raise a hook, not an exception: the packet already
/// reached the visitor, but the spec is suspect.
/// Three doors onto the same table: <c>Dispatch</c> (throws on a malformed body),
/// <c>TryDispatch</c> (same visitor, a false + reason instead of the throw) and
/// <c>TryDecode</c> (visitor-free — hands back the decoded packet itself).</summary>
public static partial class PacketFlow
{
    public static event TrailingBytesHook? OnTrailingBytes;
    /// <summary>Raises <see cref = "OnTrailingBytes"/> for a caller that decoded the body
    /// itself. An event can only be raised inside the type that declares it, and the
    /// generated handlers decode without going through <see cref = "Dispatch"/>; the hook
    /// stays the one place a suspect spec is reported from.</summary>
    internal static void RaiseTrailingBytes(int packetId, int protocolVersion, long remainingBytes) => OnTrailingBytes?.Invoke(packetId, protocolVersion, remainingBytes);
    public static void Dispatch<TVisitor>(in IncomingPacket raw, int protocolVersion, PacketPhase phase, PacketDirection dir, ref TVisitor visitor)
        where TVisitor : IPacketVisitor
    {
        if (!PacketRegistry.TryGetOrdinal(raw.Id, protocolVersion, phase, dir, out var ordinal))
        {
            visitor.Unknown(in raw);
            return;
        }

        var reader = new MinecraftPrimitiveReader(raw.Body);
        bool handled;
        // The jump table is shared with the Try door, which must tell a failed body read
        // from an exception thrown by the visitor: the table lowers this flag once the
        // body is decoded, right before it calls the visitor. Dispatch converts nothing,
        // so here the flag is written and never read.
        bool reading = true;
        switch (phase, dir)
        {
            case (PacketPhase.Handshaking, PacketDirection.Serverbound):
                handled = DispatchHandshakingServerbound(ordinal, ref reader, protocolVersion, ref visitor, ref reading);
                break;
            case (PacketPhase.Status, PacketDirection.Clientbound):
                handled = DispatchStatusClientbound(ordinal, ref reader, protocolVersion, ref visitor, ref reading);
                break;
            case (PacketPhase.Status, PacketDirection.Serverbound):
                handled = DispatchStatusServerbound(ordinal, ref reader, protocolVersion, ref visitor, ref reading);
                break;
            case (PacketPhase.Login, PacketDirection.Clientbound):
                handled = DispatchLoginClientbound(ordinal, ref reader, protocolVersion, ref visitor, ref reading);
                break;
            case (PacketPhase.Login, PacketDirection.Serverbound):
                handled = DispatchLoginServerbound(ordinal, ref reader, protocolVersion, ref visitor, ref reading);
                break;
            case (PacketPhase.Configuration, PacketDirection.Clientbound):
                handled = DispatchConfigurationClientbound(ordinal, ref reader, protocolVersion, ref visitor, ref reading);
                break;
            case (PacketPhase.Configuration, PacketDirection.Serverbound):
                handled = DispatchConfigurationServerbound(ordinal, ref reader, protocolVersion, ref visitor, ref reading);
                break;
            case (PacketPhase.Play, PacketDirection.Clientbound):
                handled = DispatchPlayClientbound(ordinal, ref reader, protocolVersion, ref visitor, ref reading);
                break;
            case (PacketPhase.Play, PacketDirection.Serverbound):
                handled = DispatchPlayServerbound(ordinal, ref reader, protocolVersion, ref visitor, ref reading);
                break;
            default:
                handled = false;
                break;
        }

        if (!handled)
        {
            visitor.Unknown(in raw);
            return;
        }

        if (reader.RemainingCount != 0)
            OnTrailingBytes?.Invoke(raw.Id, protocolVersion, reader.RemainingCount);
    }

    /// <summary>Dispatch that survives a malformed body: returns false with
    /// <paramref name = "error"/> filled where <see cref = "Dispatch"/> would let the
    /// exception out. True means the packet reached the visitor — including the normal
    /// stream condition of an id this (phase, direction) has no mapping for, which
    /// reaches <c>Unknown</c> exactly as in <see cref = "Dispatch"/>. Trailing bytes stay
    /// a hook, not a failure. Only a failure of the body read is converted, and only the
    /// kinds a decoder may swallow (see <c>TryClassify</c>): cancellation, a stubbed
    /// decoder and out-of-memory still propagate. An exception thrown by the visitor
    /// itself is never converted — the table lowers <c>reading</c> before it calls the
    /// visitor, so the consumer's own bugs come out as themselves.</summary>
    public static bool TryDispatch<TVisitor>(in IncomingPacket raw, int protocolVersion, PacketPhase phase, PacketDirection direction, ref TVisitor visitor, out DecodeError error)
        where TVisitor : IPacketVisitor
    {
        error = DecodeError.None;
        if (!PacketRegistry.TryGetOrdinal(raw.Id, protocolVersion, phase, direction, out var ordinal))
        {
            visitor.Unknown(in raw);
            return true;
        }

        var reader = new MinecraftPrimitiveReader(raw.Body);
        bool handled;
        // True while the body is being read; the table lowers it right before it hands the
        // packet to the visitor. The filter below tests it, so an exception out of the
        // visitor is not mistaken for a malformed packet.
        bool reading = true;
        try
        {
            switch (phase, direction)
            {
                case (PacketPhase.Handshaking, PacketDirection.Serverbound):
                    handled = DispatchHandshakingServerbound(ordinal, ref reader, protocolVersion, ref visitor, ref reading);
                    break;
                case (PacketPhase.Status, PacketDirection.Clientbound):
                    handled = DispatchStatusClientbound(ordinal, ref reader, protocolVersion, ref visitor, ref reading);
                    break;
                case (PacketPhase.Status, PacketDirection.Serverbound):
                    handled = DispatchStatusServerbound(ordinal, ref reader, protocolVersion, ref visitor, ref reading);
                    break;
                case (PacketPhase.Login, PacketDirection.Clientbound):
                    handled = DispatchLoginClientbound(ordinal, ref reader, protocolVersion, ref visitor, ref reading);
                    break;
                case (PacketPhase.Login, PacketDirection.Serverbound):
                    handled = DispatchLoginServerbound(ordinal, ref reader, protocolVersion, ref visitor, ref reading);
                    break;
                case (PacketPhase.Configuration, PacketDirection.Clientbound):
                    handled = DispatchConfigurationClientbound(ordinal, ref reader, protocolVersion, ref visitor, ref reading);
                    break;
                case (PacketPhase.Configuration, PacketDirection.Serverbound):
                    handled = DispatchConfigurationServerbound(ordinal, ref reader, protocolVersion, ref visitor, ref reading);
                    break;
                case (PacketPhase.Play, PacketDirection.Clientbound):
                    handled = DispatchPlayClientbound(ordinal, ref reader, protocolVersion, ref visitor, ref reading);
                    break;
                case (PacketPhase.Play, PacketDirection.Serverbound):
                    handled = DispatchPlayServerbound(ordinal, ref reader, protocolVersion, ref visitor, ref reading);
                    break;
                default:
                    handled = false;
                    break;
            }
        }
        catch (Exception ex)when (reading && TryClassify(ex, out var reason))
        {
            error = reason;
            return false;
        }

        if (!handled)
        {
            visitor.Unknown(in raw);
            return true;
        }

        if (reader.RemainingCount != 0)
            OnTrailingBytes?.Invoke(raw.Id, protocolVersion, reader.RemainingCount);
        return true;
    }

    /// <summary>One raw packet in, one decoded packet out — no visitor to write.
    /// An id this (phase, direction) cannot map yields an <see cref = "UnknownPacket"/>
    /// and still returns true: an unmapped id is a normal stream condition, not an error.
    /// A malformed body returns false with <paramref name = "error"/> filled and
    /// <paramref name = "packet"/> null. The allocation-free hot path is
    /// <see cref = "Dispatch"/> / <see cref = "TryDispatch"/>; this door costs nothing
    /// extra either — packets are classes, so the capture is a reference, not a box.</summary>
    public static bool TryDecode(in IncomingPacket raw, int protocolVersion, PacketPhase phase, PacketDirection direction, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out IPacket? packet, out DecodeError error)
    {
        var capture = new Capture(phase, direction);
        if (!TryDispatch(in raw, protocolVersion, phase, direction, ref capture, out error))
        {
            packet = null;
            return false;
        }

        packet = capture.Result!;
        return true;
    }

    /// <summary>Keeps the decoded packet as <see cref = "IPacket"/>. The assignment is a
    /// reference conversion (every packet is a class that implements it), so there is no
    /// boxing and no adapter object — the same jump table, one field write.</summary>
    private struct Capture : IPacketVisitor
    {
        private readonly PacketPhase _phase;
        private readonly PacketDirection _direction;
        public IPacket? Result;
        public Capture(PacketPhase phase, PacketDirection direction)
        {
            _phase = phase;
            _direction = direction;
            Result = null;
        }

        public void Visit<T>(T packet)
            where T : class, IPacket<T> => Result = (IPacket)packet;
        public void Unknown(in IncomingPacket raw) => Result = new UnknownPacket(raw.Id, _phase, _direction);
    }

    /// <summary>Maps an exception raised while reading a packet body onto a
    /// <see cref = "DecodeError"/>. Returns false for exceptions a decoder must never
    /// swallow — used as an exception filter, so those propagate without unwinding.
    /// <para>
    /// <c>ArgumentException</c> is deliberately NOT on the propagate list. Bytes off the
    /// wire reach it: a compound with a duplicate key ends in <c>Dictionary.Add</c> inside
    /// <c>NbtCompound.Add</c>, and an unnamed tag in a compound throws there too. Those are
    /// data errors, so they are <c>Malformed</c>. Only an exception the caller's own code
    /// raised should escape a Try door, and that case is handled before the filter runs:
    /// the jump table lowers <c>reading</c> before it calls the visitor.
    /// </para></summary>
    private static bool TryClassify(Exception ex, out DecodeError error)
    {
        switch (ex)
        {
            case ProtocolNotSupportException _:
            case NotSupportedException _:
                error = DecodeError.UnsupportedVersion;
                return true;
            case OperationCanceledException _:
            case NotImplementedException _:
            case OutOfMemoryException _:
                error = DecodeError.None;
                return false;
            default:
                error = DecodeError.Malformed;
                return true;
        }
    }

    /// <summary>One ordinal, one read, one constrained call. <paramref name = "reading"/>
    /// goes false between the two: above it the exception is the packet's fault, below it
    /// the visitor's. Only the Try door reads it.</summary>
    private static bool DispatchHandshakingServerbound<TVisitor>(ushort ordinal, ref MinecraftPrimitiveReader reader, int protocolVersion, ref TVisitor visitor, ref bool reading)
        where TVisitor : IPacketVisitor
    {
        switch (ordinal)
        {
            case 0:
            {
                var packet = Packets.Handshaking.Serverbound.LegacyServerListPingPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 1:
            {
                var packet = Packets.Handshaking.Serverbound.SetProtocolPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            default:
                return false;
        }
    }

    /// <summary>One ordinal, one read, one constrained call. <paramref name = "reading"/>
    /// goes false between the two: above it the exception is the packet's fault, below it
    /// the visitor's. Only the Try door reads it.</summary>
    private static bool DispatchStatusClientbound<TVisitor>(ushort ordinal, ref MinecraftPrimitiveReader reader, int protocolVersion, ref TVisitor visitor, ref bool reading)
        where TVisitor : IPacketVisitor
    {
        switch (ordinal)
        {
            case 0:
            {
                var packet = Packets.Status.Clientbound.PongResponsePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 1:
            {
                var packet = Packets.Status.Clientbound.ServerInfoPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            default:
                return false;
        }
    }

    /// <summary>One ordinal, one read, one constrained call. <paramref name = "reading"/>
    /// goes false between the two: above it the exception is the packet's fault, below it
    /// the visitor's. Only the Try door reads it.</summary>
    private static bool DispatchStatusServerbound<TVisitor>(ushort ordinal, ref MinecraftPrimitiveReader reader, int protocolVersion, ref TVisitor visitor, ref bool reading)
        where TVisitor : IPacketVisitor
    {
        switch (ordinal)
        {
            case 0:
            {
                var packet = Packets.Status.Serverbound.PingRequestPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 1:
            {
                var packet = Packets.Status.Serverbound.PingStartPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            default:
                return false;
        }
    }

    /// <summary>One ordinal, one read, one constrained call. <paramref name = "reading"/>
    /// goes false between the two: above it the exception is the packet's fault, below it
    /// the visitor's. Only the Try door reads it.</summary>
    private static bool DispatchLoginClientbound<TVisitor>(ushort ordinal, ref MinecraftPrimitiveReader reader, int protocolVersion, ref TVisitor visitor, ref bool reading)
        where TVisitor : IPacketVisitor
    {
        switch (ordinal)
        {
            case 0:
            {
                var packet = Packets.Login.Clientbound.LoginCompressPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 1:
            {
                var packet = Packets.Login.Clientbound.LoginCookieRequestPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 2:
            {
                var packet = Packets.Login.Clientbound.LoginDisconnectPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 3:
            {
                var packet = Packets.Login.Clientbound.EncryptionRequestPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 4:
            {
                var packet = Packets.Login.Clientbound.LoginPluginRequestPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 5:
            {
                var packet = Packets.Login.Clientbound.LoginSuccessPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            default:
                return false;
        }
    }

    /// <summary>One ordinal, one read, one constrained call. <paramref name = "reading"/>
    /// goes false between the two: above it the exception is the packet's fault, below it
    /// the visitor's. Only the Try door reads it.</summary>
    private static bool DispatchLoginServerbound<TVisitor>(ushort ordinal, ref MinecraftPrimitiveReader reader, int protocolVersion, ref TVisitor visitor, ref bool reading)
        where TVisitor : IPacketVisitor
    {
        switch (ordinal)
        {
            case 0:
            {
                var packet = Packets.Login.Serverbound.LoginCookieResponsePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 2:
            {
                var packet = Packets.Login.Serverbound.LoginAcknowledgedPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 3:
            {
                var packet = Packets.Login.Serverbound.LoginPluginResponsePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 4:
            {
                var packet = Packets.Login.Serverbound.LoginStartPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            default:
                return false;
        }
    }

    /// <summary>One ordinal, one read, one constrained call. <paramref name = "reading"/>
    /// goes false between the two: above it the exception is the packet's fault, below it
    /// the visitor's. Only the Try door reads it.</summary>
    private static bool DispatchConfigurationClientbound<TVisitor>(ushort ordinal, ref MinecraftPrimitiveReader reader, int protocolVersion, ref TVisitor visitor, ref bool reading)
        where TVisitor : IPacketVisitor
    {
        switch (ordinal)
        {
            case 0:
            {
                var packet = Packets.Configuration.Clientbound.AddResourcePackPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 1:
            {
                var packet = Packets.Configuration.Clientbound.ClearDialogPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 2:
            {
                var packet = Packets.Configuration.Clientbound.CodeOfConductPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 3:
            {
                var packet = Packets.Configuration.Clientbound.CookieRequestPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 4:
            {
                var packet = Packets.Configuration.Clientbound.CustomPayloadPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 5:
            {
                var packet = Packets.Configuration.Clientbound.CustomReportDetailsPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 6:
            {
                var packet = Packets.Configuration.Clientbound.DisconnectPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 7:
            {
                var packet = Packets.Configuration.Clientbound.FeatureFlagsPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 8:
            {
                var packet = Packets.Configuration.Clientbound.FinishConfigurationPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 9:
            {
                var packet = Packets.Configuration.Clientbound.KeepAlivePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 10:
            {
                var packet = Packets.Configuration.Clientbound.PingPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 11:
            {
                var packet = Packets.Configuration.Clientbound.RemoveResourcePackPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 12:
            {
                var packet = Packets.Configuration.Clientbound.ResetChatPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 13:
            {
                var packet = Packets.Configuration.Clientbound.ResourcePackSendPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 14:
            {
                var packet = Packets.Configuration.Clientbound.SelectKnownPacksPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 15:
            {
                var packet = Packets.Configuration.Clientbound.ShowDialogPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 16:
            {
                var packet = Packets.Configuration.Clientbound.StoreCookiePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 17:
            {
                var packet = Packets.Configuration.Clientbound.TagsPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 18:
            {
                var packet = Packets.Configuration.Clientbound.TransferPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            default:
                return false;
        }
    }

    /// <summary>One ordinal, one read, one constrained call. <paramref name = "reading"/>
    /// goes false between the two: above it the exception is the packet's fault, below it
    /// the visitor's. Only the Try door reads it.</summary>
    private static bool DispatchConfigurationServerbound<TVisitor>(ushort ordinal, ref MinecraftPrimitiveReader reader, int protocolVersion, ref TVisitor visitor, ref bool reading)
        where TVisitor : IPacketVisitor
    {
        switch (ordinal)
        {
            case 0:
            {
                var packet = Packets.Configuration.Serverbound.AcceptCodeOfConductPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 1:
            {
                var packet = Packets.Configuration.Serverbound.CookieResponsePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 2:
            {
                var packet = Packets.Configuration.Serverbound.CustomClickActionPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 3:
            {
                var packet = Packets.Configuration.Serverbound.CustomPayloadPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 4:
            {
                var packet = Packets.Configuration.Serverbound.CustomReportDetailsPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 5:
            {
                var packet = Packets.Configuration.Serverbound.FinishConfigurationPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 6:
            {
                var packet = Packets.Configuration.Serverbound.KeepAlivePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 7:
            {
                var packet = Packets.Configuration.Serverbound.PongPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 8:
            {
                var packet = Packets.Configuration.Serverbound.ResourcePackReceivePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 9:
            {
                var packet = Packets.Configuration.Serverbound.SelectKnownPacksPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 10:
            {
                var packet = Packets.Configuration.Serverbound.ClientInformationPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            default:
                return false;
        }
    }

    /// <summary>One ordinal, one read, one constrained call. <paramref name = "reading"/>
    /// goes false between the two: above it the exception is the packet's fault, below it
    /// the visitor's. Only the Try door reads it.</summary>
    private static bool DispatchPlayClientbound<TVisitor>(ushort ordinal, ref MinecraftPrimitiveReader reader, int protocolVersion, ref TVisitor visitor, ref bool reading)
        where TVisitor : IPacketVisitor
    {
        switch (ordinal)
        {
            case 0:
            {
                var packet = Packets.Play.Clientbound.AbilitiesPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 1:
            {
                var packet = Packets.Play.Clientbound.AcknowledgePlayerDiggingPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 2:
            {
                var packet = Packets.Play.Clientbound.ActionBarPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 3:
            {
                var packet = Packets.Play.Clientbound.AddResourcePackPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 4:
            {
                var packet = Packets.Play.Clientbound.AnimationPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 5:
            {
                var packet = Packets.Play.Clientbound.AttachEntityPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 6:
            {
                var packet = Packets.Play.Clientbound.BlockActionPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 7:
            {
                var packet = Packets.Play.Clientbound.BlockBreakAnimationPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 8:
            {
                var packet = Packets.Play.Clientbound.BlockChangePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 9:
            {
                var packet = Packets.Play.Clientbound.CameraPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 10:
            {
                var packet = Packets.Play.Clientbound.ChatPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 11:
            {
                var packet = Packets.Play.Clientbound.ChatPreviewPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 12:
            {
                var packet = Packets.Play.Clientbound.ChatSuggestionsPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 13:
            {
                var packet = Packets.Play.Clientbound.ChunkBatchFinishedPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 14:
            {
                var packet = Packets.Play.Clientbound.ChunkBatchStartPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 15:
            {
                var packet = Packets.Play.Clientbound.ChunkBiomesPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 16:
            {
                var packet = Packets.Play.Clientbound.ClearDialogPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 17:
            {
                var packet = Packets.Play.Clientbound.ClearTitlesPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 18:
            {
                var packet = Packets.Play.Clientbound.CloseWindowPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 19:
            {
                var packet = Packets.Play.Clientbound.CollectPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 20:
            {
                var packet = Packets.Play.Clientbound.CookieRequestPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 21:
            {
                var packet = Packets.Play.Clientbound.CraftProgressBarPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 22:
            {
                var packet = Packets.Play.Clientbound.CustomPayloadPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 23:
            {
                var packet = Packets.Play.Clientbound.CustomReportDetailsPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 24:
            {
                var packet = Packets.Play.Clientbound.DamageEventPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 25:
            {
                var packet = Packets.Play.Clientbound.DeathCombatEventPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 26:
            {
                var packet = Packets.Play.Clientbound.DebugSamplePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 27:
            {
                var packet = Packets.Play.Clientbound.DestroyEntityPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 28:
            {
                var packet = Packets.Play.Clientbound.DifficultyPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 29:
            {
                var packet = Packets.Play.Clientbound.EndCombatEventPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 30:
            {
                var packet = Packets.Play.Clientbound.EnterCombatEventPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 31:
            {
                var packet = Packets.Play.Clientbound.EntityPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 32:
            {
                var packet = Packets.Play.Clientbound.EntityDestroyPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 33:
            {
                var packet = Packets.Play.Clientbound.EntityHeadRotationPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 34:
            {
                var packet = Packets.Play.Clientbound.EntityLookPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 36:
            {
                var packet = Packets.Play.Clientbound.EntityMoveLookPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 37:
            {
                var packet = Packets.Play.Clientbound.EntityStatusPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 38:
            {
                var packet = Packets.Play.Clientbound.EntityTeleportPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 39:
            {
                var packet = Packets.Play.Clientbound.EntityUpdateAttributesPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 41:
            {
                var packet = Packets.Play.Clientbound.ExperiencePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 43:
            {
                var packet = Packets.Play.Clientbound.FeatureFlagsPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 44:
            {
                var packet = Packets.Play.Clientbound.GameRuleValuesPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 45:
            {
                var packet = Packets.Play.Clientbound.GameStateChangePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 46:
            {
                var packet = Packets.Play.Clientbound.GameTestHighlightPosPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 47:
            {
                var packet = Packets.Play.Clientbound.HeldItemSlotPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 48:
            {
                var packet = Packets.Play.Clientbound.HurtAnimationPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 49:
            {
                var packet = Packets.Play.Clientbound.InitializeWorldBorderPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 50:
            {
                var packet = Packets.Play.Clientbound.KeepAlivePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 51:
            {
                var packet = Packets.Play.Clientbound.KickDisconnectPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 52:
            {
                var packet = Packets.Play.Clientbound.LoginPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 53:
            {
                var packet = Packets.Play.Clientbound.LowDiskSpaceWarningPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 55:
            {
                var packet = Packets.Play.Clientbound.MessageHeaderPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 56:
            {
                var packet = Packets.Play.Clientbound.MoveMinecartPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 57:
            {
                var packet = Packets.Play.Clientbound.NamedEntitySpawnPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 58:
            {
                var packet = Packets.Play.Clientbound.NamedSoundEffectPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 59:
            {
                var packet = Packets.Play.Clientbound.NbtQueryResponsePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 60:
            {
                var packet = Packets.Play.Clientbound.OpenBookPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 61:
            {
                var packet = Packets.Play.Clientbound.OpenHorseWindowPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 62:
            {
                var packet = Packets.Play.Clientbound.OpenSignEntityPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 63:
            {
                var packet = Packets.Play.Clientbound.OpenWindowPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 64:
            {
                var packet = Packets.Play.Clientbound.PingPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 65:
            {
                var packet = Packets.Play.Clientbound.PingResponsePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 66:
            {
                var packet = Packets.Play.Clientbound.PlayerChatPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 67:
            {
                var packet = Packets.Play.Clientbound.PlayerRemovePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 68:
            {
                var packet = Packets.Play.Clientbound.PlayerRotationPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 69:
            {
                var packet = Packets.Play.Clientbound.PlayerlistHeaderPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 70:
            {
                var packet = Packets.Play.Clientbound.PlayerPositionPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 71:
            {
                var packet = Packets.Play.Clientbound.ProfilelessChatPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 72:
            {
                var packet = Packets.Play.Clientbound.RecipeBookRemovePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 73:
            {
                var packet = Packets.Play.Clientbound.RelEntityMovePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 74:
            {
                var packet = Packets.Play.Clientbound.RemoveEntityEffectPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 75:
            {
                var packet = Packets.Play.Clientbound.RemoveResourcePackPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 76:
            {
                var packet = Packets.Play.Clientbound.ResetScorePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 77:
            {
                var packet = Packets.Play.Clientbound.ResourcePackSendPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 78:
            {
                var packet = Packets.Play.Clientbound.RespawnPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 79:
            {
                var packet = Packets.Play.Clientbound.ScoreboardDisplayObjectivePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 80:
            {
                var packet = Packets.Play.Clientbound.SelectAdvancementTabPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 81:
            {
                var packet = Packets.Play.Clientbound.ServerDataPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 82:
            {
                var packet = Packets.Play.Clientbound.SetCooldownPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 83:
            {
                var packet = Packets.Play.Clientbound.SetPassengersPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 84:
            {
                var packet = Packets.Play.Clientbound.SetProjectilePowerPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 85:
            {
                var packet = Packets.Play.Clientbound.SetTickingStatePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 86:
            {
                var packet = Packets.Play.Clientbound.SetTitleSubtitlePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 87:
            {
                var packet = Packets.Play.Clientbound.SetTitleTextPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 88:
            {
                var packet = Packets.Play.Clientbound.SetTitleTimePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 89:
            {
                var packet = Packets.Play.Clientbound.ShouldDisplayChatPreviewPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 90:
            {
                var packet = Packets.Play.Clientbound.SimulationDistancePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 92:
            {
                var packet = Packets.Play.Clientbound.SpawnEntityExperienceOrbPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 93:
            {
                var packet = Packets.Play.Clientbound.SpawnEntityLivingPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 94:
            {
                var packet = Packets.Play.Clientbound.SpawnEntityPaintingPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 95:
            {
                var packet = Packets.Play.Clientbound.SpawnPositionPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 96:
            {
                var packet = Packets.Play.Clientbound.StartConfigurationPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 97:
            {
                var packet = Packets.Play.Clientbound.StatisticsPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 98:
            {
                var packet = Packets.Play.Clientbound.StepTickPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 99:
            {
                var packet = Packets.Play.Clientbound.StoreCookiePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 100:
            {
                var packet = Packets.Play.Clientbound.SyncEntityPositionPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 101:
            {
                var packet = Packets.Play.Clientbound.SystemChatPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 102:
            {
                var packet = Packets.Play.Clientbound.TabCompletePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 103:
            {
                var packet = Packets.Play.Clientbound.TagsPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 104:
            {
                var packet = Packets.Play.Clientbound.TeamsPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 105:
            {
                var packet = Packets.Play.Clientbound.TestInstanceBlockStatusPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 106:
            {
                var packet = Packets.Play.Clientbound.TileEntityDataPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 107:
            {
                var packet = Packets.Play.Clientbound.TransactionPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 108:
            {
                var packet = Packets.Play.Clientbound.TransferPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 109:
            {
                var packet = Packets.Play.Clientbound.UnloadChunkPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 110:
            {
                var packet = Packets.Play.Clientbound.UpdateHealthPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 112:
            {
                var packet = Packets.Play.Clientbound.UpdateTimePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 113:
            {
                var packet = Packets.Play.Clientbound.UpdateViewDistancePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 114:
            {
                var packet = Packets.Play.Clientbound.UpdateViewPositionPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 115:
            {
                var packet = Packets.Play.Clientbound.VehicleMovePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 116:
            {
                var packet = Packets.Play.Clientbound.WorldBorderCenterPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 117:
            {
                var packet = Packets.Play.Clientbound.WorldBorderLerpSizePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 118:
            {
                var packet = Packets.Play.Clientbound.WorldBorderSizePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 119:
            {
                var packet = Packets.Play.Clientbound.WorldBorderWarningDelayPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 120:
            {
                var packet = Packets.Play.Clientbound.WorldBorderWarningReachPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 121:
            {
                var packet = Packets.Play.Clientbound.WorldEventPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            default:
                return false;
        }
    }

    /// <summary>One ordinal, one read, one constrained call. <paramref name = "reading"/>
    /// goes false between the two: above it the exception is the packet's fault, below it
    /// the visitor's. Only the Try door reads it.</summary>
    private static bool DispatchPlayServerbound<TVisitor>(ushort ordinal, ref MinecraftPrimitiveReader reader, int protocolVersion, ref TVisitor visitor, ref bool reading)
        where TVisitor : IPacketVisitor
    {
        switch (ordinal)
        {
            case 0:
            {
                var packet = Packets.Play.Serverbound.AbilitiesPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 1:
            {
                var packet = Packets.Play.Serverbound.ArmAnimationPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 2:
            {
                var packet = Packets.Play.Serverbound.AttackPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 3:
            {
                var packet = Packets.Play.Serverbound.BlockDigPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 4:
            {
                var packet = Packets.Play.Serverbound.BlockPlacePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 5:
            {
                var packet = Packets.Play.Serverbound.ChangeGamemodePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 6:
            {
                var packet = Packets.Play.Serverbound.ChatPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 7:
            {
                var packet = Packets.Play.Serverbound.ChatCommandSignedPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 8:
            {
                var packet = Packets.Play.Serverbound.ChatMessagePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 9:
            {
                var packet = Packets.Play.Serverbound.ChatPreviewPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 10:
            {
                var packet = Packets.Play.Serverbound.ChatSessionUpdatePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 11:
            {
                var packet = Packets.Play.Serverbound.ChunkBatchReceivedPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 12:
            {
                var packet = Packets.Play.Serverbound.ClientCommandPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 13:
            {
                var packet = Packets.Play.Serverbound.CloseWindowPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 14:
            {
                var packet = Packets.Play.Serverbound.ConfigurationAcknowledgedPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 15:
            {
                var packet = Packets.Play.Serverbound.CookieResponsePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 16:
            {
                var packet = Packets.Play.Serverbound.CraftRecipeRequestPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 17:
            {
                var packet = Packets.Play.Serverbound.CustomClickActionPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 18:
            {
                var packet = Packets.Play.Serverbound.CustomPayloadPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 19:
            {
                var packet = Packets.Play.Serverbound.DebugSampleSubscriptionPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 20:
            {
                var packet = Packets.Play.Serverbound.DisplayedRecipePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 21:
            {
                var packet = Packets.Play.Serverbound.EnchantItemPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 22:
            {
                var packet = Packets.Play.Serverbound.EntityActionPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 23:
            {
                var packet = Packets.Play.Serverbound.FlyingPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 24:
            {
                var packet = Packets.Play.Serverbound.GenerateStructurePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 25:
            {
                var packet = Packets.Play.Serverbound.HeldItemSlotPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 26:
            {
                var packet = Packets.Play.Serverbound.KeepAlivePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 27:
            {
                var packet = Packets.Play.Serverbound.LockDifficultyPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 28:
            {
                var packet = Packets.Play.Serverbound.LookPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 29:
            {
                var packet = Packets.Play.Serverbound.NameItemPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 30:
            {
                var packet = Packets.Play.Serverbound.PickItemPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 31:
            {
                var packet = Packets.Play.Serverbound.PickItemFromBlockPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 32:
            {
                var packet = Packets.Play.Serverbound.PickItemFromEntityPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 33:
            {
                var packet = Packets.Play.Serverbound.PingRequestPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 34:
            {
                var packet = Packets.Play.Serverbound.PlayerInputPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 35:
            {
                var packet = Packets.Play.Serverbound.PlayerLoadedPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 36:
            {
                var packet = Packets.Play.Serverbound.PongPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 37:
            {
                var packet = Packets.Play.Serverbound.PositionPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 38:
            {
                var packet = Packets.Play.Serverbound.PositionLookPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 39:
            {
                var packet = Packets.Play.Serverbound.QueryBlockNbtPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 40:
            {
                var packet = Packets.Play.Serverbound.QueryEntityNbtPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 41:
            {
                var packet = Packets.Play.Serverbound.RecipeBookPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 42:
            {
                var packet = Packets.Play.Serverbound.ResourcePackReceivePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 43:
            {
                var packet = Packets.Play.Serverbound.SelectBundleItemPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 44:
            {
                var packet = Packets.Play.Serverbound.SelectTradePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 45:
            {
                var packet = Packets.Play.Serverbound.SetBeaconEffectPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 46:
            {
                var packet = Packets.Play.Serverbound.SetDifficultyPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 47:
            {
                var packet = Packets.Play.Serverbound.SetGameRulePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 48:
            {
                var packet = Packets.Play.Serverbound.SetSlotStatePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 49:
            {
                var packet = Packets.Play.Serverbound.SetTestBlockPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 50:
            {
                var packet = Packets.Play.Serverbound.SpectatePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 51:
            {
                var packet = Packets.Play.Serverbound.SpectateEntityPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 52:
            {
                var packet = Packets.Play.Serverbound.SteerBoatPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 53:
            {
                var packet = Packets.Play.Serverbound.SteerVehiclePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 54:
            {
                var packet = Packets.Play.Serverbound.TabCompletePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 55:
            {
                var packet = Packets.Play.Serverbound.TeleportConfirmPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 56:
            {
                var packet = Packets.Play.Serverbound.TickEndPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 57:
            {
                var packet = Packets.Play.Serverbound.TransactionPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 58:
            {
                var packet = Packets.Play.Serverbound.UpdateCommandBlockPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 59:
            {
                var packet = Packets.Play.Serverbound.UpdateCommandBlockMinecartPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 60:
            {
                var packet = Packets.Play.Serverbound.UpdateJigsawBlockPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 61:
            {
                var packet = Packets.Play.Serverbound.UpdateSignPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 62:
            {
                var packet = Packets.Play.Serverbound.UpdateStructureBlockPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 64:
            {
                var packet = Packets.Play.Serverbound.UseItemPacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            case 65:
            {
                var packet = Packets.Play.Serverbound.VehicleMovePacket.Read(ref reader, protocolVersion);
                reading = false;
                visitor.Visit(packet);
                return true;
            }

            default:
                return false;
        }
    }
}
