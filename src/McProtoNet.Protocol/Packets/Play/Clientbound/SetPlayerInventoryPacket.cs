using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SetPlayerInventory", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
[PacketId(768, 769, 0x66)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x65)]
public sealed partial class SetPlayerInventoryPacket : IServerPacket
{
    public int SlotId { get; set; }

    public VFirst_768Fields? VFirst_768 { get; set; }
    public V769_LastFields? V769_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(SlotId);
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
                return;
            case >= 768 and <= 768:
            {
                var fields = VFirst_768 ?? throw new InvalidOperationException("SetPlayerInventoryPacket 768 fields missing.");
                writer.WriteType<Slot>(fields.Contents, protocolVersion);
                return;
            }
            case >= 769 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V769_Last ?? throw new InvalidOperationException("SetPlayerInventoryPacket 769-last fields missing.");
                writer.WriteType<Slot>(fields.Contents, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SetPlayerInventoryPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        SlotId = reader.ReadVarInt();
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
            {
                return;
            }
            case >= 768 and <= 768:
            {
                VFirst_768 = new VFirst_768Fields { Contents = reader.ReadType<Slot>(protocolVersion) };
                return;
            }
            case >= 769 and <= MinecraftVersion.LatestProtocol:
            {
                V769_Last = new V769_LastFields { Contents = reader.ReadType<Slot>(protocolVersion) };
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SetPlayerInventoryPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    public struct VFirst_768Fields { public Slot Contents { get; set; } }
    public struct V769_LastFields { public Slot Contents { get; set; } }
}