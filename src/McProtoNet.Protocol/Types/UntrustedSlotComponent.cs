using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

[ProtocolSupport(770, MinecraftVersion.LatestProtocol)]
public sealed partial record UntrustedSlotComponent(SlotComponentType Type, ByteArray Data)
{
    public static UntrustedSlotComponent Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        return reader.ReadUntrustedSlotComponent(protocolVersion);
    }

    public void Write(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteUntrustedSlotComponent(this, protocolVersion);
    }
}
