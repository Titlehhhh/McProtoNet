using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

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

    public static PacketIdentity Identity => new("play.toClient.entity", "Entity", PacketPhase.Play, PacketDirection.Clientbound, 31);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x2B;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x2A;
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
