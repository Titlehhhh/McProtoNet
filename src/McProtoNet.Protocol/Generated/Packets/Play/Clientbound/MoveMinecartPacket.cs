using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.move_minecart", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("Steps", "MinecartStep[]")]
public sealed partial record MoveMinecartPacket(int EntityId, MinecartStep[] Steps) : IPacket<MoveMinecartPacket>, IPacket
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

    public static PacketIdentity Identity => new("play.toClient.move_minecart", "MoveMinecart", PacketPhase.Play, PacketDirection.Clientbound, 60);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
