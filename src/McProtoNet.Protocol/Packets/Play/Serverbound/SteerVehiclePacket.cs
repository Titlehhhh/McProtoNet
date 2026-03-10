using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("SteerVehicle", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 767)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x1D)]
[PacketId(751, 754, 0x1D)]
[PacketId(755, 758, 0x1C)]
[PacketId(759, 759, 0x1E)]
[PacketId(760, 760, 0x1F)]
[PacketId(761, 761, 0x1E)]
[PacketId(762, 763, 0x1F)]
[PacketId(764, 764, 0x22)]
[PacketId(765, 765, 0x23)]
[PacketId(766, 767, 0x26)]
public sealed partial class SteerVehiclePacket : IClientPacket
{
    public VFirst_767Fields? VFirst_767 { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
            {
                var fields = VFirst_767 ?? throw new InvalidOperationException("SteerVehiclePacket first-767 fields missing.");
                writer.WriteFloat(fields.Sideways);
                writer.WriteFloat(fields.Forward);
                writer.WriteUnsignedByte(fields.Jump);
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SteerVehiclePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
                VFirst_767 = new VFirst_767Fields
                {
                    Sideways = reader.ReadFloat(),
                    Forward = reader.ReadFloat(),
                    Jump = reader.ReadUnsignedByte()
                };
                return;
            case >= 768 and <= MinecraftVersion.LatestProtocol:
                VFirst_767 = null;
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SteerVehiclePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_767Fields
    {
        public float Sideways { get; set; }
        public float Forward { get; set; }
        public byte Jump { get; set; }
    }
}