using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("Position", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 767)]
[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x12)]
[PacketId(751, 754, 0x12)]
[PacketId(755, 758, 0x11)]
[PacketId(759, 759, 0x13)]
[PacketId(760, 760, 0x14)]
[PacketId(761, 761, 0x13)]
[PacketId(762, 763, 0x14)]
[PacketId(764, 764, 0x16)]
[PacketId(765, 765, 0x17)]
[PacketId(766, 767, 0x1A)]
[PacketId(768, 770, 0x1C)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x1D)]
public sealed partial class PositionPacket : IClientPacket
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public VFirst_767Fields? VFirst_767 { get; set; }
    public V768_LastFields? V768_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteDouble(X);
        writer.WriteDouble(Y);
        writer.WriteDouble(Z);
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
            {
                var fields = VFirst_767 ?? throw new InvalidOperationException("PositionPacket first-767 fields missing.");
                writer.WriteBoolean(fields.OnGround);
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V768_Last ?? throw new InvalidOperationException("PositionPacket 768-last fields missing.");
                writer.WriteType<MovementFlags>(fields.Flags, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(PositionPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        X = reader.ReadDouble();
        Y = reader.ReadDouble();
        Z = reader.ReadDouble();
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
                ThrowHelper.ThrowProtocolNotSupported(nameof(PositionPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_767Fields { public bool OnGround { get; set; } }
    public struct V768_LastFields { public MovementFlags Flags { get; set; } }
}