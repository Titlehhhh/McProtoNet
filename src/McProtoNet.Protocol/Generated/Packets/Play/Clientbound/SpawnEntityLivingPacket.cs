using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, 758)]
[Packet("play.toClient.spawn_entity_living", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("EntityUuid", "Guid")]
[PacketField("Type", "int")]
[PacketField("X", "double")]
[PacketField("Y", "double")]
[PacketField("Z", "double")]
[PacketField("Yaw", "int")]
[PacketField("Pitch", "int")]
[PacketField("HeadPitch", "int")]
[PacketField("VelocityX", "int")]
[PacketField("VelocityY", "int")]
[PacketField("VelocityZ", "int")]
public sealed partial record SpawnEntityLivingPacket(int EntityId, Guid EntityUuid, int Type, double X, double Y, double Z, int Yaw, int Pitch, int HeadPitch, int VelocityX, int VelocityY, int VelocityZ) : IPacket<SpawnEntityLivingPacket>, IPacket
{
    public static SpawnEntityLivingPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpawnEntityLivingPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        var entityUuid = reader.ReadUUID();
        var type = reader.ReadVarInt();
        var x = reader.ReadDouble();
        var y = reader.ReadDouble();
        var z = reader.ReadDouble();
        var yaw = reader.ReadSignedByte();
        var pitch = reader.ReadSignedByte();
        var headPitch = reader.ReadSignedByte();
        var velocityX = reader.ReadSignedShort();
        var velocityY = reader.ReadSignedShort();
        var velocityZ = reader.ReadSignedShort();
        return new SpawnEntityLivingPacket(entityId, entityUuid, type, x, y, z, yaw, pitch, headPitch, velocityX, velocityY, velocityZ);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpawnEntityLivingPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
        writer.WriteUUID(EntityUuid);
        writer.WriteVarInt(Type);
        writer.WriteDouble(X);
        writer.WriteDouble(Y);
        writer.WriteDouble(Z);
        writer.WriteSignedByte((sbyte)Yaw);
        writer.WriteSignedByte((sbyte)Pitch);
        writer.WriteSignedByte((sbyte)HeadPitch);
        writer.WriteSignedShort((short)VelocityX);
        writer.WriteSignedShort((short)VelocityY);
        writer.WriteSignedShort((short)VelocityZ);
    }

    public static PacketIdentity Identity => new("play.toClient.spawn_entity_living", "SpawnEntityLiving", PacketPhase.Play, PacketDirection.Clientbound, 93);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x02;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 758)
        {
            id = 0x02;
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
