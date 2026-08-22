using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.spectate", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Target", "Guid")]
public sealed partial record SpectatePacket(Guid Target) : IPacket<SpectatePacket>, IPacket
{
    public static SpectatePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpectatePacket>(protocolVersion);
        var target = reader.ReadUUID();
        return new SpectatePacket(target);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpectatePacket>(protocolVersion);
        writer.WriteUUID(Target);
    }

    public static PacketIdentity Identity => new("play.toServer.spectate", "Spectate", PacketPhase.Play, PacketDirection.Serverbound, 50);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x2C;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 758)
        {
            id = 0x2D;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x2F;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 763)
        {
            id = 0x30;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x33;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x34;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x37;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 768)
        {
            id = 0x39;
            return true;
        }

        if (protocolVersion >= 769 && protocolVersion <= 769)
        {
            id = 0x3B;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 774)
        {
            id = 0x3D;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x40;
            return true;
        }

        id = 0;
        return false;
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (TryGetPacketId(protocolVersion, out var id))
            return id;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
