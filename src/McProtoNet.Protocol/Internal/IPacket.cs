namespace McProtoNet.Protocol;

public interface IPacket
{
    static virtual PacketIdentifier PacketId { get; }
    PacketIdentifier GetPacketId();
    public static virtual bool IsSupportedVersionStatic(int protocolVersion) => throw new NotImplementedException();

    bool IsSupportedVersion(int protocolVersion);
}

