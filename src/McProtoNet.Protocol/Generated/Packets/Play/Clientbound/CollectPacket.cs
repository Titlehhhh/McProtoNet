using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.collect", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("CollectedEntityId", "int")]
[PacketField("CollectorEntityId", "int")]
[PacketField("PickupItemCount", "int")]
public sealed partial record CollectPacket(int CollectedEntityId, int CollectorEntityId, int PickupItemCount) : IPacket<CollectPacket>, IPacket
{
    public static CollectPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CollectPacket>(protocolVersion);
        var collectedEntityId = reader.ReadVarInt();
        var collectorEntityId = reader.ReadVarInt();
        var pickupItemCount = reader.ReadVarInt();
        return new CollectPacket(collectedEntityId, collectorEntityId, pickupItemCount);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CollectPacket>(protocolVersion);
        writer.WriteVarInt(CollectedEntityId);
        writer.WriteVarInt(CollectorEntityId);
        writer.WriteVarInt(PickupItemCount);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("CollectedEntityId");
        writer.WriteNumberValue(CollectedEntityId);
        writer.WritePropertyName("CollectorEntityId");
        writer.WriteNumberValue(CollectorEntityId);
        writer.WritePropertyName("PickupItemCount");
        writer.WriteNumberValue(PickupItemCount);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.collect", "Collect", PacketPhase.Play, PacketDirection.Clientbound, 20);

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
