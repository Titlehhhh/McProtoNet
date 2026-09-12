using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.set_passengers", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("Passengers", "int[]")]
public sealed partial record SetPassengersPacket(int EntityId, int[] Passengers) : IPacket<SetPassengersPacket>, IPacket
{
    public static SetPassengersPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetPassengersPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        int passengersCount = reader.ReadVarInt();
        var passengers = new int[passengersCount];
        for (int i = 0; i < passengers.Length; i++)
            passengers[i] = reader.ReadVarInt();
        return new SetPassengersPacket(entityId, passengers);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetPassengersPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
        writer.WriteVarInt(Passengers.Length);
        foreach (var passengersItem in Passengers)
            writer.WriteVarInt(passengersItem);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("EntityId");
        writer.WriteNumberValue(EntityId);
        writer.WritePropertyName("Passengers");
        writer.WriteStartArray();
        foreach (var item0 in Passengers)
        {
            writer.WriteNumberValue(item0);
        }

        writer.WriteEndArray();
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.set_passengers", "SetPassengers", PacketPhase.Play, PacketDirection.Clientbound, 91);

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
