using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("OpenSignEntity", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class OpenSignEntityPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 762),
        new(763, MinecraftVersion.LatestProtocol),
    };

    public Position Location { get; set; }

    public V763_769Fields? V763_769 { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 762:
            {
                writer.WritePosition(Location, protocolVersion);
                return;
            }
            case >= 763 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V763_769 ?? throw new InvalidOperationException("OpenSignEntity V763_769 fields missing.");
                writer.WritePosition(Location, protocolVersion);
                writer.WriteBoolean(fields.IsFrontText);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.OpenSignEntity), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 762:
            {
                Location = reader.ReadPosition(protocolVersion);
                return;
            }
            case >= 763 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = new V763_769Fields();
                Location = reader.ReadPosition(protocolVersion);
                fields.IsFrontText = reader.ReadBoolean();
                V763_769 = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.OpenSignEntity), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V763_769Fields
    {
        public bool IsFrontText { get; set; }
    }

}
