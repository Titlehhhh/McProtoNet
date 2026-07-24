using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
public sealed partial class MoveMinecartPacket : IProtocolType<MoveMinecartPacket>
{
    public int EntityId { get; }
    public MinecartStep[] Steps { get; }

    public MoveMinecartPacket(int entityId, MinecartStep[] steps)
    {
        EntityId = entityId;
        Steps = steps;
    }

    public static MoveMinecartPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<MoveMinecartPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        int stepsCount = reader.ReadVarInt();
        var steps = new MinecartStep[stepsCount];
        for (int i = 0; i < steps.Length; i++)
            steps[i] = reader.ReadType<MinecartStep>(protocolVersion);
        return new MoveMinecartPacket(entityId, steps);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<MoveMinecartPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
        writer.WriteVarInt(Steps.Length);
        foreach (var stepsItem in Steps)
            writer.WriteType<MinecartStep>(stepsItem, protocolVersion);
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 768 && protocolVersion <= 769)
            return 0x31;
        if (protocolVersion >= 770 && protocolVersion <= 770)
            return 0x30;
        if (protocolVersion >= 771 && protocolVersion <= 772)
            return 0x30;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
