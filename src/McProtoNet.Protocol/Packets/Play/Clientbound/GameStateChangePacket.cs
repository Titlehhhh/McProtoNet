using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("GameStateChange", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x1E)]
[PacketId(751, 754, 0x1D)]
[PacketId(755, 758, 0x1E)]
[PacketId(759, 759, 0x1B)]
[PacketId(760, 760, 0x1D)]
[PacketId(761, 761, 0x1C)]
[PacketId(762, 763, 0x1F)]
[PacketId(764, 765, 0x20)]
[PacketId(766, 767, 0x22)]
[PacketId(768, 769, 0x23)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x22)]
public sealed partial class GameStateChangePacket : IServerPacket
{
    public byte Reason { get; set; }
    public float GameMode { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 770:
                writer.WriteUnsignedByte(Reason);
                writer.WriteFloat(GameMode);
                return;

            case >= 771 and <= MinecraftVersion.LatestProtocol:
                writer.WriteUnsignedByte(Reason);
                writer.WriteFloat(GameMode);
                return;

            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(GameStateChangePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 770:
                Reason = reader.ReadUnsignedByte();
                GameMode = reader.ReadFloat();
                return;

            case >= 771 and <= MinecraftVersion.LatestProtocol:
                Reason = reader.ReadUnsignedByte();
                GameMode = reader.ReadFloat();
                return;

            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(GameStateChangePacket), protocolVersion, SupportedVersions);
                return;
        }
    }
}