using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("ScoreboardDisplayObjective", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x43)]
[PacketId(751, 754, 0x43)]
[PacketId(755, 759, 0x4C)]
[PacketId(760, 760, 0x4F)]
[PacketId(761, 761, 0x4D)]
[PacketId(762, 763, 0x51)]
[PacketId(764, 764, 0x53)]
[PacketId(765, 765, 0x55)]
[PacketId(766, 767, 0x57)]
[PacketId(768, 769, 0x5C)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x5B)]
public sealed partial class ScoreboardDisplayObjectivePacket : IServerPacket
{
    public string Name { get; set; }

    public VFirst_763Fields? VFirst_763 { get; set; }
    public V764_LastFields? V764_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteString(Name);
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 763:
            {
                var fields = VFirst_763 ?? throw new InvalidOperationException("ScoreboardDisplayObjectivePacket 1-763 fields missing.");
                writer.WriteSignedByte(fields.Position);
                return;
            }
            case >= 764 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V764_Last ?? throw new InvalidOperationException("ScoreboardDisplayObjectivePacket 764-last fields missing.");
                writer.WriteVarInt(fields.Position);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ScoreboardDisplayObjectivePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Name = reader.ReadString();
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 763:
            {
                VFirst_763 = new VFirst_763Fields { Position = reader.ReadSignedByte() };
                return;
            }
            case >= 764 and <= MinecraftVersion.LatestProtocol:
            {
                V764_Last = new V764_LastFields { Position = reader.ReadVarInt() };
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ScoreboardDisplayObjectivePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    public struct VFirst_763Fields { public sbyte Position { get; set; } }
    public struct V764_LastFields { public int Position { get; set; } }
}