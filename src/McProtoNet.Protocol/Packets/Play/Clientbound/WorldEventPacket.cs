using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("WorldEvent", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x22)]
[PacketId(751, 754, 0x21)]
[PacketId(755, 758, 0x23)]
[PacketId(759, 759, 0x20)]
[PacketId(760, 760, 0x22)]
[PacketId(761, 761, 0x21)]
[PacketId(762, 763, 0x25)]
[PacketId(764, 765, 0x26)]
[PacketId(766, 767, 0x28)]
[PacketId(768, 769, 0x29)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x28)]
public sealed partial class WorldEventPacket : IServerPacket
{
    public int EffectId { get; set; }
    public Position Location { get; set; }
    public int Data { get; set; }
    public bool Global { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteSignedInt(EffectId);
        writer.WriteType<Position>(Location, protocolVersion);
        writer.WriteSignedInt(Data);
        writer.WriteBoolean(Global);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        EffectId = reader.ReadSignedInt();
        Location = reader.ReadType<Position>(protocolVersion);
        Data = reader.ReadSignedInt();
        Global = reader.ReadBoolean();
    }
}