using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public readonly partial record struct VillagerData(int Type, int Profession, int Level) : IProtocolType<VillagerData>
{
    public static VillagerData Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<VillagerData>(protocolVersion);
        var type = reader.ReadVarInt();
        var profession = reader.ReadVarInt();
        var level = reader.ReadVarInt();
        return new VillagerData(type, profession, level);
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<VillagerData>(protocolVersion);
        writer.WriteVarInt(Type);
        writer.WriteVarInt(Profession);
        writer.WriteVarInt(Level);
    }
}
