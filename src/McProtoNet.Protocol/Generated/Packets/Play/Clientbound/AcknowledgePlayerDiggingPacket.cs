using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.acknowledge_player_digging", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Location", "Position", Group = "VUntil758", To = 758)]
[PacketField("Block", "int", Group = "VUntil758", To = 758)]
[PacketField("Status", "int", Group = "VUntil758", To = 758)]
[PacketField("Successful", "bool", Group = "VUntil758", To = 758)]
[PacketField("SequenceId", "int", Group = "V759_Last", From = 759)]
public sealed partial record AcknowledgePlayerDiggingPacket(AcknowledgePlayerDiggingPacket.VUntil758Layer? VUntil758 = null, AcknowledgePlayerDiggingPacket.V759_LastLayer? V759_Last = null) : IPacket<AcknowledgePlayerDiggingPacket>, IPacket
{
    public readonly record struct VUntil758Layer(Position Location, int Block, int Status, bool Successful);
    public readonly record struct V759_LastLayer(int SequenceId);
    public static AcknowledgePlayerDiggingPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AcknowledgePlayerDiggingPacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            var location = reader.ReadType<Position>(protocolVersion);
            var block = reader.ReadVarInt();
            var status = reader.ReadVarInt();
            var successful = reader.ReadBoolean();
            return new AcknowledgePlayerDiggingPacket(VUntil758: new VUntil758Layer(location, block, status, successful));
        }

        if (protocolVersion >= 759)
        {
            var sequenceId = reader.ReadVarInt();
            return new AcknowledgePlayerDiggingPacket(V759_Last: new V759_LastLayer(sequenceId));
        }

        throw new System.NotSupportedException($"AcknowledgePlayerDiggingPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AcknowledgePlayerDiggingPacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            var layer = VUntil758 ?? throw new WrongLayerException("AcknowledgePlayerDiggingPacket", protocolVersion, "VUntil758");
            Position Location = layer.Location;
            int Block = layer.Block;
            int Status = layer.Status;
            bool Successful = layer.Successful;
            writer.WriteType<Position>(Location, protocolVersion);
            writer.WriteVarInt(Block);
            writer.WriteVarInt(Status);
            writer.WriteBoolean(Successful);
            return;
        }

        if (protocolVersion >= 759)
        {
            var layer = V759_Last ?? throw new WrongLayerException("AcknowledgePlayerDiggingPacket", protocolVersion, "V759_Last");
            int SequenceId = layer.SequenceId;
            writer.WriteVarInt(SequenceId);
            return;
        }

        throw new System.NotSupportedException($"AcknowledgePlayerDiggingPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        if (VUntil758 is { } vUntil758)
        {
            writer.WritePropertyName("Location");
            vUntil758.Location.WriteJson(writer);
            writer.WritePropertyName("Block");
            writer.WriteNumberValue(vUntil758.Block);
            writer.WritePropertyName("Status");
            writer.WriteNumberValue(vUntil758.Status);
            writer.WritePropertyName("Successful");
            writer.WriteBooleanValue(vUntil758.Successful);
        }
        else if (V759_Last is { } v759_Last)
        {
            writer.WritePropertyName("SequenceId");
            writer.WriteNumberValue(v759_Last.SequenceId);
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.acknowledge_player_digging", "AcknowledgePlayerDigging", PacketPhase.Play, PacketDirection.Clientbound, 1);

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
