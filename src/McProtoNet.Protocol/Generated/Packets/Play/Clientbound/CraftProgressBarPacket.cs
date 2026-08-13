using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.craft_progress_bar", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("WindowId", "int")]
[PacketField("Property", "int")]
[PacketField("Value", "int")]
public sealed partial record CraftProgressBarPacket(int WindowId, int Property, int Value) : IPacket<CraftProgressBarPacket>, IPacket
{
    public static CraftProgressBarPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CraftProgressBarPacket>(protocolVersion);
        if (protocolVersion <= 767)
        {
            var windowId = reader.ReadUnsignedByte();
            var property = reader.ReadSignedShort();
            var value = reader.ReadSignedShort();
            return new CraftProgressBarPacket(windowId, property, value);
        }

        if (protocolVersion >= 768)
        {
            var windowId = reader.ReadVarInt();
            var property = reader.ReadSignedShort();
            var value = reader.ReadSignedShort();
            return new CraftProgressBarPacket(windowId, property, value);
        }

        throw new System.NotSupportedException($"CraftProgressBarPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CraftProgressBarPacket>(protocolVersion);
        if (protocolVersion <= 767)
        {
            writer.WriteUnsignedByte((byte)WindowId);
            writer.WriteSignedShort((short)Property);
            writer.WriteSignedShort((short)Value);
            return;
        }

        if (protocolVersion >= 768)
        {
            writer.WriteVarInt(WindowId);
            writer.WriteSignedShort((short)Property);
            writer.WriteSignedShort((short)Value);
            return;
        }

        throw new System.NotSupportedException($"CraftProgressBarPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.craft_progress_bar", "CraftProgressBar", PacketPhase.Play, PacketDirection.Clientbound, 21);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x15;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x14;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x15;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 760)
        {
            id = 0x12;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x11;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x13;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 769)
        {
            id = 0x14;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x13;
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
