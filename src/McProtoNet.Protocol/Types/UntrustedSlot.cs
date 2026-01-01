using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

[ProtocolSupport(770, MinecraftVersion.LatestProtocol)]
public sealed partial record UntrustedSlot(Slot Slot, IReadOnlyList<UntrustedSlotComponent> Components)
{
    public static UntrustedSlot Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        return reader.ReadUntrustedSlot(protocolVersion);
    }

    public void Write(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteUntrustedSlot(this, protocolVersion);
    }
}
