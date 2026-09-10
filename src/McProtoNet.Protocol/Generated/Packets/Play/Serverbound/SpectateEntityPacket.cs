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

    public static PacketIdentity Identity => new("play.toServer.spectate_entity", "SpectateEntity", PacketPhase.Play, PacketDirection.Serverbound, 54);

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
