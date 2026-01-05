using System;
using McProtoNet.Protocol.Extensions;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("CookieRequest", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class CookieRequestPacket : IServerPacket
{
    public PacketCommonCookieRequest Data { get; set; } = null!;

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                writer.WritePacketCommonCookieRequest(Data, protocolVersion);
                return;
            default:
                throw new ProtocolNotSupportException(nameof(ServerPlayPacket.CookieRequest), protocolVersion);
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                Data = reader.ReadPacketCommonCookieRequest(protocolVersion);
                return;
            default:
                throw new ProtocolNotSupportException(nameof(ServerPlayPacket.CookieRequest), protocolVersion);
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
