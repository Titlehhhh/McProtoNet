using McProtoNet.Serialization;

namespace SampleBotCSharp;

class LoginStartPacket : IPacket
{
    public string Name { get; set; }
    public Guid UUID { get; set; }

    public void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteString(Name);
        writer.WriteUUID(UUID);
    }
}