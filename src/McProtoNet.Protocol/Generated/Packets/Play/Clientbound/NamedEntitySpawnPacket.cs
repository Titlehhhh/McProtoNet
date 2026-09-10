using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, 763)]
[Packet("play.toClient.named_entity_spawn", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("PlayerUuid", "Guid")]
[PacketField("X", "double")]
[PacketField("Y", "double")]
[PacketField("Z", "double")]
[PacketField("Yaw", "int")]
[PacketField("Pitch", "int")]
public sealed partial record NamedEntitySpawnPacket(int EntityId, Guid PlayerUuid, double X, double Y, double Z, int Yaw, int Pitch) : IPacket<NamedEntitySpawnPacket>, IPacket
{
    public static NamedEntitySpawnPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<NamedEntitySpawnPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        var playerUuid = reader.ReadUUID();
        var x = reader.ReadDouble();
        var y = reader.ReadDouble();
        var z = reader.ReadDouble();
        var yaw = reader.ReadSignedByte();
        var pitch = reader.ReadSignedByte();
        return new NamedEntitySpawnPacket(entityId, playerUuid, x, y, z, yaw, pitch);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<NamedEntitySpawnPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
        writer.WriteUUID(PlayerUuid);
        writer.WriteDouble(X);
        writer.WriteDouble(Y);
        writer.WriteDouble(Z);
        writer.WriteSignedByte((sbyte)Yaw);
        writer.WriteSignedByte((sbyte)Pitch);
    }

    public static PacketIdentity Identity => new("play.toClient.named_entity_spawn", "NamedEntitySpawn", PacketPhase.Play, PacketDirection.Clientbound, 62);

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
