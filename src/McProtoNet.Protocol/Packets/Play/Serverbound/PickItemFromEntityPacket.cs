using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("PickItemFromEntity", PacketState.Play, PacketDirection.Serverbound)]
public sealed partial class PickItemFromEntityPacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(769, MinecraftVersion.LatestProtocol)
    };

    public int EntityId { get; set; }
    public bool IncludeData { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 769 and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(EntityId);
                writer.WriteBoolean(IncludeData);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.PickItemFromEntity), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 769 and <= MinecraftVersion.LatestProtocol:
                EntityId = reader.ReadVarInt();
                IncludeData = reader.ReadBoolean();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.PickItemFromEntity), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
