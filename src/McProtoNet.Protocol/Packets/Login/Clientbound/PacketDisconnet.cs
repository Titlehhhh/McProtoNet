using McProtoNet.Protocol;
using McProtoNet.Serialization;


namespace McProtoNet.Protocol.ClientboundPackets.Login;

public sealed class DisconnectPacket : IServerPacket
{
    public string Reason { get; set; }


    public void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Reason = reader.ReadString();
    }

    public new static bool SupportedVersion(int protocolVersion)
    {
        return protocolVersion is >= 340 and <= 769;
    }


    public static PacketIdentifier PacketId => ServerLoginPacket.Disconnect;

    public PacketIdentifier GetPacketId() => PacketId;
}