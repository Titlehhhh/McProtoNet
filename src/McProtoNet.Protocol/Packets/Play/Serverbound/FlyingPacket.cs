using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("Flying", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 767)]
[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x15)]
[PacketId(751, 754, 0x15)]
[PacketId(755, 758, 0x14)]
[PacketId(759, 759, 0x16)]
[PacketId(760, 760, 0x17)]
[PacketId(761, 761, 0x16)]
[PacketId(762, 763, 0x17)]
[PacketId(764, 764, 0x19)]
[PacketId(765, 765, 0x1A)]
[PacketId(766, 767, 0x1D)]
[PacketId(768, 770, 0x1F)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x20)]
public sealed partial class FlyingPacket : IClientPacket
{
    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
            {
                var fields = VFirst_767 ?? throw new InvalidOperationException("FlyingPacket first-767 fields missing.");
                writer.WriteBoolean(fields.OnGround);
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V768_Last ?? throw new InvalidOperationException("FlyingPacket 768-last fields missing.");
                writer.WriteType<MovementFlags>(fields.Flags, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(FlyingPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
                VFirst_767 = new VFirst_767Fields { OnGround = reader.ReadBoolean() };
                V768_Last = null;
                return;
            case >= 768 and <= MinecraftVersion.LatestProtocol:
                V768_Last = new V768_LastFields { Flags = reader.ReadType<MovementFlags>(protocolVersion) };
                VFirst_767 = null;
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(FlyingPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public VFirst_767Fields? VFirst_767 { get; set; }
    public V768_LastFields? V768_Last { get; set; }

    public struct VFirst_767Fields { public bool OnGround { get; set; } }
    public struct V768_LastFields { public MovementFlags Flags { get; set; } }
}