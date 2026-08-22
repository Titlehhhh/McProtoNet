using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(775, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.spectate_entity", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("EntityId", "int")]
public sealed partial record SpectateEntityPacket(int EntityId) : IPacket<SpectateEntityPacket>, IPacket
{
    public static SpectateEntityPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpectateEntityPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        return new SpectateEntityPacket(entityId);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpectateEntityPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
    }

    public static PacketIdentity Identity => new("play.toServer.spectate_entity", "SpectateEntity", PacketPhase.Play, PacketDirection.Serverbound, 52);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x3E;
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
