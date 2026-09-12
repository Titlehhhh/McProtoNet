using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.entity_velocity", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("VelocityX", "int", Group = "VUntil772", To = 772)]
[PacketField("VelocityY", "int", Group = "VUntil772", To = 772)]
[PacketField("VelocityZ", "int", Group = "VUntil772", To = 772)]
[PacketField("Velocity", "LpVec3", Group = "V773_Last", From = 773)]
public sealed partial record EntityVelocityPacket(int EntityId, EntityVelocityPacket.VUntil772Layer? VUntil772 = null, EntityVelocityPacket.V773_LastLayer? V773_Last = null) : IPacket<EntityVelocityPacket>, IPacket
{
    public readonly record struct VUntil772Layer(int VelocityX, int VelocityY, int VelocityZ);
    public readonly record struct V773_LastLayer(LpVec3 Velocity);
    public static EntityVelocityPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityVelocityPacket>(protocolVersion);
        if (protocolVersion <= 772)
        {
            var entityId = reader.ReadVarInt();
            var velocityX = reader.ReadSignedShort();
            var velocityY = reader.ReadSignedShort();
            var velocityZ = reader.ReadSignedShort();
            return new EntityVelocityPacket(entityId, VUntil772: new VUntil772Layer(velocityX, velocityY, velocityZ));
        }

        if (protocolVersion >= 773)
        {
            var entityId = reader.ReadVarInt();
            var velocity = reader.ReadType<LpVec3>(protocolVersion);
            return new EntityVelocityPacket(entityId, V773_Last: new V773_LastLayer(velocity));
        }

        throw new System.NotSupportedException($"EntityVelocityPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityVelocityPacket>(protocolVersion);
        if (protocolVersion <= 772)
        {
            var layer = VUntil772 ?? throw new WrongLayerException("EntityVelocityPacket", protocolVersion, "VUntil772");
            int VelocityX = layer.VelocityX;
            int VelocityY = layer.VelocityY;
            int VelocityZ = layer.VelocityZ;
            writer.WriteVarInt(EntityId);
            writer.WriteSignedShort((short)VelocityX);
            writer.WriteSignedShort((short)VelocityY);
            writer.WriteSignedShort((short)VelocityZ);
            return;
        }

        if (protocolVersion >= 773)
        {
            var layer = V773_Last ?? throw new WrongLayerException("EntityVelocityPacket", protocolVersion, "V773_Last");
            LpVec3 Velocity = layer.Velocity;
            writer.WriteVarInt(EntityId);
            writer.WriteType<LpVec3>(Velocity, protocolVersion);
            return;
        }

        throw new System.NotSupportedException($"EntityVelocityPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("EntityId");
        writer.WriteNumberValue(EntityId);
        if (VUntil772 is { } vUntil772)
        {
            writer.WritePropertyName("VelocityX");
            writer.WriteNumberValue(vUntil772.VelocityX);
            writer.WritePropertyName("VelocityY");
            writer.WriteNumberValue(vUntil772.VelocityY);
            writer.WritePropertyName("VelocityZ");
            writer.WriteNumberValue(vUntil772.VelocityZ);
        }
        else if (V773_Last is { } v773_Last)
        {
            writer.WritePropertyName("Velocity");
            v773_Last.Velocity.WriteJson(writer);
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.entity_velocity", "EntityVelocity", PacketPhase.Play, PacketDirection.Clientbound, 42);

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
