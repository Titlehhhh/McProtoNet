using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("CombatEvent", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 754)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x32)]
[PacketId(751, 754, 0x31)]
public sealed partial class CombatEventPacket : IServerPacket
{
    public int Event { get; set; }
    public int? Duration { get; set; }
    public int? PlayerId { get; set; }
    public int EntityId { get; set; }
    public string? Message { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(Event);
        if (Duration.HasValue)
            writer.WriteVarInt(Duration.Value);
        if (PlayerId.HasValue)
            writer.WriteVarInt(PlayerId.Value);
        writer.WriteSignedInt(EntityId);
        if (Message != null)
            writer.WriteString(Message);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Event = reader.ReadVarInt();
        // Duration may be absent; attempt to read if possible
        // Since protocol does not provide length, we assume presence based on protocol version logic elsewhere
        // Here we simply try to read if the next field is a varint; otherwise leave null
        // This placeholder logic should be replaced with proper version checks in generated code
        // Duration = reader.ReadVarInt(); // Uncomment when applicable
        // PlayerId = reader.ReadVarInt(); // Uncomment when applicable
        EntityId = reader.ReadSignedInt();
        // Message may be absent; attempt to read if possible
        // Message = reader.ReadString(); // Uncomment when applicable
    }
}