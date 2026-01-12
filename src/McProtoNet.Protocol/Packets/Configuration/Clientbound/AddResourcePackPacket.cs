using System;
using McProtoNet.NBT;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("AddResourcePack", PacketState.Configuration, PacketDirection.Clientbound)]
public sealed partial class AddResourcePackPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(765, 765),
        new(766, MinecraftVersion.LatestProtocol)
    };

    public V765Fields? V765 { get; set; }
    public PacketCommonAddResourcePack? Data { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case 765:
            {
                var fields = V765 ?? throw new InvalidOperationException("AddResourcePack V765 fields missing.");
                writer.WriteUUID(fields.Uuid);
                writer.WriteString(fields.Url);
                writer.WriteString(fields.Hash);
                writer.WriteBoolean(fields.Forced);
                writer.WriteAnonOptionalNbtTag(fields.PromptMessage, protocolVersion);
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                var data = Data ?? throw new InvalidOperationException("AddResourcePack data missing.");
                writer.WritePacketCommonAddResourcePack(data, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerConfigurationPacket.AddResourcePack), protocolVersion,
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
                    Uuid = reader.ReadUUID(),
                    Url = reader.ReadString(),
                    Hash = reader.ReadString(),
                    Forced = reader.ReadBoolean(),
                    PromptMessage = reader.ReadOptionalNbtTag(readRootTag: false)
                };
                Data = null;
                return;
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                Data = reader.ReadPacketCommonAddResourcePack(protocolVersion);
                V765 = null;
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerConfigurationPacket.AddResourcePack), protocolVersion,
                    SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V765Fields
    {
        public Guid Uuid { get; set; }
        public string Url { get; set; }
        public string Hash { get; set; }
        public bool Forced { get; set; }
        public NbtTag? PromptMessage { get; set; }
    }
}
