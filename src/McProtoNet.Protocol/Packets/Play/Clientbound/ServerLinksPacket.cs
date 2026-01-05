using System;
using McProtoNet.Protocol.Extensions;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("ServerLinks", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class ServerLinksPacket : IServerPacket
{
    public PacketCommonServerLinks Data { get; set; } = null!;

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 767 and <= MinecraftVersion.LatestProtocol:
                writer.WritePacketCommonServerLinks(Data, protocolVersion);
                return;
            default:
                throw new ProtocolNotSupportException(nameof(ServerPlayPacket.ServerLinks), protocolVersion);
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 767 and <= MinecraftVersion.LatestProtocol:
                Data = reader.ReadPacketCommonServerLinks(protocolVersion);
                return;
            default:
                throw new ProtocolNotSupportException(nameof(ServerPlayPacket.ServerLinks), protocolVersion);
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
