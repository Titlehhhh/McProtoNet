using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public readonly partial record struct RegistryEntryHolder<T>
{
    public static RegistryEntryHolder<T> Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RegistryEntryHolder<T>>(protocolVersion);
        return reader.ReadRegistryEntryHolder<T>(protocolVersion);
    }

    public void Write(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RegistryEntryHolder<T>>(protocolVersion);
        writer.WriteRegistryEntryHolder(this, protocolVersion);
    }
}
