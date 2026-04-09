using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Extensions;

public static partial class ProtocolSerializationExtensions
{
    public static SlotComponentType ReadSlotComponentType(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SlotComponentType>(protocolVersion);
        int id = reader.ReadVarInt();
        return protocolVersion switch
        {
            766 => SlotComponentTypeExtensions.FromId766(id, protocolVersion),
            767 => SlotComponentTypeExtensions.FromId767(id, protocolVersion),
            >= 768 and <= 769 => SlotComponentTypeExtensions.FromId768To769(id, protocolVersion),
            >= 770 and <= 772 => SlotComponentTypeExtensions.FromId770To772(id, protocolVersion),
            _ => throw new InvalidOperationException($"Unknown SlotComponentType id {id} for protocol {protocolVersion}.")
        };
    }

    public static void WriteSlotComponentType(this MinecraftPrimitiveWriter writer, SlotComponentType value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SlotComponentType>(protocolVersion);
        int id = protocolVersion switch
        {
            766 => SlotComponentTypeExtensions.ToId766(value, protocolVersion),
            767 => SlotComponentTypeExtensions.ToId767(value, protocolVersion),
            >= 768 and <= 769 => SlotComponentTypeExtensions.ToId768To769(value, protocolVersion),
            >= 770 and <= 772 => SlotComponentTypeExtensions.ToId770To772(value, protocolVersion),
            _ => throw new InvalidOperationException($"Unknown SlotComponentType {value} for protocol {protocolVersion}.")
        };
        writer.WriteVarInt(id);
    }
}
