using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, 754)]
[Packet("play.toClient.entity", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
public sealed partial record EntityPacket(int EntityId) : IPacket<EntityPacket>, IPacket
{
    public static EntityPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        return new EntityPacket(entityId);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
    }

    public static PacketIdentity Identity => new("play.toClient.entity", "Entity", PacketPhase.Play, PacketDirection.Clientbound, 33);

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
