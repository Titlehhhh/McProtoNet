using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.use_entity", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Target", "int")]
[PacketField("Sneaking", "bool")]
[PacketField("Action", "InteractAction", Group = "VUntil774", To = 774)]
[PacketField("Hand", "int", Group = "V775_Last", From = 775)]
[PacketField("Location", "LpVec3", Group = "V775_Last", From = 775)]
public sealed partial record UseEntityPacket(int Target, bool Sneaking, UseEntityPacket.VUntil774Layer? VUntil774 = null, UseEntityPacket.V775_LastLayer? V775_Last = null) : IPacket<UseEntityPacket>, IPacket
{
    public readonly record struct VUntil774Layer(InteractAction Action);
    public readonly record struct V775_LastLayer(int Hand, LpVec3 Location);
    public static UseEntityPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UseEntityPacket>(protocolVersion);
        if (protocolVersion <= 774)
        {
            var target = reader.ReadVarInt();
            var _mouse = reader.ReadVarInt();
            var action = InteractAction.Read(ref reader, protocolVersion, (int)_mouse);
            var sneaking = reader.ReadBoolean();
            return new UseEntityPacket(target, sneaking, VUntil774: new VUntil774Layer(action));
        }

        if (protocolVersion >= 775)
        {
            var target = reader.ReadVarInt();
            var hand = reader.ReadVarInt();
            var location = reader.ReadType<LpVec3>(protocolVersion);
            var sneaking = reader.ReadBoolean();
            return new UseEntityPacket(target, sneaking, V775_Last: new V775_LastLayer(hand, location));
        }

        throw new System.NotSupportedException($"UseEntityPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UseEntityPacket>(protocolVersion);
        if (protocolVersion <= 774)
        {
            var layer = VUntil774 ?? throw new WrongLayerException("UseEntityPacket", protocolVersion, "VUntil774");
            InteractAction Action = layer.Action;
            writer.WriteVarInt(Target);
            writer.WriteVarInt(Action.Discriminator(protocolVersion));
            Action.Write(writer, protocolVersion);
            writer.WriteBoolean(Sneaking);
            return;
        }

        if (protocolVersion >= 775)
        {
            var layer = V775_Last ?? throw new WrongLayerException("UseEntityPacket", protocolVersion, "V775_Last");
            int Hand = layer.Hand;
            LpVec3 Location = layer.Location;
            writer.WriteVarInt(Target);
            writer.WriteVarInt(Hand);
            writer.WriteType<LpVec3>(Location, protocolVersion);
            writer.WriteBoolean(Sneaking);
            return;
        }

        throw new System.NotSupportedException($"UseEntityPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toServer.use_entity", "UseEntity", PacketPhase.Play, PacketDirection.Serverbound, 62);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x0E;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x0E;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x0D;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x0F;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x10;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x0F;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x10;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x12;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x13;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x16;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 770)
        {
            id = 0x18;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x19;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x1A;
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
