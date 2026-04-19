using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("AcknowledgePlayerDigging", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x07)]
[PacketId(751, 754, 0x07)]
[PacketId(755, 758, 0x08)]
[PacketId(759, 761, 0x05)]
[PacketId(762, 763, 0x06)]
[PacketId(764, 769, 0x05)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x04)]
public sealed partial class AcknowledgePlayerDiggingPacket : IServerPacket
{
    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            {
                var fields = VFirst_758 ?? throw new InvalidOperationException("AcknowledgePlayerDiggingPacket 1-758 fields missing.");
                writer.WriteType<Position>(fields.Location, protocolVersion);
                writer.WriteVarInt(fields.Block);
                writer.WriteVarInt(fields.Status);
                writer.WriteBoolean(fields.Successful);
                return;
            }
            case >= 759 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V759_Last ?? throw new InvalidOperationException("AcknowledgePlayerDiggingPacket 759-last fields missing.");
                writer.WriteVarInt(fields.SequenceId);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(AcknowledgePlayerDiggingPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            {
                VFirst_758 = new VFirst_758Fields
                {
                    Location = reader.ReadType<Position>(protocolVersion),
                    Block = reader.ReadVarInt(),
                    Status = reader.ReadVarInt(),
                    Successful = reader.ReadBoolean()
                };
                V759_Last = null;
                return;
            }
            case >= 759 and <= MinecraftVersion.LatestProtocol:
            {
                V759_Last = new V759_LastFields
                {
                    SequenceId = reader.ReadVarInt()
                };
                VFirst_758 = null;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(AcknowledgePlayerDiggingPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    public VFirst_758Fields? VFirst_758 { get; set; }
    public V759_LastFields? V759_Last { get; set; }

    public struct VFirst_758Fields
    {
        public Position Location { get; set; }
        public int Block { get; set; }
        public int Status { get; set; }
        public bool Successful { get; set; }
    }

    public struct V759_LastFields
    {
        public int SequenceId { get; set; }
    }
}