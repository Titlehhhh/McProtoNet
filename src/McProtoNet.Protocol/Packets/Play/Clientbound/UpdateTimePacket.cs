using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("UpdateTime", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x4E)]
[PacketId(751, 754, 0x4E)]
[PacketId(755, 756, 0x58)]
[PacketId(757, 759, 0x59)]
[PacketId(760, 760, 0x5C)]
[PacketId(761, 761, 0x5A)]
[PacketId(762, 763, 0x5E)]
[PacketId(764, 764, 0x60)]
[PacketId(765, 765, 0x62)]
[PacketId(766, 767, 0x64)]
[PacketId(768, 769, 0x6B)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x6A)]
public sealed partial class UpdateTimePacket : IServerPacket
{
    public long Age { get; set; }
    public long Time { get; set; }
    public V768_LastFields? V768_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteSignedLong(Age);
        writer.WriteSignedLong(Time);
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
            {
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V768_Last ?? throw new InvalidOperationException("UpdateTimePacket 768-last fields missing.");
                writer.WriteBoolean(fields.TickDayTime);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(UpdateTimePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Age = reader.ReadSignedLong();
        Time = reader.ReadSignedLong();
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
            {
                V768_Last = null;
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                V768_Last = new V768_LastFields { TickDayTime = reader.ReadBoolean() };
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(UpdateTimePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    public struct V768_LastFields
    {
        public bool TickDayTime { get; set; }
    }
}