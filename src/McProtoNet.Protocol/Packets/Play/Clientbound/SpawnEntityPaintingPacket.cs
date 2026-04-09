using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SpawnEntityPainting", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class SpawnEntityPaintingPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 758),
    };

    public int EntityId { get; set; }
    public Guid EntityUUID { get; set; }
    public int Title { get; set; }
    public Position Location { get; set; }
    public byte Direction { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
                writer.WriteVarInt(EntityId);
                writer.WriteUUID(EntityUUID);
                writer.WriteVarInt(Title);
                writer.WritePosition(Location, protocolVersion);
                writer.WriteUnsignedByte(Direction);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.SpawnEntityPainting), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
                EntityId = reader.ReadVarInt();
                EntityUUID = reader.ReadUUID();
                Title = reader.ReadVarInt();
                Location = reader.ReadPosition(protocolVersion);
                Direction = reader.ReadUnsignedByte();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.SpawnEntityPainting), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
