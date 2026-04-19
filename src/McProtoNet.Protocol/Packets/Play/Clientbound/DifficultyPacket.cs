using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("Difficulty", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x0D)]
[PacketId(751, 754, 0x0D)]
[PacketId(755, 758, 0x0E)]
[PacketId(759, 761, 0x0B)]
[PacketId(762, 763, 0x0C)]
[PacketId(764, 769, 0x0B)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x0A)]
public sealed partial class DifficultyPacket : IServerPacket
{
    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 770:
            {
                var fields = VFirst_770 ?? throw new InvalidOperationException("DifficultyPacket First-770 fields missing.");
                writer.WriteUnsignedByte(fields.Difficulty);
                writer.WriteBoolean(fields.DifficultyLocked);
                return;
            }
            case >= 771 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V771_Last ?? throw new InvalidOperationException("DifficultyPacket 771-last fields missing.");
                writer.WriteVarInt(fields.Difficulty);
                writer.WriteBoolean(fields.DifficultyLocked);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(DifficultyPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 770:
            {
                VFirst_770 = new VFirst_770Fields
                {
                    Difficulty = reader.ReadUnsignedByte(),
                    DifficultyLocked = reader.ReadBoolean()
                };
                return;
            }
            case >= 771 and <= MinecraftVersion.LatestProtocol:
            {
                V771_Last = new V771_LastFields
                {
                    Difficulty = reader.ReadVarInt(),
                    DifficultyLocked = reader.ReadBoolean()
                };
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(DifficultyPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    public VFirst_770Fields? VFirst_770 { get; set; }
    public V771_LastFields? V771_Last { get; set; }

    public struct VFirst_770Fields
    {
        public byte Difficulty { get; set; }
        public bool DifficultyLocked { get; set; }
    }

    public struct V771_LastFields
    {
        public int Difficulty { get; set; }
        public bool DifficultyLocked { get; set; }
    }
}