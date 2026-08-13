using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.open_horse_window", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("WindowId", "int")]
[PacketField("NbSlots", "int")]
[PacketField("EntityId", "int")]
public sealed partial record OpenHorseWindowPacket(int WindowId, int NbSlots, int EntityId) : IPacket<OpenHorseWindowPacket>, IPacket
{
    public static OpenHorseWindowPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<OpenHorseWindowPacket>(protocolVersion);
        if (protocolVersion <= 767)
        {
            var windowId = reader.ReadUnsignedByte();
            var nbSlots = reader.ReadVarInt();
            var entityId = reader.ReadSignedInt();
            return new OpenHorseWindowPacket(windowId, nbSlots, entityId);
        }

        if (protocolVersion >= 768)
        {
            var windowId = reader.ReadVarInt();
            var nbSlots = reader.ReadVarInt();
            var entityId = reader.ReadSignedInt();
            return new OpenHorseWindowPacket(windowId, nbSlots, entityId);
        }

        throw new System.NotSupportedException($"OpenHorseWindowPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<OpenHorseWindowPacket>(protocolVersion);
        if (protocolVersion <= 767)
        {
            writer.WriteUnsignedByte((byte)WindowId);
            writer.WriteVarInt(NbSlots);
            writer.WriteSignedInt(EntityId);
            return;
        }

        if (protocolVersion >= 768)
        {
            writer.WriteVarInt(WindowId);
            writer.WriteVarInt(NbSlots);
            writer.WriteSignedInt(EntityId);
            return;
        }

        throw new System.NotSupportedException($"OpenHorseWindowPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.open_horse_window", "OpenHorseWindow", PacketPhase.Play, PacketDirection.Clientbound, 57);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x1F;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x1E;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x1F;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x1C;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x1E;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x1D;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x20;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x21;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x23;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x24;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x23;
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
