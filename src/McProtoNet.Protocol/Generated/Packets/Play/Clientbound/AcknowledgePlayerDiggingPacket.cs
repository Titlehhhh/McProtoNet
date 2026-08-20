using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

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

    public static PacketIdentity Identity => new("play.toClient.acknowledge_player_digging", "AcknowledgePlayerDigging", PacketPhase.Play, PacketDirection.Clientbound, 1);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x07;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x07;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x08;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 761)
        {
            id = 0x05;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x06;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 769)
        {
            id = 0x05;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 776)
        {
            id = 0x04;
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
