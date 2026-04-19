using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SyncEntityPosition", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
[PacketId(768, 769, 0x20)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x1F)]
public sealed partial class SyncEntityPositionPacket : IServerPacket
{
    public int EntityId { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public double Dx { get; set; }
    public double Dy { get; set; }
    public double Dz { get; set; }
    public float Yaw { get; set; }
    public float Pitch { get; set; }
    public bool OnGround { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteVarInt(EntityId);
    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => EntityId = reader.ReadVarInt();
}