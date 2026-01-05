using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Clientbound;

[PacketInfo("Compress", PacketState.Login, PacketDirection.Clientbound)]
public sealed partial class CompressPacket : IServerPacket
{
    public int Threshold { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(Threshold);
                return;
            default:
                throw new ProtocolNotSupportException(nameof(ServerLoginPacket.Compress), protocolVersion);
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                Threshold = reader.ReadVarInt();
                return;
            default:
                throw new ProtocolNotSupportException(nameof(ServerLoginPacket.Compress), protocolVersion);
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
