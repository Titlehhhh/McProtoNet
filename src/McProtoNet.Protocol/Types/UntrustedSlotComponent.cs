using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

[ProtocolSupport(770, MinecraftVersion.LatestProtocol)]
public sealed partial record UntrustedSlotComponent(SlotComponentType Type, ByteArray Data)
{
    public static UntrustedSlotComponent Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UntrustedSlotComponent>(protocolVersion);
        var type = SlotComponentTypeExtensions.Read(ref reader, protocolVersion);
        var data = ByteArray.Read(ref reader, protocolVersion);
        return new UntrustedSlotComponent(type, data);
    }

    public void Write(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UntrustedSlotComponent>(protocolVersion);
        Type.Write(ref writer, protocolVersion);
        Data.Write(ref writer, protocolVersion);
    }
}
