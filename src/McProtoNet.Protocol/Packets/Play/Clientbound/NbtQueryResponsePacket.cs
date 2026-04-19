using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("NbtQueryResponse", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x54)]
[PacketId(751, 754, 0x54)]
[PacketId(755, 756, 0x5F)]
[PacketId(757, 758, 0x60)]
[PacketId(759, 759, 0x61)]
[PacketId(760, 760, 0x64)]
[PacketId(761, 761, 0x62)]
[PacketId(762, 763, 0x66)]
[PacketId(764, 764, 0x69)]
[PacketId(765, 765, 0x6B)]
[PacketId(766, 767, 0x6E)]
[PacketId(768, 769, 0x75)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x74)]
public sealed partial class NbtQueryResponsePacket : IServerPacket
{
    public int TransactionId { get; set; }

    public VFirst_763Fields? VFirst_763 { get; set; }
    public V764_LastFields? V764_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(TransactionId);
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 763:
            {
                var fields = VFirst_763 ?? throw new InvalidOperationException("NbtQueryResponsePacket 0-763 fields missing.");
                writer.WriteOptionalNbtTag(fields.Nbt, protocolVersion);
                return;
            }
            case >= 764 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V764_Last ?? throw new InvalidOperationException("NbtQueryResponsePacket 764-last fields missing.");
                writer.WriteAnonOptionalNbtTag(fields.Nbt, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(NbtQueryResponsePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        TransactionId = reader.ReadVarInt();
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 763:
            {
                VFirst_763 = new VFirst_763Fields { Nbt = reader.ReadOptionalNbtTag(protocolVersion) };
                V764_Last = null;
                return;
            }
            case >= 764 and <= MinecraftVersion.LatestProtocol:
            {
                V764_Last = new V764_LastFields { Nbt = reader.ReadAnonOptionalNbtTag(protocolVersion) };
                VFirst_763 = null;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(NbtQueryResponsePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    public struct VFirst_763Fields { public NbtTag? Nbt { get; set; } }
    public struct V764_LastFields { public NbtTag? Nbt { get; set; } }
}