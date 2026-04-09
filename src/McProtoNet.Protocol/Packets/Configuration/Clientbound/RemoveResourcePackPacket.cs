using System;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("RemoveResourcePack", PacketState.Configuration, PacketDirection.Clientbound)]
public sealed partial class RemoveResourcePackPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(765, 765),
        new(766, MinecraftVersion.LatestProtocol)
    };

    public V765Fields? V765 { get; set; }
    public PacketCommonRemoveResourcePack? Data { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case 765:
            {
                var fields = V765 ?? throw new InvalidOperationException("RemoveResourcePack V765 fields missing.");
                if (fields.Uuid is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteUUID(fields.Uuid.Value);
                }
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                var data = Data ?? throw new InvalidOperationException("RemoveResourcePack data missing.");
                writer.WritePacketCommonRemoveResourcePack(data, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerConfigurationPacket.RemoveResourcePack), protocolVersion,
                    SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case 765:
                V765 = new V765Fields
                {
                    Uuid = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadUUID())
                };
                Data = null;
                return;
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                Data = reader.ReadPacketCommonRemoveResourcePack(protocolVersion);
                V765 = null;
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerConfigurationPacket.RemoveResourcePack), protocolVersion,
                    SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V765Fields
    {
        public Guid? Uuid { get; set; }
    }
}
