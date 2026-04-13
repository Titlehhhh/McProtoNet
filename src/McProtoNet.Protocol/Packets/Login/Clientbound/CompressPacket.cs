using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Clientbound;

[PacketInfo("Compress", PacketState.Login, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol, 0x03)]
public sealed partial class CompressPacket : IPacket
{
    public int Threshold { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteVarInt(Threshold);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Threshold = reader.ReadVarInt();

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}