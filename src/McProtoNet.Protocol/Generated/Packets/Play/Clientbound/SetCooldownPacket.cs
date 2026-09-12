using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.set_cooldown", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("CooldownTicks", "int")]
[PacketField("ItemId", "int", Group = "VUntil767", To = 767)]
[PacketField("CooldownGroup", "string", Group = "V768_Last", From = 768)]
public sealed partial record SetCooldownPacket(int CooldownTicks, SetCooldownPacket.VUntil767Layer? VUntil767 = null, SetCooldownPacket.V768_LastLayer? V768_Last = null) : IPacket<SetCooldownPacket>, IPacket
{
    public readonly record struct VUntil767Layer(int ItemId);
    public readonly record struct V768_LastLayer(string CooldownGroup);
    public static SetCooldownPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetCooldownPacket>(protocolVersion);
        if (protocolVersion <= 767)
        {
            var itemId = reader.ReadVarInt();
            var cooldownTicks = reader.ReadVarInt();
            return new SetCooldownPacket(cooldownTicks, VUntil767: new VUntil767Layer(itemId));
        }

        if (protocolVersion >= 768)
        {
            var cooldownGroup = reader.ReadString();
            var cooldownTicks = reader.ReadVarInt();
            return new SetCooldownPacket(cooldownTicks, V768_Last: new V768_LastLayer(cooldownGroup));
        }

        throw new System.NotSupportedException($"SetCooldownPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetCooldownPacket>(protocolVersion);
        if (protocolVersion <= 767)
        {
            var layer = VUntil767 ?? throw new WrongLayerException("SetCooldownPacket", protocolVersion, "VUntil767");
            int ItemId = layer.ItemId;
            writer.WriteVarInt(ItemId);
            writer.WriteVarInt(CooldownTicks);
            return;
        }

        if (protocolVersion >= 768)
        {
            var layer = V768_Last ?? throw new WrongLayerException("SetCooldownPacket", protocolVersion, "V768_Last");
            string CooldownGroup = layer.CooldownGroup;
            writer.WriteString(CooldownGroup);
            writer.WriteVarInt(CooldownTicks);
            return;
        }

        throw new System.NotSupportedException($"SetCooldownPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("CooldownTicks");
        writer.WriteNumberValue(CooldownTicks);
        if (VUntil767 is { } vUntil767)
        {
            writer.WritePropertyName("ItemId");
            writer.WriteNumberValue(vUntil767.ItemId);
        }
        else if (V768_Last is { } v768_Last)
        {
            writer.WritePropertyName("CooldownGroup");
            writer.WriteStringValue(v768_Last.CooldownGroup);
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.set_cooldown", "SetCooldown", PacketPhase.Play, PacketDirection.Clientbound, 91);

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
