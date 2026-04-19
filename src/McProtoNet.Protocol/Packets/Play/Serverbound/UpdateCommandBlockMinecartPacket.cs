using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("UpdateCommandBlockMinecart", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x26)]
[PacketId(751, 758, 0x27)]
[PacketId(759, 759, 0x29)]
[PacketId(760, 763, 0x2A)]
[PacketId(764, 764, 0x2D)]
[PacketId(765, 765, 0x2E)]
[PacketId(766, 767, 0x31)]
[PacketId(768, 768, 0x33)]
[PacketId(769, 770, 0x35)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x36)]
public sealed partial class UpdateCommandBlockMinecartPacket : IClientPacket
{
    public int EntityId { get; set; }
    public string Command { get; set; } = string.Empty;
    public bool TrackOutput { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(EntityId);
        writer.WriteString(Command);
        writer.WriteBoolean(TrackOutput);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        EntityId = reader.ReadVarInt();
        Command = reader.ReadString();
        TrackOutput = reader.ReadBoolean();
    }
}