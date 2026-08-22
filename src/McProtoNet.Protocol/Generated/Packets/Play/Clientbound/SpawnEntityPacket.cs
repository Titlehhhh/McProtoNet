using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
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
[PacketField("VelocityX", "int", Group = "VUntil758", To = 758)]
[PacketField("VelocityY", "int", Group = "VUntil758", To = 758)]
[PacketField("VelocityZ", "int", Group = "VUntil758", To = 758)]
[PacketField("HeadPitch", "int", Group = "V759_772", From = 759, To = 772)]
[PacketField("VelocityX", "int", Group = "V759_772", From = 759, To = 772)]
[PacketField("VelocityY", "int", Group = "V759_772", From = 759, To = 772)]
[PacketField("VelocityZ", "int", Group = "V759_772", From = 759, To = 772)]
[PacketField("HeadPitch", "int", Group = "V773_Last", From = 773)]
[PacketField("Velocity", "LpVec3", Group = "V773_Last", From = 773)]
public sealed partial record SpawnEntityPacket(int EntityId, Guid ObjectUuid, int Type, double X, double Y, double Z, int Pitch, int Yaw, int ObjectData, SpawnEntityPacket.VUntil758Layer? VUntil758 = null, SpawnEntityPacket.V759_772Layer? V759_772 = null, SpawnEntityPacket.V773_LastLayer? V773_Last = null) : IPacket<SpawnEntityPacket>, IPacket
{
    public readonly record struct VUntil758Layer(int VelocityX, int VelocityY, int VelocityZ);
    public readonly record struct V759_772Layer(int HeadPitch, int VelocityX, int VelocityY, int VelocityZ);
    public readonly record struct V773_LastLayer(int HeadPitch, LpVec3 Velocity);
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
            return new SpawnEntityPacket(entityId, objectUuid, type, x, y, z, pitch, yaw, objectData, VUntil758: new VUntil758Layer(velocityX, velocityY, velocityZ));
        }

        if (protocolVersion >= 759 && protocolVersion <= 772)
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
            return new SpawnEntityPacket(entityId, objectUuid, type, x, y, z, pitch, yaw, objectData, V759_772: new V759_772Layer(headPitch, velocityX, velocityY, velocityZ));
        }

        if (protocolVersion >= 773)
        {
            var entityId = reader.ReadVarInt();
            var objectUuid = reader.ReadUUID();
            var type = reader.ReadVarInt();
            var x = reader.ReadDouble();
            var y = reader.ReadDouble();
            var z = reader.ReadDouble();
            var velocity = reader.ReadType<LpVec3>(protocolVersion);
            var pitch = reader.ReadSignedByte();
            var yaw = reader.ReadSignedByte();
            var headPitch = reader.ReadSignedByte();
            var objectData = reader.ReadVarInt();
            return new SpawnEntityPacket(entityId, objectUuid, type, x, y, z, pitch, yaw, objectData, V773_Last: new V773_LastLayer(headPitch, velocity));
        }

        throw new System.NotSupportedException($"SpawnEntityPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpawnEntityPacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            var layer = VUntil758 ?? throw new WrongLayerException("SpawnEntityPacket", protocolVersion, "VUntil758");
            int VelocityX = layer.VelocityX;
            int VelocityY = layer.VelocityY;
            int VelocityZ = layer.VelocityZ;
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

        if (protocolVersion >= 759 && protocolVersion <= 772)
        {
            var layer = V759_772 ?? throw new WrongLayerException("SpawnEntityPacket", protocolVersion, "V759_772");
            int HeadPitch = layer.HeadPitch;
            int VelocityX = layer.VelocityX;
            int VelocityY = layer.VelocityY;
            int VelocityZ = layer.VelocityZ;
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

        if (protocolVersion >= 773)
        {
            var layer = V773_Last ?? throw new WrongLayerException("SpawnEntityPacket", protocolVersion, "V773_Last");
            int HeadPitch = layer.HeadPitch;
            LpVec3 Velocity = layer.Velocity;
            writer.WriteVarInt(EntityId);
            writer.WriteUUID(ObjectUuid);
            writer.WriteVarInt(Type);
            writer.WriteDouble(X);
            writer.WriteDouble(Y);
            writer.WriteDouble(Z);
            writer.WriteType<LpVec3>(Velocity, protocolVersion);
            writer.WriteSignedByte((sbyte)Pitch);
            writer.WriteSignedByte((sbyte)Yaw);
            writer.WriteSignedByte((sbyte)HeadPitch);
            writer.WriteVarInt(ObjectData);
            return;
        }

        throw new System.NotSupportedException($"SpawnEntityPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.spawn_entity", "SpawnEntity", PacketPhase.Play, PacketDirection.Clientbound, 91);

    PacketIdentity IPacket.Identity => Identity;

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

        if (protocolVersion >= 762 && protocolVersion <= 776)
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
