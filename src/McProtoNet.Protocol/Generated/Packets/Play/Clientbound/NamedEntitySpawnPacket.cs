using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
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

    public static PacketIdentity Identity => new("play.toClient.named_entity_spawn", "NamedEntitySpawn", PacketPhase.Play, PacketDirection.Clientbound, 57);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x04;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 758)
        {
            id = 0x04;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 761)
        {
            id = 0x02;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x03;
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
