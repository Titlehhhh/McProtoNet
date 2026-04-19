using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Status.Serverbound;

[PacketInfo("Ping", PacketState.Status, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol, 0x01)]
public sealed partial class PingPacket : IClientPacket
{
    public long Time { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteSignedLong(Time);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Time = reader.ReadSignedLong();
}