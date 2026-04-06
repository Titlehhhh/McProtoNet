using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("SetDifficulty", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 770)]
[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x02)]
[PacketId(751, 767, 0x02)]
[PacketId(768, MinecraftVersion.LatestProtocol, 0x03)]
public sealed partial class SetDifficultyPacket : IClientPacket
{
    public VFirst_770Fields? VFirst_770 { get; set; }
    public V771_LastFields? V771_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 770:
            {
                var fields = VFirst_770 ?? throw new InvalidOperationException("SetDifficultyPacket First-770 fields missing.");
                writer.WriteUnsignedByte(fields.NewDifficulty);
                return;
            }
            case >= 771 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V771_Last ?? throw new InvalidOperationException("SetDifficultyPacket 771-Last fields missing.");
                writer.WriteVarInt(fields.NewDifficulty);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SetDifficultyPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 770:
                VFirst_770 = new VFirst_770Fields { NewDifficulty = reader.ReadUnsignedByte() };
                V771_Last = null;
                return;
            case >= 771 and <= MinecraftVersion.LatestProtocol:
                V771_Last = new V771_LastFields { NewDifficulty = reader.ReadVarInt() };
                VFirst_770 = null;
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SetDifficultyPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_770Fields { public byte NewDifficulty { get; set; } }
    public struct V771_LastFields { public int NewDifficulty { get; set; } }
}