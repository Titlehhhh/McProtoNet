using System;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

[PacketInfo("ResourcePackReceive", PacketState.Configuration, PacketDirection.Serverbound)]
public sealed partial class ResourcePackReceivePacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(764, 764),
        new(765, MinecraftVersion.LatestProtocol)
    };

    public int Result { get; set; }
    public V765_769Fields? V765_769 { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case 764:
                writer.WriteVarInt(Result);
                return;
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V765_769 ?? throw new InvalidOperationException("ResourcePackReceive V765_769 fields missing.");
                writer.WriteUUID(fields.Uuid);
                writer.WriteVarInt(Result);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientConfigurationPacket.ResourcePackReceive), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case 764:
                Result = reader.ReadVarInt();
                V765_769 = null;
                return;
            case >= 765 and <= MinecraftVersion.LatestProtocol:
                V765_769 = new V765_769Fields
                {
                    Uuid = reader.ReadUUID()
                };
                Result = reader.ReadVarInt();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientConfigurationPacket.ResourcePackReceive), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V765_769Fields
    {
        public Guid Uuid { get; set; }
    }
}