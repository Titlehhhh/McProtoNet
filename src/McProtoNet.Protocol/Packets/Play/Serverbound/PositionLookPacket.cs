using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("PositionLook", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x13)]
[PacketId(751, 754, 0x13)]
[PacketId(755, 758, 0x12)]
[PacketId(759, 759, 0x14)]
[PacketId(760, 760, 0x15)]
[PacketId(761, 761, 0x14)]
[PacketId(762, 763, 0x15)]
[PacketId(764, 764, 0x17)]
[PacketId(765, 765, 0x18)]
[PacketId(766, 767, 0x1B)]
[PacketId(768, 770, 0x1D)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x1E)]
public sealed partial class PositionLookPacket : IClientPacket
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public float Yaw { get; set; }
    public float Pitch { get; set; }

    public VFirst_767Fields? VFirst_767 { get; set; }
    public V768_LastFields? V768_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteDouble(X);
        writer.WriteDouble(Y);
        writer.WriteDouble(Z);
        writer.WriteFloat(Yaw);
        writer.WriteFloat(Pitch);
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
            {
                var fields = VFirst_767 ?? throw new InvalidOperationException("PositionLookPacket 0-767 fields missing.");
                writer.WriteBoolean(fields.OnGround);
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V768_Last ?? throw new InvalidOperationException("PositionLookPacket 768-last fields missing.");
                writer.WriteType<MovementFlags>(fields.Flags, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(PositionLookPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        X = reader.ReadDouble();
        Y = reader.ReadDouble();
        Z = reader.ReadDouble();
        Yaw = reader.ReadFloat();
        Pitch = reader.ReadFloat();
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
            {
                VFirst_767 = new VFirst_767Fields { OnGround = reader.ReadBoolean() };
                V768_Last = null;
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                V768_Last = new V768_LastFields { Flags = reader.ReadType<MovementFlags>(protocolVersion) };
                VFirst_767 = null;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(PositionLookPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    public struct VFirst_767Fields { public bool OnGround { get; set; } }
    public struct V768_LastFields { public MovementFlags Flags { get; set; } }
}