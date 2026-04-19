using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("UpdateJigsawBlock", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x28)]
[PacketId(751, 758, 0x29)]
[PacketId(759, 759, 0x2B)]
[PacketId(760, 763, 0x2C)]
[PacketId(764, 764, 0x2F)]
[PacketId(765, 765, 0x30)]
[PacketId(766, 767, 0x33)]
[PacketId(768, 768, 0x35)]
[PacketId(769, 770, 0x37)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x38)]
public sealed partial class UpdateJigsawBlockPacket : IClientPacket
{
    public Position Location { get; set; }
    public string Name { get; set; }
    public string Target { get; set; }
    public string Pool { get; set; }
    public string FinalState { get; set; }
    public string JointType { get; set; }

    public int? SelectionPriority { get; set; }
    public int? PlacementPriority { get; set; }

    public V765_LastFields? V765_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteType<Position>(Location, protocolVersion);
        writer.WriteString(Name);
        writer.WriteString(Target);
        writer.WriteString(Pool);
        writer.WriteString(FinalState);
        writer.WriteString(JointType);
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
            {
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V765_Last ?? throw new InvalidOperationException("UpdateJigsawBlockPacket 765-last fields missing.");
                writer.WriteVarInt(fields.SelectionPriority);
                writer.WriteVarInt(fields.PlacementPriority);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(UpdateJigsawBlockPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Location = reader.ReadType<Position>(protocolVersion);
        Name = reader.ReadString();
        Target = reader.ReadString();
        Pool = reader.ReadString();
        FinalState = reader.ReadString();
        JointType = reader.ReadString();
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
            {
                V765_Last = null;
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                V765_Last = new V765_LastFields
                {
                    SelectionPriority = reader.ReadVarInt(),
                    PlacementPriority = reader.ReadVarInt()
                };
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(UpdateJigsawBlockPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    public struct V765_LastFields
    {
        public int SelectionPriority { get; set; }
        public int PlacementPriority { get; set; }
    }
}