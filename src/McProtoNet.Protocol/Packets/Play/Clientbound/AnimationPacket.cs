using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("Animation", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x05)]
[PacketId(751, 754, 0x05)]
[PacketId(755, 758, 0x06)]
[PacketId(759, 761, 0x03)]
[PacketId(762, 763, 0x04)]
[PacketId(764, 769, 0x03)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x02)]
public sealed partial class AnimationPacket : IServerPacket
{
    public int EntityId { get; set; }
    public byte Animation { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(EntityId);
        writer.WriteUnsignedByte(Animation);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        EntityId = reader.ReadVarInt();
        Animation = reader.ReadUnsignedByte();
    }
}