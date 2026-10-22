using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Handshaking.Serverbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("handshaking.toServer.set_protocol", PacketPhase.Handshaking, PacketDirection.Serverbound)]
[PacketField("ProtocolVersion", "int")]
[PacketField("ServerHost", "string")]
[PacketField("ServerPort", "int")]
[PacketField("NextState", "int")]
public sealed partial record SetProtocolPacket(int ProtocolVersion, string ServerHost, int ServerPort, int NextState) : IPacket<SetProtocolPacket>, IPacket
{
    public static SetProtocolPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetProtocolPacket>(protocolVersion);
        var protocolVersion_ = reader.ReadVarInt();
        var serverHost = reader.ReadString();
        var serverPort = reader.ReadUnsignedShort();
        var nextState = reader.ReadVarInt();
        return new SetProtocolPacket(protocolVersion_, serverHost, serverPort, nextState);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetProtocolPacket>(protocolVersion);
        writer.WriteVarInt(ProtocolVersion);
        writer.WriteString(ServerHost);
        writer.WriteUnsignedShort((ushort)ServerPort);
        writer.WriteVarInt(NextState);
    }

    public static PacketIdentity Identity => new("handshaking.toServer.set_protocol", "SetProtocol", PacketPhase.Handshaking, PacketDirection.Serverbound, 1);

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
