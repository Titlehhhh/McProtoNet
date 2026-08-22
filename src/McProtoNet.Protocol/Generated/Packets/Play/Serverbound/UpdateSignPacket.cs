using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.update_sign", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Location", "Position")]
[PacketField("Text1", "string")]
[PacketField("Text2", "string")]
[PacketField("Text3", "string")]
[PacketField("Text4", "string")]
[PacketField("IsFrontText", "bool", Group = "V763_Last", From = 763)]
public sealed partial record UpdateSignPacket(Position Location, string Text1, string Text2, string Text3, string Text4, UpdateSignPacket.V763_LastLayer? V763_Last = null) : IPacket<UpdateSignPacket>, IPacket
{
    public readonly record struct V763_LastLayer(bool IsFrontText);
    public static UpdateSignPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateSignPacket>(protocolVersion);
        if (protocolVersion <= 762)
        {
            var location = reader.ReadType<Position>(protocolVersion);
            var text1 = reader.ReadString();
            var text2 = reader.ReadString();
            var text3 = reader.ReadString();
            var text4 = reader.ReadString();
            return new UpdateSignPacket(location, text1, text2, text3, text4);
        }

        if (protocolVersion >= 763)
        {
            var location = reader.ReadType<Position>(protocolVersion);
            var isFrontText = reader.ReadBoolean();
            var text1 = reader.ReadString();
            var text2 = reader.ReadString();
            var text3 = reader.ReadString();
            var text4 = reader.ReadString();
            return new UpdateSignPacket(location, text1, text2, text3, text4, V763_Last: new V763_LastLayer(isFrontText));
        }

        throw new System.NotSupportedException($"UpdateSignPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateSignPacket>(protocolVersion);
        if (protocolVersion <= 762)
        {
            writer.WriteType<Position>(Location, protocolVersion);
            writer.WriteString(Text1);
            writer.WriteString(Text2);
            writer.WriteString(Text3);
            writer.WriteString(Text4);
            return;
        }

        if (protocolVersion >= 763)
        {
            var layer = V763_Last ?? throw new WrongLayerException("UpdateSignPacket", protocolVersion, "V763_Last");
            bool IsFrontText = layer.IsFrontText;
            writer.WriteType<Position>(Location, protocolVersion);
            writer.WriteBoolean(IsFrontText);
            writer.WriteString(Text1);
            writer.WriteString(Text2);
            writer.WriteString(Text3);
            writer.WriteString(Text4);
            return;
        }

        throw new System.NotSupportedException($"UpdateSignPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toServer.update_sign", "UpdateSign", PacketPhase.Play, PacketDirection.Serverbound, 61);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x2A;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 758)
        {
            id = 0x2B;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x2D;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 763)
        {
            id = 0x2E;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x31;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x32;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x35;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 768)
        {
            id = 0x37;
            return true;
        }

        if (protocolVersion >= 769 && protocolVersion <= 769)
        {
            id = 0x39;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 770)
        {
            id = 0x3A;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x3B;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x3D;
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
