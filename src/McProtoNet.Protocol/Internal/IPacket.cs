namespace McProtoNet.Protocol;

public interface IPacket
{
    static virtual PacketIdentifier PacketId => PacketIdentifier.Undefined;
    static virtual bool IsSupportedVersionStatic(int protocolVersion) => false;
    
    PacketIdentifier GetPacketId();
    bool IsSupportedVersion(int protocolVersion);
}