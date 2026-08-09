using McProtoNet.Net;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;
public delegate void TrailingBytesHook(int packetId, int protocolVersion, long remainingBytes);
/// <summary>Generated dispatcher. Packets whose codegen is still stubbed are not
/// dispatched — they fall through to <c>Unknown</c> instead of throwing inside the
/// receive loop. Trailing bytes raise a hook, not an exception: the packet already
/// reached the visitor, but the spec is suspect.</summary>
public static partial class PacketFlow
{
    public static event TrailingBytesHook? OnTrailingBytes;
    public static void Dispatch<TVisitor>(in InputPacket raw, int protocolVersion, PacketPhase phase, PacketDirection dir, ref TVisitor visitor)
        where TVisitor : IPacketVisitor
    {
        if (!PacketRegistry.TryGetOrdinal(raw.Id, protocolVersion, phase, dir, out var ordinal))
        {
            visitor.Unknown(in raw);
            return;
        }

        var reader = new MinecraftPrimitiveReader(raw.Data);
        bool handled;
        switch (phase, dir)
        {
            case (PacketPhase.Handshaking, PacketDirection.Serverbound):
                handled = DispatchHandshakingServerbound(ordinal, ref reader, protocolVersion, ref visitor);
                break;
            case (PacketPhase.Status, PacketDirection.Clientbound):
                handled = DispatchStatusClientbound(ordinal, ref reader, protocolVersion, ref visitor);
                break;
            case (PacketPhase.Status, PacketDirection.Serverbound):
                handled = DispatchStatusServerbound(ordinal, ref reader, protocolVersion, ref visitor);
                break;
            case (PacketPhase.Login, PacketDirection.Clientbound):
                handled = DispatchLoginClientbound(ordinal, ref reader, protocolVersion, ref visitor);
                break;
            case (PacketPhase.Login, PacketDirection.Serverbound):
                handled = DispatchLoginServerbound(ordinal, ref reader, protocolVersion, ref visitor);
                break;
            case (PacketPhase.Configuration, PacketDirection.Clientbound):
                handled = DispatchConfigurationClientbound(ordinal, ref reader, protocolVersion, ref visitor);
                break;
            case (PacketPhase.Configuration, PacketDirection.Serverbound):
                handled = DispatchConfigurationServerbound(ordinal, ref reader, protocolVersion, ref visitor);
                break;
            case (PacketPhase.Play, PacketDirection.Clientbound):
                handled = DispatchPlayClientbound(ordinal, ref reader, protocolVersion, ref visitor);
                break;
            case (PacketPhase.Play, PacketDirection.Serverbound):
                handled = DispatchPlayServerbound(ordinal, ref reader, protocolVersion, ref visitor);
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

    private static bool DispatchHandshakingServerbound<TVisitor>(ushort ordinal, ref MinecraftPrimitiveReader reader, int protocolVersion, ref TVisitor visitor)
        where TVisitor : IPacketVisitor
    {
        switch (ordinal)
        {
            case 0:
                visitor.Visit(Packets.Handshaking.Serverbound.LegacyServerListPingPacket.Read(ref reader, protocolVersion));
                return true;
            case 1:
                visitor.Visit(Packets.Handshaking.Serverbound.SetProtocolPacket.Read(ref reader, protocolVersion));
                return true;
            default:
                return false;
        }
    }

    private static bool DispatchStatusClientbound<TVisitor>(ushort ordinal, ref MinecraftPrimitiveReader reader, int protocolVersion, ref TVisitor visitor)
        where TVisitor : IPacketVisitor
    {
        switch (ordinal)
        {
            case 0:
                visitor.Visit(Packets.Status.Clientbound.PongResponsePacket.Read(ref reader, protocolVersion));
                return true;
            case 1:
                visitor.Visit(Packets.Status.Clientbound.ServerInfoPacket.Read(ref reader, protocolVersion));
                return true;
            default:
                return false;
        }
    }

    private static bool DispatchStatusServerbound<TVisitor>(ushort ordinal, ref MinecraftPrimitiveReader reader, int protocolVersion, ref TVisitor visitor)
        where TVisitor : IPacketVisitor
    {
        switch (ordinal)
        {
            case 0:
                visitor.Visit(Packets.Status.Serverbound.PingRequestPacket.Read(ref reader, protocolVersion));
                return true;
            case 1:
                visitor.Visit(Packets.Status.Serverbound.PingStartPacket.Read(ref reader, protocolVersion));
                return true;
            default:
                return false;
        }
    }

    private static bool DispatchLoginClientbound<TVisitor>(ushort ordinal, ref MinecraftPrimitiveReader reader, int protocolVersion, ref TVisitor visitor)
        where TVisitor : IPacketVisitor
    {
        switch (ordinal)
        {
            case 0:
                visitor.Visit(Packets.Login.Clientbound.LoginCompressPacket.Read(ref reader, protocolVersion));
                return true;
            case 1:
                visitor.Visit(Packets.Login.Clientbound.LoginCookieRequestPacket.Read(ref reader, protocolVersion));
                return true;
            case 2:
                visitor.Visit(Packets.Login.Clientbound.LoginDisconnectPacket.Read(ref reader, protocolVersion));
                return true;
            case 3:
                visitor.Visit(Packets.Login.Clientbound.EncryptionRequestPacket.Read(ref reader, protocolVersion));
                return true;
            case 4:
                visitor.Visit(Packets.Login.Clientbound.LoginPluginRequestPacket.Read(ref reader, protocolVersion));
                return true;
            case 5:
                visitor.Visit(Packets.Login.Clientbound.LoginSuccessPacket.Read(ref reader, protocolVersion));
                return true;
            default:
                return false;
        }
    }

    private static bool DispatchLoginServerbound<TVisitor>(ushort ordinal, ref MinecraftPrimitiveReader reader, int protocolVersion, ref TVisitor visitor)
        where TVisitor : IPacketVisitor
    {
        switch (ordinal)
        {
            case 0:
                visitor.Visit(Packets.Login.Serverbound.LoginCookieResponsePacket.Read(ref reader, protocolVersion));
                return true;
            case 2:
                visitor.Visit(Packets.Login.Serverbound.LoginAcknowledgedPacket.Read(ref reader, protocolVersion));
                return true;
            case 3:
                visitor.Visit(Packets.Login.Serverbound.LoginPluginResponsePacket.Read(ref reader, protocolVersion));
                return true;
            case 4:
                visitor.Visit(Packets.Login.Serverbound.LoginStartPacket.Read(ref reader, protocolVersion));
                return true;
            default:
                return false;
        }
    }

    private static bool DispatchConfigurationClientbound<TVisitor>(ushort ordinal, ref MinecraftPrimitiveReader reader, int protocolVersion, ref TVisitor visitor)
        where TVisitor : IPacketVisitor
    {
        switch (ordinal)
        {
            case 0:
                visitor.Visit(Packets.Configuration.Clientbound.DisconnectPacket.Read(ref reader, protocolVersion));
                return true;
            case 1:
                visitor.Visit(Packets.Configuration.Clientbound.FinishConfigurationPacket.Read(ref reader, protocolVersion));
                return true;
            case 2:
                visitor.Visit(Packets.Configuration.Clientbound.KeepAlivePacket.Read(ref reader, protocolVersion));
                return true;
            case 3:
                visitor.Visit(Packets.Configuration.Clientbound.PingPacket.Read(ref reader, protocolVersion));
                return true;
            case 4:
                visitor.Visit(Packets.Configuration.Clientbound.SelectKnownPacksPacket.Read(ref reader, protocolVersion));
                return true;
            default:
                return false;
        }
    }

    private static bool DispatchConfigurationServerbound<TVisitor>(ushort ordinal, ref MinecraftPrimitiveReader reader, int protocolVersion, ref TVisitor visitor)
        where TVisitor : IPacketVisitor
    {
        switch (ordinal)
        {
            case 0:
                visitor.Visit(Packets.Configuration.Serverbound.FinishConfigurationPacket.Read(ref reader, protocolVersion));
                return true;
            case 1:
                visitor.Visit(Packets.Configuration.Serverbound.KeepAlivePacket.Read(ref reader, protocolVersion));
                return true;
            case 2:
                visitor.Visit(Packets.Configuration.Serverbound.PongPacket.Read(ref reader, protocolVersion));
                return true;
            case 3:
                visitor.Visit(Packets.Configuration.Serverbound.SelectKnownPacksPacket.Read(ref reader, protocolVersion));
                return true;
            case 4:
                visitor.Visit(Packets.Configuration.Serverbound.ClientInformationPacket.Read(ref reader, protocolVersion));
                return true;
            default:
                return false;
        }
    }

    private static bool DispatchPlayClientbound<TVisitor>(ushort ordinal, ref MinecraftPrimitiveReader reader, int protocolVersion, ref TVisitor visitor)
        where TVisitor : IPacketVisitor
    {
        switch (ordinal)
        {
            case 0:
                visitor.Visit(Packets.Play.Clientbound.DamageEventPacket.Read(ref reader, protocolVersion));
                return true;
            case 1:
                visitor.Visit(Packets.Play.Clientbound.EntityHeadRotationPacket.Read(ref reader, protocolVersion));
                return true;
            case 4:
                visitor.Visit(Packets.Play.Clientbound.HurtAnimationPacket.Read(ref reader, protocolVersion));
                return true;
            case 5:
                visitor.Visit(Packets.Play.Clientbound.KeepAlivePacket.Read(ref reader, protocolVersion));
                return true;
            case 7:
                visitor.Visit(Packets.Play.Clientbound.MoveMinecartPacket.Read(ref reader, protocolVersion));
                return true;
            case 8:
                visitor.Visit(Packets.Play.Clientbound.PlayerPositionPacket.Read(ref reader, protocolVersion));
                return true;
            case 9:
                visitor.Visit(Packets.Play.Clientbound.RespawnPacket.Read(ref reader, protocolVersion));
                return true;
            case 10:
                visitor.Visit(Packets.Play.Clientbound.SetCooldownPacket.Read(ref reader, protocolVersion));
                return true;
            case 11:
                visitor.Visit(Packets.Play.Clientbound.SetProjectilePowerPacket.Read(ref reader, protocolVersion));
                return true;
            case 12:
                visitor.Visit(Packets.Play.Clientbound.SpawnEntityPacket.Read(ref reader, protocolVersion));
                return true;
            case 13:
                visitor.Visit(Packets.Play.Clientbound.SpawnPositionPacket.Read(ref reader, protocolVersion));
                return true;
            case 15:
                visitor.Visit(Packets.Play.Clientbound.UnloadChunkPacket.Read(ref reader, protocolVersion));
                return true;
            case 16:
                visitor.Visit(Packets.Play.Clientbound.UpdateHealthPacket.Read(ref reader, protocolVersion));
                return true;
            case 17:
                visitor.Visit(Packets.Play.Clientbound.UpdateTimePacket.Read(ref reader, protocolVersion));
                return true;
            case 18:
                visitor.Visit(Packets.Play.Clientbound.UpdateViewDistancePacket.Read(ref reader, protocolVersion));
                return true;
            default:
                return false;
        }
    }

    private static bool DispatchPlayServerbound<TVisitor>(ushort ordinal, ref MinecraftPrimitiveReader reader, int protocolVersion, ref TVisitor visitor)
        where TVisitor : IPacketVisitor
    {
        switch (ordinal)
        {
            case 0:
                visitor.Visit(Packets.Play.Serverbound.KeepAlivePacket.Read(ref reader, protocolVersion));
                return true;
            case 1:
                visitor.Visit(Packets.Play.Serverbound.LockDifficultyPacket.Read(ref reader, protocolVersion));
                return true;
            case 2:
                visitor.Visit(Packets.Play.Serverbound.NameItemPacket.Read(ref reader, protocolVersion));
                return true;
            case 3:
                visitor.Visit(Packets.Play.Serverbound.SpectatePacket.Read(ref reader, protocolVersion));
                return true;
            case 4:
                visitor.Visit(Packets.Play.Serverbound.TeleportConfirmPacket.Read(ref reader, protocolVersion));
                return true;
            default:
                return false;
        }
    }
}
