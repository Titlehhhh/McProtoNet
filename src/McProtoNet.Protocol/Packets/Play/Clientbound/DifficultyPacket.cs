using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("Difficulty", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 770)]
[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x0D)]
[PacketId(751, 754, 0x0D)]
[PacketId(755, 758, 0x0E)]
[PacketId(759, 761, 0x0B)]
[PacketId(762, 763, 0x0C)]
[PacketId(764, 769, 0x0B)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x0A)]
public sealed partial class DifficultyPacket : IServerPacket
{
    public bool DifficultyLocked { get; set; }

    public VFirst_770Fields? VFirst_770 { get; set; }
    public V771_LastFields? V771_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 770:
                {
                    var fields = VFirst_770 ?? throw new InvalidOperationException("DifficultyPacket  first-770 fields missing.");
                    writer.WriteBoolean(DifficultyLocked);
                    writer.WriteUnsignedByte(fields.Difficulty);
                    return;
                }
            case >= 771 and <= MinecraftVersion.LatestProtocol:
                {
                    var fields = V771_Last ?? throw new InvalidOperationException("DifficultyPacket 771-last fields missing.");
                    writer.WriteBoolean(DifficultyLocked);
                    writer.WriteVarInt(fields.Difficulty);
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
                    DifficultyLocked = reader.ReadBoolean();
                    VFirst_770 = new VFirst_770Fields { Difficulty = reader.ReadUnsignedByte() };
                    V771_Last = null;
                    return;
                }
            case >= 771 and <= MinecraftVersion.LatestProtocol:
                {
                    DifficultyLocked = reader.ReadBoolean();
                    V771_Last = new V771_LastFields { Difficulty = reader.ReadVarInt() };
                    VFirst_770 = null;
                    return;
                }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(DifficultyPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_770Fields
    {
        public byte Difficulty { get; set; }
    }

    public struct V771_LastFields
    {
        public int Difficulty { get; set; }
    }
}