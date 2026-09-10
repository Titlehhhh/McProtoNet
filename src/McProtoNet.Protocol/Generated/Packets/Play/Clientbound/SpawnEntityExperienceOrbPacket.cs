using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, 769)]
[Packet("play.toClient.spawn_entity_experience_orb", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("X", "double")]
[PacketField("Y", "double")]
[PacketField("Z", "double")]
[PacketField("Count", "int")]
public sealed partial record SpawnEntityExperienceOrbPacket(int EntityId, double X, double Y, double Z, int Count) : IPacket<SpawnEntityExperienceOrbPacket>, IPacket
{
    public static SpawnEntityExperienceOrbPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpawnEntityExperienceOrbPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        var x = reader.ReadDouble();
        var y = reader.ReadDouble();
        var z = reader.ReadDouble();
        var count = reader.ReadSignedShort();
        return new SpawnEntityExperienceOrbPacket(entityId, x, y, z, count);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpawnEntityExperienceOrbPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
        writer.WriteDouble(X);
        writer.WriteDouble(Y);
        writer.WriteDouble(Z);
        writer.WriteSignedShort((short)Count);
    }

    public static PacketIdentity Identity => new("play.toClient.spawn_entity_experience_orb", "SpawnEntityExperienceOrb", PacketPhase.Play, PacketDirection.Clientbound, 99);

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
