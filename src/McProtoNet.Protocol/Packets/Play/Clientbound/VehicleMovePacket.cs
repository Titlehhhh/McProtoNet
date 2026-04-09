using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("VehicleMove", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x2C)]
[PacketId(751, 754, 0x2B)]
[PacketId(755, 758, 0x2C)]
[PacketId(759, 759, 0x29)]
[PacketId(760, 760, 0x2B)]
[PacketId(761, 761, 0x2A)]
[PacketId(762, 763, 0x2E)]
[PacketId(764, 765, 0x2F)]
[PacketId(766, 767, 0x31)]
[PacketId(768, 769, 0x33)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x32)]
public sealed partial class VehicleMovePacket : IServerPacket
{
    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
            {
                writer.WriteDouble(X);
                writer.WriteDouble(Y);
                writer.WriteDouble(Z);
                writer.WriteFloat(Yaw);
                writer.WriteFloat(Pitch);
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
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
            {
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                Yaw = reader.ReadFloat();
                Pitch = reader.ReadFloat();
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(VehicleMovePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public float Yaw { get; set; }
    public float Pitch { get; set; }
}