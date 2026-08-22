using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.update_jigsaw_block", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Location", "Position")]
[PacketField("Name", "string")]
[PacketField("Target", "string")]
[PacketField("Pool", "string")]
[PacketField("FinalState", "string")]
[PacketField("JointType", "string")]
[PacketField("SelectionPriority", "int", Group = "V765_Last", From = 765)]
[PacketField("PlacementPriority", "int", Group = "V765_Last", From = 765)]
public sealed partial record UpdateJigsawBlockPacket(Position Location, string Name, string Target, string Pool, string FinalState, string JointType, UpdateJigsawBlockPacket.V765_LastLayer? V765_Last = null) : IPacket<UpdateJigsawBlockPacket>, IPacket
{
    public readonly record struct V765_LastLayer(int SelectionPriority, int PlacementPriority);
    public static UpdateJigsawBlockPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateJigsawBlockPacket>(protocolVersion);
        if (protocolVersion <= 764)
        {
            var location = reader.ReadType<Position>(protocolVersion);
            var name = reader.ReadString();
            var target = reader.ReadString();
            var pool = reader.ReadString();
            var finalState = reader.ReadString();
            var jointType = reader.ReadString();
            return new UpdateJigsawBlockPacket(location, name, target, pool, finalState, jointType);
        }

        if (protocolVersion >= 765)
        {
            var location = reader.ReadType<Position>(protocolVersion);
            var name = reader.ReadString();
            var target = reader.ReadString();
            var pool = reader.ReadString();
            var finalState = reader.ReadString();
            var jointType = reader.ReadString();
            var selectionPriority = reader.ReadVarInt();
            var placementPriority = reader.ReadVarInt();
            return new UpdateJigsawBlockPacket(location, name, target, pool, finalState, jointType, V765_Last: new V765_LastLayer(selectionPriority, placementPriority));
        }

        throw new System.NotSupportedException($"UpdateJigsawBlockPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateJigsawBlockPacket>(protocolVersion);
        if (protocolVersion <= 764)
        {
            writer.WriteType<Position>(Location, protocolVersion);
            writer.WriteString(Name);
            writer.WriteString(Target);
            writer.WriteString(Pool);
            writer.WriteString(FinalState);
            writer.WriteString(JointType);
            return;
        }

        if (protocolVersion >= 765)
        {
            var layer = V765_Last ?? throw new WrongLayerException("UpdateJigsawBlockPacket", protocolVersion, "V765_Last");
            int SelectionPriority = layer.SelectionPriority;
            int PlacementPriority = layer.PlacementPriority;
            writer.WriteType<Position>(Location, protocolVersion);
            writer.WriteString(Name);
            writer.WriteString(Target);
            writer.WriteString(Pool);
            writer.WriteString(FinalState);
            writer.WriteString(JointType);
            writer.WriteVarInt(SelectionPriority);
            writer.WriteVarInt(PlacementPriority);
            return;
        }

        throw new System.NotSupportedException($"UpdateJigsawBlockPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toServer.update_jigsaw_block", "UpdateJigsawBlock", PacketPhase.Play, PacketDirection.Serverbound, 61);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x28;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 758)
        {
            id = 0x29;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x2B;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 763)
        {
            id = 0x2C;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x2F;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x30;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x33;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 768)
        {
            id = 0x35;
            return true;
        }

        if (protocolVersion >= 769 && protocolVersion <= 770)
        {
            id = 0x37;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x38;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x3A;
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
