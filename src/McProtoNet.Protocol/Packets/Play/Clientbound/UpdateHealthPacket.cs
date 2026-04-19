using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("UpdateHealth", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x49)]
[PacketId(751, 754, 0x49)]
[PacketId(755, 759, 0x52)]
[PacketId(760, 760, 0x55)]
[PacketId(761, 761, 0x53)]
[PacketId(762, 763, 0x57)]
[PacketId(764, 764, 0x59)]
[PacketId(765, 765, 0x5B)]
[PacketId(766, 767, 0x5D)]
[PacketId(768, 769, 0x62)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x61)]
public sealed partial class UpdateHealthPacket : IServerPacket
{
    public float Health { get; set; }
    public int Food { get; set; }
    public float FoodSaturation { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteFloat(Health);
        writer.WriteVarInt(Food);
        writer.WriteFloat(FoodSaturation);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Health = reader.ReadFloat();
        Food = reader.ReadVarInt();
        FoodSaturation = reader.ReadFloat();
    }
}