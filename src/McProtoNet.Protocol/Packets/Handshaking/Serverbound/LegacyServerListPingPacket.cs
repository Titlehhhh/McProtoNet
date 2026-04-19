using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Handshaking.Serverbound;

[PacketInfo("LegacyServerListPing", PacketState.Handshaking, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol, 0xFE)]
public sealed partial class LegacyServerListPingPacket : IClientPacket
{
    public byte Payload { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteUnsignedByte(Payload);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Payload = reader.ReadUnsignedByte();
}