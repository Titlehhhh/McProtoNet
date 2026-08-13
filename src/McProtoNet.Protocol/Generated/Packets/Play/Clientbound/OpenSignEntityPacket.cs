using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.open_sign_entity", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Location", "Position")]
[PacketField("IsFrontText", "bool", Group = "V763_Last", From = 763)]
public sealed partial record OpenSignEntityPacket(Position Location, OpenSignEntityPacket.V763_LastLayer? V763_Last = null) : IPacket<OpenSignEntityPacket>, IPacket
{
    public readonly record struct V763_LastLayer(bool IsFrontText);
    public static OpenSignEntityPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<OpenSignEntityPacket>(protocolVersion);
        if (protocolVersion <= 762)
        {
            var location = reader.ReadType<Position>(protocolVersion);
            return new OpenSignEntityPacket(location);
        }

        if (protocolVersion >= 763)
        {
            var location = reader.ReadType<Position>(protocolVersion);
            var isFrontText = reader.ReadBoolean();
            return new OpenSignEntityPacket(location, V763_Last: new V763_LastLayer(isFrontText));
        }

        throw new System.NotSupportedException($"OpenSignEntityPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<OpenSignEntityPacket>(protocolVersion);
        if (protocolVersion <= 762)
        {
            writer.WriteType<Position>(Location, protocolVersion);
            return;
        }

        if (protocolVersion >= 763)
        {
            var layer = V763_Last ?? throw new WrongLayerException("OpenSignEntityPacket", protocolVersion, "V763_Last");
            bool IsFrontText = layer.IsFrontText;
            writer.WriteType<Position>(Location, protocolVersion);
            writer.WriteBoolean(IsFrontText);
            return;
        }

        throw new System.NotSupportedException($"OpenSignEntityPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.open_sign_entity", "OpenSignEntity", PacketPhase.Play, PacketDirection.Clientbound, 58);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x2F;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x2E;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x2F;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x2C;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x2E;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x2D;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x31;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x32;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x34;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x36;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x35;
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
