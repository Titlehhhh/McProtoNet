using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.move_minecart", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("Steps", "MinecartStep[]")]
public sealed partial record MoveMinecartPacket(int EntityId, MinecartStep[] Steps) : IPacket<MoveMinecartPacket>
{
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

    public static PacketIdentity Identity => new("play.toClient.move_minecart", "MoveMinecart", PacketPhase.Play, PacketDirection.Clientbound, 7);

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x31;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x30;
            return true;
        }

        id = 0;
        return false;
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (TryGetPacketId(protocolVersion, out var id))
            return id;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
