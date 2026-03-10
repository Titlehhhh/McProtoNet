using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("CloseWindow", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 765)]
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x13)]
[PacketId(751, 754, 0x12)]
[PacketId(755, 758, 0x13)]
[PacketId(759, 760, 0x10)]
[PacketId(761, 761, 0x0F)]
[PacketId(762, 763, 0x11)]
[PacketId(764, 769, 0x12)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x11)]
public sealed partial class CloseWindowPacket : IServerPacket
{
    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
            {
                var fields = VFirst_765 ?? throw new InvalidOperationException("CloseWindowPacket first-765 fields missing.");
                writer.WriteUnsignedByte(fields.WindowId);
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V766_Last ?? throw new InvalidOperationException("CloseWindowPacket 766-last fields missing.");
                writer.WriteType(fields.WindowId, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(CloseWindowPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
                VFirst_765 = new VFirst_765Fields { WindowId = reader.ReadUnsignedByte() };
                V766_Last = null;
                return;
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                V766_Last = new V766_LastFields { WindowId = reader.ReadType<ContainerID>(protocolVersion) };
                VFirst_765 = null;
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(CloseWindowPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public VFirst_765Fields? VFirst_765 { get; set; }
    public V766_LastFields? V766_Last { get; set; }

    public struct VFirst_765Fields
    {
        public byte WindowId { get; set; }
    }

    public struct V766_LastFields
    {
        public ContainerID WindowId { get; set; }
    }
}