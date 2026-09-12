using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

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

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Location");
        Location.WriteJson(writer);
        writer.WritePropertyName("Name");
        writer.WriteStringValue(Name);
        writer.WritePropertyName("Target");
        writer.WriteStringValue(Target);
        writer.WritePropertyName("Pool");
        writer.WriteStringValue(Pool);
        writer.WritePropertyName("FinalState");
        writer.WriteStringValue(FinalState);
        writer.WritePropertyName("JointType");
        writer.WriteStringValue(JointType);
        if (V765_Last is { } v765_Last)
        {
            writer.WritePropertyName("SelectionPriority");
            writer.WriteNumberValue(v765_Last.SelectionPriority);
            writer.WritePropertyName("PlacementPriority");
            writer.WriteNumberValue(v765_Last.PlacementPriority);
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toServer.update_jigsaw_block", "UpdateJigsawBlock", PacketPhase.Play, PacketDirection.Serverbound, 63);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
