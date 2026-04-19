using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("WorldBorderLerpSize", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[PacketId(755, 758, 0x43)]
[PacketId(759, 759, 0x42)]
[PacketId(760, 760, 0x45)]
[PacketId(761, 761, 0x44)]
[PacketId(762, 763, 0x48)]
[PacketId(764, 764, 0x4A)]
[PacketId(765, 765, 0x4C)]
[PacketId(766, 767, 0x4E)]
[PacketId(768, 769, 0x53)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x52)]
public sealed partial class WorldBorderLerpSizePacket : IServerPacket
{
    public double OldDiameter { get; set; }
    public double NewDiameter { get; set; }

    public V755_758Fields? V755_758 { get; set; }
    public V759_LastFields? V759_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteDouble(OldDiameter);
        writer.WriteDouble(NewDiameter);
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            {
                var fields = V755_758 ?? throw new InvalidOperationException("WorldBorderLerpSizePacket 755-758 fields missing.");
                writer.WriteSignedLong(fields.Speed);
                return;
            }
            case >= 759 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V759_Last ?? throw new InvalidOperationException("WorldBorderLerpSizePacket 759-last fields missing.");
                writer.WriteVarInt(fields.Speed);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(WorldBorderLerpSizePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        OldDiameter = reader.ReadDouble();
        NewDiameter = reader.ReadDouble();
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            {
                V755_758 = new V755_758Fields { Speed = reader.ReadSignedLong() };
                V759_Last = null;
                return;
            }
            case >= 759 and <= MinecraftVersion.LatestProtocol:
            {
                V759_Last = new V759_LastFields { Speed = reader.ReadVarInt() };
                V755_758 = null;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(WorldBorderLerpSizePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    public struct V755_758Fields { public long Speed { get; set; } }
    public struct V759_LastFields { public int Speed { get; set; } }
}