using System;
using McProtoNet.NBT;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("Disconnect", PacketState.Configuration, PacketDirection.Clientbound)]
public sealed partial class DisconnectPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(764, 764),
        new(765, MinecraftVersion.LatestProtocol)
    };
    public V764Fields? V764 { get; set; }
    public V765_LastFields? V765_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case 764:
            {
                var fields = V764 ?? throw new InvalidOperationException("Disconnect V764 fields missing.");
                writer.WriteString(fields.Reason);
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V765_Last ?? throw new InvalidOperationException("Disconnect V765_Last fields missing.");
                writer.WriteAnonymousNbtTag(fields.Reason, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerConfigurationPacket.Disconnect), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case 764:
                V764 = new V764Fields
                {
                    Reason = reader.ReadString()
                };
                return;
            case >= 765 and <= MinecraftVersion.LatestProtocol:
                V765_Last = new V765_LastFields
                {
                    Reason = reader.ReadNbtTag(readRootTag: false)
                };
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerConfigurationPacket.Disconnect), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V764Fields
    {
        public string Reason { get; set; }
    }

    public struct V765_LastFields
    {
        public NbtTag? Reason { get; set; }
    }
}