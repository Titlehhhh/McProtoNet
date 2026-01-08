using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("AcknowledgePlayerDigging", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class AcknowledgePlayerDiggingPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 758),
        new(759, MinecraftVersion.LatestProtocol),
    };

    public Position Location { get; set; }
    public int Block { get; set; }
    public int Status { get; set; }
    public bool Successful { get; set; }

    public V759_LastFields? V759_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            {
                writer.WritePosition(Location, protocolVersion);
                writer.WriteVarInt(Block);
                writer.WriteVarInt(Status);
                writer.WriteBoolean(Successful);
                return;
            }
            case >= 759 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V759_Last ?? throw new InvalidOperationException("AcknowledgePlayerDigging V759_Last fields missing.");
                writer.WriteVarInt(fields.SequenceId);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.AcknowledgePlayerDigging), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            {
                Location = reader.ReadPosition(protocolVersion);
                Block = reader.ReadVarInt();
                Status = reader.ReadVarInt();
                Successful = reader.ReadBoolean();
                return;
            }
            case >= 759 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = new V759_LastFields();
                fields.SequenceId = reader.ReadVarInt();
                V759_Last = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.AcknowledgePlayerDigging), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V759_LastFields
    {
        public int SequenceId { get; set; }
    }

}
