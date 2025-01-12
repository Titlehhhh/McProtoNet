using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public interface IServerPacket
{
    public void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    public static virtual ServerPacket PacketId { get; }
    
    public ServerPacket GetPacketId();
    
    public static virtual bool VersionSupported(int protocolVersion)
    {
        throw new NotImplementedException();
    }
}