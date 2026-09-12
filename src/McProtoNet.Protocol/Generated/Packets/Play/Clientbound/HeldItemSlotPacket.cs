using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.held_item_slot", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Slot", "int")]
public sealed partial record HeldItemSlotPacket(int Slot) : IPacket<HeldItemSlotPacket>, IPacket
{
    public static HeldItemSlotPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<HeldItemSlotPacket>(protocolVersion);
        if (protocolVersion <= 768)
        {
            var slot = reader.ReadSignedByte();
            return new HeldItemSlotPacket(slot);
        }

        if (protocolVersion >= 769)
        {
            var slot = reader.ReadVarInt();
            return new HeldItemSlotPacket(slot);
        }

        throw new System.NotSupportedException($"HeldItemSlotPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<HeldItemSlotPacket>(protocolVersion);
        if (protocolVersion <= 768)
        {
            writer.WriteSignedByte((sbyte)Slot);
            return;
        }

        if (protocolVersion >= 769)
        {
            writer.WriteVarInt(Slot);
            return;
        }

        throw new System.NotSupportedException($"HeldItemSlotPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Slot");
        writer.WriteNumberValue(Slot);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.held_item_slot", "HeldItemSlot", PacketPhase.Play, PacketDirection.Clientbound, 50);

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
