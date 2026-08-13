using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.use_item", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Hand", "int")]
[PacketField("Sequence", "int", Group = "V759_766", From = 759, To = 766)]
[PacketField("Sequence", "int", Group = "V767_Last", From = 767)]
[PacketField("Rotation", "Vec2f", Group = "V767_Last", From = 767)]
public sealed partial record UseItemPacket(int Hand, UseItemPacket.V759_766Layer? V759_766 = null, UseItemPacket.V767_LastLayer? V767_Last = null) : IPacket<UseItemPacket>, IPacket
{
    public readonly record struct V759_766Layer(int Sequence);
    public readonly record struct V767_LastLayer(int Sequence, Vec2f Rotation);
    public static UseItemPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UseItemPacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            var hand = reader.ReadVarInt();
            return new UseItemPacket(hand);
        }

        if (protocolVersion >= 759 && protocolVersion <= 766)
        {
            var hand = reader.ReadVarInt();
            var sequence = reader.ReadVarInt();
            return new UseItemPacket(hand, V759_766: new V759_766Layer(sequence));
        }

        if (protocolVersion >= 767)
        {
            var hand = reader.ReadVarInt();
            var sequence = reader.ReadVarInt();
            var rotation = reader.ReadType<Vec2f>(protocolVersion);
            return new UseItemPacket(hand, V767_Last: new V767_LastLayer(sequence, rotation));
        }

        throw new System.NotSupportedException($"UseItemPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UseItemPacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            writer.WriteVarInt(Hand);
            return;
        }

        if (protocolVersion >= 759 && protocolVersion <= 766)
        {
            var layer = V759_766 ?? throw new WrongLayerException("UseItemPacket", protocolVersion, "V759_766");
            int Sequence = layer.Sequence;
            writer.WriteVarInt(Hand);
            writer.WriteVarInt(Sequence);
            return;
        }

        if (protocolVersion >= 767)
        {
            var layer = V767_Last ?? throw new WrongLayerException("UseItemPacket", protocolVersion, "V767_Last");
            int Sequence = layer.Sequence;
            Vec2f Rotation = layer.Rotation;
            writer.WriteVarInt(Hand);
            writer.WriteVarInt(Sequence);
            writer.WriteType<Vec2f>(Rotation, protocolVersion);
            return;
        }

        throw new System.NotSupportedException($"UseItemPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toServer.use_item", "UseItem", PacketPhase.Play, PacketDirection.Serverbound, 59);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x2E;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 758)
        {
            id = 0x2F;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x31;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 763)
        {
            id = 0x32;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x35;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x36;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x39;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 768)
        {
            id = 0x3B;
            return true;
        }

        if (protocolVersion >= 769 && protocolVersion <= 769)
        {
            id = 0x3D;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 770)
        {
            id = 0x3F;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 772)
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
