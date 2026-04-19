using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("ConfigurationAcknowledged", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[PacketId(764, 765, 0x0B)]
[PacketId(766, 767, 0x0C)]
[PacketId(768, 770, 0x0E)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x0F)]
public sealed partial class ConfigurationAcknowledgedPacket : IClientPacket
{
    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
                return;

            case >= 764 and <= MinecraftVersion.LatestProtocol:
            { 
                // No fields defined for this range
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ConfigurationAcknowledgedPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
                // No fields defined for this range
                return;
            case >= 764 and <= MinecraftVersion.LatestProtocol:
            {
                // No fields defined for this range
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ConfigurationAcknowledgedPacket), protocolVersion, SupportedVersions);
                return;
        }
    }
}