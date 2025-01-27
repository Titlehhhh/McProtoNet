using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.ServerboundPackets.Handshaking;

public sealed class SetProtocolPacket : IClientPacket
{
    public int ProtocolVersion { get; set; }
    public string ServerHost { get; set; }
    public ushort ServerPort { get; set; }
    public int NextState { get; set; }

    public static PacketIdentifier PacketId => ClientHandshakingPacket.SetProtocol;

    public PacketIdentifier GetPacketId() => PacketId;


    public static bool SupportedVersion(int protocolVersion)
    {
        return protocolVersion is >= 340 and <= 769;
    }

    public void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(ProtocolVersion);
        writer.WriteString(ServerHost);
        writer.WriteUnsignedShort(ServerPort);
        writer.WriteVarInt(NextState);
    }
}