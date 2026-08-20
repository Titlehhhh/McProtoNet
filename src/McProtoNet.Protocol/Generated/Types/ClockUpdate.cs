using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;
[ProtocolSupport(775, MinecraftVersion.LatestProtocol)]
public readonly partial record struct ClockUpdate(int Id, long TotalTicks, float PartialTick, float Rate) : IProtocolType<ClockUpdate>
{
    public static ClockUpdate Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ClockUpdate>(protocolVersion);
        var id = reader.ReadVarInt();
        var totalTicks = reader.ReadVarLong();
        var partialTick = reader.ReadFloat();
        var rate = reader.ReadFloat();
        return new ClockUpdate(id, totalTicks, partialTick, rate);
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ClockUpdate>(protocolVersion);
        writer.WriteVarInt(Id);
        writer.WriteVarLong(TotalTicks);
        writer.WriteFloat(PartialTick);
        writer.WriteFloat(Rate);
    }
}
