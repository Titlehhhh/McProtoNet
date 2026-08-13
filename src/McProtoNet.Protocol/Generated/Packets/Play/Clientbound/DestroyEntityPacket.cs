using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(755, 755)]
[Packet("play.toClient.destroy_entity", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
public sealed partial record DestroyEntityPacket(int EntityId) : IPacket<DestroyEntityPacket>, IPacket
{
    public static DestroyEntityPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DestroyEntityPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        return new DestroyEntityPacket(entityId);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DestroyEntityPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
    }

    public static PacketIdentity Identity => new("play.toClient.destroy_entity", "DestroyEntity", PacketPhase.Play, PacketDirection.Clientbound, 27);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 755 && protocolVersion <= 755)
        {
            id = 0x3A;
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
