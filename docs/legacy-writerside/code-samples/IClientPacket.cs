public interface IClientPacket : IPacket
{
    void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion);
}