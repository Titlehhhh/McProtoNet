using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SetCursorItem", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
[PacketId(768, 769, 0x5A)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x59)]
public sealed partial class SetCursorItemPacket : IServerPacket
{
    public Slot Contents { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                writer.WriteType<Slot>(Contents, protocolVersion);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SetCursorItemPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                Contents = reader.ReadType<Slot>(protocolVersion);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SetCursorItemPacket), protocolVersion, SupportedVersions);
                return;
        }
    }
}