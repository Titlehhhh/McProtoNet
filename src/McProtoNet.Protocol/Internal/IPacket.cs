namespace McProtoNet.Protocol;

public interface IPacket
{
    static virtual PacketIdentifier PacketId { get; }
    PacketIdentifier GetPacketId();
}