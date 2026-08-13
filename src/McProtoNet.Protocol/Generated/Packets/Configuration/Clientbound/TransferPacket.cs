using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toClient.transfer", PacketPhase.Configuration, PacketDirection.Clientbound)]
[PacketField("Host", "string")]
[PacketField("Port", "int")]
public sealed partial record TransferPacket(string Host, int Port) : IPacket<TransferPacket>, IPacket
{
    public static TransferPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TransferPacket>(protocolVersion);
        var host = reader.ReadString();
        var port = reader.ReadVarInt();
        return new TransferPacket(host, port);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TransferPacket>(protocolVersion);
        writer.WriteString(Host);
        writer.WriteVarInt(Port);
    }

    public static PacketIdentity Identity => new("configuration.toClient.transfer", "Transfer", PacketPhase.Configuration, PacketDirection.Clientbound, 17);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 766 && protocolVersion <= 772)
        {
            id = 0x0B;
            return true;
        }

        id = 0;
        return false;
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (TryGetPacketId(protocolVersion, out var id))
            return id;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
