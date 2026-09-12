using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;
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

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("EntityId");
        writer.WriteNumberValue(EntityId);
        writer.WritePropertyName("ObjectUuid");
        writer.WriteStringValue(ObjectUuid);
        writer.WritePropertyName("Type");
        writer.WriteNumberValue(Type);
        writer.WritePropertyName("X");
        if (double.IsFinite(X))
            writer.WriteNumberValue(X);
        else
            writer.WriteStringValue(double.IsNaN(X) ? "NaN" : X > 0 ? "Infinity" : "-Infinity");
        writer.WritePropertyName("Y");
        if (double.IsFinite(Y))
            writer.WriteNumberValue(Y);
        else
            writer.WriteStringValue(double.IsNaN(Y) ? "NaN" : Y > 0 ? "Infinity" : "-Infinity");
        writer.WritePropertyName("Z");
        if (double.IsFinite(Z))
            writer.WriteNumberValue(Z);
        else
            writer.WriteStringValue(double.IsNaN(Z) ? "NaN" : Z > 0 ? "Infinity" : "-Infinity");
        writer.WritePropertyName("Pitch");
        writer.WriteNumberValue(Pitch);
        writer.WritePropertyName("Yaw");
        writer.WriteNumberValue(Yaw);
        writer.WritePropertyName("ObjectData");
        writer.WriteNumberValue(ObjectData);
        if (VUntil758 is { } vUntil758)
        {
            writer.WritePropertyName("VelocityX");
            writer.WriteNumberValue(vUntil758.VelocityX);
            writer.WritePropertyName("VelocityY");
            writer.WriteNumberValue(vUntil758.VelocityY);
            writer.WritePropertyName("VelocityZ");
            writer.WriteNumberValue(vUntil758.VelocityZ);
        }
        else if (V759_772 is { } v759_772)
        {
            writer.WritePropertyName("HeadPitch");
            writer.WriteNumberValue(v759_772.HeadPitch);
            writer.WritePropertyName("VelocityX");
            writer.WriteNumberValue(v759_772.VelocityX);
            writer.WritePropertyName("VelocityY");
            writer.WriteNumberValue(v759_772.VelocityY);
            writer.WritePropertyName("VelocityZ");
            writer.WriteNumberValue(v759_772.VelocityZ);
        }
        else if (V773_Last is { } v773_Last)
        {
            writer.WritePropertyName("HeadPitch");
            writer.WriteNumberValue(v773_Last.HeadPitch);
            writer.WritePropertyName("Velocity");
            v773_Last.Velocity.WriteJson(writer);
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.spawn_entity", "SpawnEntity", PacketPhase.Play, PacketDirection.Clientbound, 99);

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
