using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("ScoreboardDisplayObjective", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class ScoreboardDisplayObjectivePacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 763),
        new(764, MinecraftVersion.LatestProtocol),
    };

    public string Name { get; set; }
    public sbyte Position { get; set; }



    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 763:
            {
                writer.WriteSignedByte(Position);
                writer.WriteString(Name);
                return;
            }
            case >= 764 and <= MinecraftVersion.LatestProtocol:
            {
                writer.WriteVarInt(Position);
                writer.WriteString(Name);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.ScoreboardDisplayObjective), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 763:
            {
                Position = reader.ReadSignedByte();
                Name = reader.ReadString();
                return;
            }
            case >= 764 and <= MinecraftVersion.LatestProtocol:
            {
                Position = reader.ReadVarInt();
                Name = reader.ReadString();
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.ScoreboardDisplayObjective), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);


}
