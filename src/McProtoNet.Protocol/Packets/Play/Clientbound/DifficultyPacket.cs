using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("Difficulty", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class DifficultyPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 770),
        new(771, MinecraftVersion.LatestProtocol),
    };

    public byte Difficulty { get; set; }

    public VFirst_769Fields? VFirst_769 { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 770:
            {
                writer.WriteUnsignedByte(Difficulty);
                return;
            }
            case >= 771 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = VFirst_769 ?? throw new InvalidOperationException("Difficulty VFirst_769 fields missing.");
                writer.WriteUnsignedByte(Difficulty);
                writer.WriteBoolean(fields.DifficultyLocked);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.Difficulty), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 770:
            {
                Difficulty = reader.ReadUnsignedByte();
                return;
            }
            case >= 771 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = new VFirst_769Fields();
                Difficulty = reader.ReadUnsignedByte();
                fields.DifficultyLocked = reader.ReadBoolean();
                VFirst_769 = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.Difficulty), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_769Fields
    {
        public bool DifficultyLocked { get; set; }
    }

}
