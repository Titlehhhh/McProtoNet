namespace McProtoNet.Protocol;

public interface IPacket
{
    static virtual PacketIdentifier PacketId { get; }
    PacketIdentifier GetPacketId();
    public static virtual bool SupportedVersion(int protocolVersion) => throw new NotImplementedException();
}