using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("VehicleMove", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x16)]
[PacketId(751, 754, 0x16)]
[PacketId(755, 758, 0x15)]
[PacketId(759, 759, 0x17)]
[PacketId(760, 760, 0x18)]
[PacketId(761, 761, 0x17)]
[PacketId(762, 763, 0x18)]
[PacketId(764, 764, 0x1A)]
[PacketId(765, 765, 0x1B)]
[PacketId(766, 767, 0x1E)]
[PacketId(768, 770, 0x20)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x21)]
public sealed partial class VehicleMovePacket : IClientPacket
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public float Yaw { get; set; }
    public float Pitch { get; set; }
    public bool? OnGround { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 768:
            {
                writer.WriteDouble(X);
                writer.WriteDouble(Y);
                writer.WriteDouble(Z);
                writer.WriteFloat(Yaw);
                writer.WriteFloat(Pitch);
                return;
            }
            case >= 769 and <= MinecraftVersion.LatestProtocol:
            {
                writer.WriteDouble(X);
                writer.WriteDouble(Y);
                writer.WriteDouble(Z);
                writer.WriteFloat(Yaw);
                writer.WriteFloat(Pitch);
                writer.WriteBoolean(OnGround.Value);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(VehicleMovePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 768:
            {
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                Yaw = reader.ReadFloat();
                Pitch = reader.ReadFloat();
                return;
            }
            case >= 769 and <= MinecraftVersion.LatestProtocol:
            {
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                Yaw = reader.ReadFloat();
                Pitch = reader.ReadFloat();
                OnGround = reader.ReadBoolean();
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(VehicleMovePacket), protocolVersion, SupportedVersions);
                return;
        }
    }
}