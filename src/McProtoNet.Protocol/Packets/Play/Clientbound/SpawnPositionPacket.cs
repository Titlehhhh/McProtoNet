using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SpawnPosition", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class SpawnPositionPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 754),
        new(755, MinecraftVersion.LatestProtocol),
    };

    public Position Location { get; set; }

    public V755_LastFields? V755_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
                writer.WritePosition(Location, protocolVersion);
                return;
            case >= 755 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V755_Last ?? throw new InvalidOperationException("SpawnPosition V755_Last missing.");
                writer.WritePosition(Location, protocolVersion);
                writer.WriteFloat(fields.Angle);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.SpawnPosition), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
                Location = reader.ReadPosition(protocolVersion);
                return;
            case >= 755 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = new V755_LastFields();
                Location = reader.ReadPosition(protocolVersion);
                fields.Angle = reader.ReadFloat();
                V755_Last = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.SpawnPosition), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V755_LastFields
    {
        public float Angle { get; set; }
    }
}
