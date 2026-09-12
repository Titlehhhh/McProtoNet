using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.attach_entity", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("VehicleId", "int")]
public sealed partial record AttachEntityPacket(int EntityId, int VehicleId) : IPacket<AttachEntityPacket>, IPacket
{
    public static AttachEntityPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AttachEntityPacket>(protocolVersion);
        var entityId = reader.ReadSignedInt();
        var vehicleId = reader.ReadSignedInt();
        return new AttachEntityPacket(entityId, vehicleId);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AttachEntityPacket>(protocolVersion);
        writer.WriteSignedInt(EntityId);
        writer.WriteSignedInt(VehicleId);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("EntityId");
        writer.WriteNumberValue(EntityId);
        writer.WritePropertyName("VehicleId");
        writer.WriteNumberValue(VehicleId);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.attach_entity", "AttachEntity", PacketPhase.Play, PacketDirection.Clientbound, 5);

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
