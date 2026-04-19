using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("TeleportConfirm", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x00)]
[PacketId(751, MinecraftVersion.LatestProtocol, 0x00)]
public sealed partial class TeleportConfirmPacket : IClientPacket
{
    public int TeleportId { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteVarInt(TeleportId);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => TeleportId = reader.ReadVarInt();
}