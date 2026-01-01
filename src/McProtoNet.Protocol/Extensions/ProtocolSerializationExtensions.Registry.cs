using McProtoNet.Serialization;
using McProtoNet.Protocol;

namespace McProtoNet.Protocol.Extensions;

public static partial class ProtocolSerializationExtensions
{
    public static RegistryEntryHolder<T> ReadRegistryEntryHolder<T>(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        throw new NotImplementedException("TODO: Implement registryEntryHolder serialization in ProtocolSerializationExtensions.");
    }

    public static void WriteRegistryEntryHolder<T>(this ref MinecraftPrimitiveWriter writer, RegistryEntryHolder<T> value,
        int protocolVersion)
    {
        throw new NotImplementedException("TODO: Implement registryEntryHolder serialization in ProtocolSerializationExtensions.");
    }
}
