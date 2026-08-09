using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.spawn_entity", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("ObjectUuid", "Guid")]
[PacketField("Type", "int")]
[PacketField("X", "double")]
[PacketField("Y", "double")]
[PacketField("Z", "double")]
[PacketField("Pitch", "int")]
[PacketField("Yaw", "int")]
[PacketField("ObjectData", "int")]
[PacketField("VelocityX", "int")]
[PacketField("VelocityY", "int")]
[PacketField("VelocityZ", "int")]
[PacketField("HeadPitch", "int", Group = "V759_Last", From = 759)]
public sealed partial record SpawnEntityPacket(int EntityId, Guid ObjectUuid, int Type, double X, double Y, double Z, int Pitch, int Yaw, int ObjectData, int VelocityX, int VelocityY, int VelocityZ, SpawnEntityPacket.V759_LastLayer? V759_Last = null) : IPacket<SpawnEntityPacket>
{
    public readonly record struct V759_LastLayer(int HeadPitch);
    public static SpawnEntityPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpawnEntityPacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            var entityId = reader.ReadVarInt();
            var objectUuid = reader.ReadUUID();
            var type = reader.ReadVarInt();
            var x = reader.ReadDouble();
            var y = reader.ReadDouble();
            var z = reader.ReadDouble();
            var pitch = reader.ReadSignedByte();
            var yaw = reader.ReadSignedByte();
            var objectData = reader.ReadSignedInt();
            var velocityX = reader.ReadSignedShort();
            var velocityY = reader.ReadSignedShort();
            var velocityZ = reader.ReadSignedShort();
            return new SpawnEntityPacket(entityId, objectUuid, type, x, y, z, pitch, yaw, objectData, velocityX, velocityY, velocityZ);
        }

        if (protocolVersion >= 759)
        {
            var entityId = reader.ReadVarInt();
            var objectUuid = reader.ReadUUID();
            var type = reader.ReadVarInt();
            var x = reader.ReadDouble();
            var y = reader.ReadDouble();
            var z = reader.ReadDouble();
            var pitch = reader.ReadSignedByte();
            var yaw = reader.ReadSignedByte();
            var headPitch = reader.ReadSignedByte();
            var objectData = reader.ReadVarInt();
            var velocityX = reader.ReadSignedShort();
            var velocityY = reader.ReadSignedShort();
            var velocityZ = reader.ReadSignedShort();
            return new SpawnEntityPacket(entityId, objectUuid, type, x, y, z, pitch, yaw, objectData, velocityX, velocityY, velocityZ, V759_Last: new V759_LastLayer(headPitch));
        }

        throw new System.NotSupportedException($"SpawnEntityPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpawnEntityPacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            writer.WriteVarInt(EntityId);
            writer.WriteUUID(ObjectUuid);
            writer.WriteVarInt(Type);
            writer.WriteDouble(X);
            writer.WriteDouble(Y);
            writer.WriteDouble(Z);
            writer.WriteSignedByte((sbyte)Pitch);
            writer.WriteSignedByte((sbyte)Yaw);
            writer.WriteSignedInt(ObjectData);
            writer.WriteSignedShort((short)VelocityX);
            writer.WriteSignedShort((short)VelocityY);
            writer.WriteSignedShort((short)VelocityZ);
            return;
        }

        if (protocolVersion >= 759)
        {
            var layer = V759_Last ?? throw new WrongLayerException("SpawnEntityPacket", protocolVersion, "V759_Last");
            int HeadPitch = layer.HeadPitch;
            writer.WriteVarInt(EntityId);
            writer.WriteUUID(ObjectUuid);
            writer.WriteVarInt(Type);
            writer.WriteDouble(X);
            writer.WriteDouble(Y);
            writer.WriteDouble(Z);
            writer.WriteSignedByte((sbyte)Pitch);
            writer.WriteSignedByte((sbyte)Yaw);
            writer.WriteSignedByte((sbyte)HeadPitch);
            writer.WriteVarInt(ObjectData);
            writer.WriteSignedShort((short)VelocityX);
            writer.WriteSignedShort((short)VelocityY);
            writer.WriteSignedShort((short)VelocityZ);
            return;
        }

        throw new System.NotSupportedException($"SpawnEntityPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.spawn_entity", "SpawnEntity", PacketPhase.Play, PacketDirection.Clientbound, 12);

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x00;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 761)
        {
            id = 0x00;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 772)
        {
            id = 0x01;
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
