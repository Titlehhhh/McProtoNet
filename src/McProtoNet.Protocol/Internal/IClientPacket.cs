using System.Dynamic;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public interface IClientPacket
{
    public void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion);

    public static virtual ClientPacket PacketId { get; }
    public ClientPacket GetPacketId();

    public static virtual bool SupportedVersion(int protocolVersion) => throw new NotImplementedException();
}
