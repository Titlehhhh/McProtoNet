using McProtoNet.NBT;

using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("OpenWindow", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x2E)]
[PacketId(751, 754, 0x2D)]
[PacketId(755, 758, 0x2E)]
[PacketId(759, 759, 0x2B)]
[PacketId(760, 760, 0x2D)]
[PacketId(761, 761, 0x2C)]
[PacketId(762, 763, 0x30)]
[PacketId(764, 765, 0x31)]
[PacketId(766, 767, 0x33)]
[PacketId(768, 769, 0x35)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x34)]
public sealed partial class OpenWindowPacket : IServerPacket
{
    public int WindowId { get; set; }
    public int InventoryType { get; set; }

    public VFirst_764Fields? VFirst_764 { get; set; }
    public V765_LastFields? V765_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(WindowId);
        writer.WriteVarInt(InventoryType);
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
            {
                var fields = VFirst_764 ?? throw new InvalidOperationException("OpenWindowPacket 1-764 fields missing.");
                writer.WriteString(fields.WindowTitle);
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V765_Last ?? throw new InvalidOperationException("OpenWindowPacket 765-last fields missing.");
                writer.WriteAnonymousNbtTag(fields.WindowTitle, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(OpenWindowPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        WindowId = reader.ReadVarInt();
        InventoryType = reader.ReadVarInt();
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
            {
                VFirst_764 = new VFirst_764Fields { WindowTitle = reader.ReadString() };
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                V765_Last = new V765_LastFields { WindowTitle = reader.ReadAnonymousNbtTag(protocolVersion) };
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(OpenWindowPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    public struct VFirst_764Fields
    {
        public string WindowTitle { get; set; }
    }

    public struct V765_LastFields
    {
        public NbtTag WindowTitle { get; set; }
    }
}