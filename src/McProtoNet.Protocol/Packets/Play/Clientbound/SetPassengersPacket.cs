using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("SetPassengers", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x4B)]
[PacketId(751, 754, 0x4B)]
[PacketId(755, 759, 0x54)]
[PacketId(760, 760, 0x57)]
[PacketId(761, 761, 0x55)]
[PacketId(762, 763, 0x59)]
[PacketId(764, 764, 0x5B)]
[PacketId(765, 765, 0x5D)]
[PacketId(766, 767, 0x5F)]
[PacketId(768, 769, 0x65)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x64)]
public sealed partial class SetPassengersPacket : IServerPacket
{
    public int EntityId { get; set; }
    public int[] Passengers { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(EntityId);
                writer.WriteArray(Passengers, LengthFormat.VarInt);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SetPassengersPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                EntityId = reader.ReadVarInt();
                Passengers = reader.ReadArray<int>(LengthFormat.VarInt);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SetPassengersPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}