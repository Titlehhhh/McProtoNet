using McProtoNet.NBT;
using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("KickDisconnect", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 764)]
[ProtocolSupport(765, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x1A)]
[PacketId(751, 754, 0x19)]
[PacketId(755, 758, 0x1A)]
[PacketId(759, 759, 0x17)]
[PacketId(760, 760, 0x19)]
[PacketId(761, 761, 0x17)]
[PacketId(762, 763, 0x1A)]
[PacketId(764, 765, 0x1B)]
[PacketId(766, 769, 0x1D)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x1C)]
public sealed partial class KickDisconnectPacket : IServerPacket
{
    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
            {
                var fields = VFirst_764 ?? throw new InvalidOperationException("KickDisconnectPacket first-764 fields missing.");
                writer.WriteString(fields.Reason);
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V765_Last ?? throw new InvalidOperationException("KickDisconnectPacket 765-last fields missing.");
                writer.WriteAnonymousNbtTag(fields.Reason, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(KickDisconnectPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
                VFirst_764 = new VFirst_764Fields { Reason = reader.ReadString() };
                V765_Last = null;
                return;
            case >= 765 and <= MinecraftVersion.LatestProtocol:
                V765_Last = new V765_LastFields { Reason = reader.ReadAnonymousNbtTag(protocolVersion) };
                VFirst_764 = null;
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(KickDisconnectPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public VFirst_764Fields? VFirst_764 { get; set; }
    public V765_LastFields? V765_Last { get; set; }

    public struct VFirst_764Fields { public string Reason { get; set; } }
    public struct V765_LastFields { public NbtTag Reason { get; set; } }
}