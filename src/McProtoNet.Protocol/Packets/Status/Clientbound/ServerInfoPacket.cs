using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Status.Clientbound;

[PacketInfo("ServerInfo", PacketState.Status, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol, 0x00)]
public sealed partial class ServerInfoPacket : IServerPacket
{
    public int Container { get; set; }
    public string Response { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(Container);
                writer.WriteString(Response);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerInfoPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                Container = reader.ReadVarInt();
                Response = reader.ReadString();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerInfoPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}