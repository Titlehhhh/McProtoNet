using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.transfer", PacketPhase.Play, PacketDirection.Clientbound)]
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

    public static PacketIdentity Identity => new("play.toClient.transfer", "Transfer", PacketPhase.Play, PacketDirection.Clientbound, 119);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
