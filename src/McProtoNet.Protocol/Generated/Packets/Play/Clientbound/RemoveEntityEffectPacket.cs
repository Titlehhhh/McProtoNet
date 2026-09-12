using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.remove_entity_effect", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("EffectId", "int")]
public sealed partial record RemoveEntityEffectPacket(int EntityId, int EffectId) : IPacket<RemoveEntityEffectPacket>, IPacket
{
    public static RemoveEntityEffectPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RemoveEntityEffectPacket>(protocolVersion);
        if (protocolVersion <= 757)
        {
            var entityId = reader.ReadVarInt();
            var effectId = reader.ReadSignedByte();
            return new RemoveEntityEffectPacket(entityId, effectId);
        }

        if (protocolVersion >= 758)
        {
            var entityId = reader.ReadVarInt();
            var effectId = reader.ReadVarInt();
            return new RemoveEntityEffectPacket(entityId, effectId);
        }

        throw new System.NotSupportedException($"RemoveEntityEffectPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RemoveEntityEffectPacket>(protocolVersion);
        if (protocolVersion <= 757)
        {
            writer.WriteVarInt(EntityId);
            writer.WriteSignedByte((sbyte)EffectId);
            return;
        }

        if (protocolVersion >= 758)
        {
            writer.WriteVarInt(EntityId);
            writer.WriteVarInt(EffectId);
            return;
        }

        throw new System.NotSupportedException($"RemoveEntityEffectPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("EntityId");
        writer.WriteNumberValue(EntityId);
        writer.WritePropertyName("EffectId");
        writer.WriteNumberValue(EffectId);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.remove_entity_effect", "RemoveEntityEffect", PacketPhase.Play, PacketDirection.Clientbound, 80);

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
