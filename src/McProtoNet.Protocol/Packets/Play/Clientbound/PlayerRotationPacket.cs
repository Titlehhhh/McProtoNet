using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("PlayerRotation", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
[PacketId(768, 769, 0x43)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x42)]
public sealed partial class PlayerRotationPacket : IServerPacket
{
    public float Yaw { get; set; }
    public float Pitch { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteFloat(Yaw);
        writer.WriteFloat(Pitch);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Yaw = reader.ReadFloat();
        Pitch = reader.ReadFloat();
    }
}